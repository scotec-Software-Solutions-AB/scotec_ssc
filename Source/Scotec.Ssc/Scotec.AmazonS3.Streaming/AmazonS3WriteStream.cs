#region

using System.Collections.Concurrent;
using System.Diagnostics.CodeAnalysis;
using Amazon.S3;
using Amazon.S3.Model;

#endregion

namespace Scotec.AmazonS3.Streaming;

/// <summary>
///     <para>
///         For the upload as well as for the MultiPartUpload, the AmazonS3Client needs the total size of the content
///         to be transferred before the upload begins. Often, however, the size is unknown. This can be the case,
///         for example, when the content to be transferred is generated. So that the upload can take place, the entire
///         content must first be available either in the memory or on a storage medium. With large amounts of data, this
///         quickly leads to a load on the main memory or to performance losses due to hard disk accesses.
///     </para>
///     <para>
///         The scotec-AmazonS3WriteStream is able to perform a multipart upload without knowing the size of the content
///         beforehand. However, this means that the multipart upload cannot be completed automatically. Therefore,
///         Flush() must be called after all data has been passed to the stream. Disposing the stream without first
///         calling Flush() will abort the upload. All parts transferred to the bucket so far are deleted in this case.
///     </para>
/// </summary>
public class AmazonS3WriteStream : Stream
{
    // Amazon S3 multipart upload limits
    //  Maximum object size: 5 TB
    //  Maximum number of parts per upload: 10,000
    //  Part numbers: 1 to 10,000 (inclusive)
    //  Part size: 5 MiB to 5 GiB.There is no minimum size limit on the last part of your multipart upload.
    //  Maximum number of parts returned for a list parts request:_ 1000
    //  Maximum number of multipart uploads returned in a list multipart uploads request: 1000

    private static readonly long MaxPartSize = 5L * 1024 * 1024 * 1024;
    private static readonly long MinPartSize = 5L * 1024 * 1024;
    private static readonly int ParallelPartsDefault = 2;

    private readonly string _bucket;
    private readonly CancellationTokenSource _cancellationTokenSource = new();
    private readonly AmazonS3Client _client;
    private readonly ConcurrentDictionary<int, string> _eTags = new();
    private readonly ConcurrentDictionary<int, Exception> _exceptions = new();
    private readonly string _key;
    private readonly long _partSize;
    private readonly SemaphoreSlim _partsLock;
    private readonly List<Task> _uploadTasks = new();
    private bool _isDisposed;

    private bool _isError;

    private bool _isFlushed;
    private long _length;
    private int _partNumber;
    private long _position;
    private Stream _stream;
    private string _uploadId;

    public AmazonS3WriteStream(AmazonS3Client client, string bucket, string key)
        : this(client, bucket, key, MinPartSize, ParallelPartsDefault)
    {
    }

    public AmazonS3WriteStream(AmazonS3Client client, string bucket, string key, long partSize)
        : this(client, bucket, key, partSize, ParallelPartsDefault)
    {
    }

    public AmazonS3WriteStream(AmazonS3Client client, string bucket, string key, int maxParallelParts)
        : this(client, bucket, key, MinPartSize, maxParallelParts)
    {
    }

    public AmazonS3WriteStream([NotNull] AmazonS3Client client, [NotNull] string bucket, string key, long partSize,
        int maxParallelParts)
    {
        if (string.IsNullOrEmpty(bucket)) throw new ArgumentException("Value cannot be null or empty.", nameof(bucket));

        if (string.IsNullOrEmpty(key)) throw new ArgumentException("Value cannot be null or empty.", nameof(key));

        if (partSize < MinPartSize || partSize > MaxPartSize)
            throw new ArgumentOutOfRangeException(nameof(partSize), "Value must be within the range of 5MB to 5GB.");

        if (maxParallelParts < 1)
            throw new ArgumentOutOfRangeException(nameof(maxParallelParts), "Value must not be lower than 1.");

        _client = client ?? throw new ArgumentNullException(nameof(client));
        _bucket = bucket;
        _key = key;
        _partSize = Math.Min(Math.Max(partSize, MinPartSize), MaxPartSize);
        _partsLock = new SemaphoreSlim(maxParallelParts);
    }

    public override bool CanRead => false;
    public override bool CanSeek => false;
    public override bool CanWrite => true;
    public override long Length => _length;

    public override long Position
    {
        get => _position;
        set => throw new NotImplementedException();
    }

    public override void Flush()
    {
        if (_isDisposed) throw new InvalidOperationException("Stream is already disposed.");

        if (_isFlushed) throw new InvalidOperationException("Stream can only be flushed once.");

        if (_isError) ThrowCurrentErrorState();

        _isFlushed = true;

        CompleteUpload();
    }

    private void ThrowCurrentErrorState()
    {
        if (_isError)
        {
            throw new AggregateException(_exceptions.Values);
        }
    }

    public override int Read(byte[] buffer, int offset, int count)
    {
        throw new NotImplementedException();
    }

    public override long Seek(long offset, SeekOrigin origin)
    {
        throw new NotImplementedException();
    }

    public override void SetLength(long value)
    {
        throw new NotImplementedException();
    }

    public override void Write(byte[] buffer, int offset, int count)
    {
        if (_isDisposed) throw new InvalidOperationException("Stream is already disposed.");

        if (_isFlushed)
            throw new InvalidOperationException("Stream does not support writing after it has been flushed.");

        if (offset + count > buffer.Length) throw new AccessViolationException();

        do
        {
            var stream = GetStream();
            var toWrite = (int)Math.Min(_partSize - stream.Length, count);

            stream.Write(buffer, offset, toWrite);

            offset += toWrite;
            count -= toWrite;
            _position += toWrite;
            _length += toWrite;

            if (stream.Length == _partSize) WritePart(false);
        } while (count > 0 && !_cancellationTokenSource.IsCancellationRequested);
    }

    protected override void Dispose(bool disposing)
    {
        if (_isDisposed) return;

        _isDisposed = true;

        if (disposing)
            if (!_isFlushed)
                AbortUpload();

        base.Dispose(disposing);
    }


    private void AbortUpload()
    {
        var task = Task.Factory.StartNew(
            async _ => { await AbortUploadAsync(); }, this, CancellationToken.None,
            TaskCreationOptions.DenyChildAttach, TaskScheduler.Default);

        task.Wait();
    }

    private async Task AbortUploadAsync()
    {
        if (string.IsNullOrEmpty(_uploadId)) return;

        // Make sure that all uploads have been completed or cancelled.
        _cancellationTokenSource.Cancel();
        Task.WaitAll(_uploadTasks.ToArray());

        var request = new AbortMultipartUploadRequest
        {
            BucketName = _bucket,
            Key = _key,
            UploadId = _uploadId
        };
        var response = await _client.AbortMultipartUploadAsync(request);
        // TODO: Check response value. Call AbortMultipartUploadAsync() again in case of error. 
        // TODO: List parts th check whether all patrs have benn removed after calling AbortMultipartUploadAsync(). 
    }


    private void CompleteUpload()
    {
        var task = Task.Factory.StartNew(
            async state => { await CompleteUploadAsync(); }, this, CancellationToken.None,
            TaskCreationOptions.DenyChildAttach, TaskScheduler.Default);

        task.Wait();
    }

    private async Task CompleteUploadAsync()
    {
        WritePart(true);
        Task.WaitAll(_uploadTasks.ToArray());

        var request = new CompleteMultipartUploadRequest
        {
            BucketName = _bucket,
            Key = _key,
            PartETags = _eTags.Select(e => new PartETag(e.Key, e.Value)).ToList(),
            UploadId = _uploadId
        };

        var response = await _client.CompleteMultipartUploadAsync(request, _cancellationTokenSource.Token);
        // TODO: Check response value. Call CompleteMultipartUploadAsync() again in case of error. 
        // TODO: If the second call fails as well, abort the upload and throw an exception.
    }

    private Stream GetStream()
    {
        return _stream ??= new MemoryStream();
    }

    private void InitializeMultiPartUpload()
    {
        _uploadId = _client.InitiateMultipartUploadAsync(new InitiateMultipartUploadRequest
        {
            BucketName = _bucket,
            Key = _key
        }).GetAwaiter().GetResult().UploadId;
    }

    private void WritePart(bool isDisposing)
    {
        if (_cancellationTokenSource.IsCancellationRequested) return;

        if (string.IsNullOrEmpty(_uploadId)) InitializeMultiPartUpload();

        var partNumber = ++_partNumber;
        var stream = GetStream();
        stream.Position = 0;
        _stream = null;

        _partsLock.Wait();
        if (_cancellationTokenSource.IsCancellationRequested) return;

        _uploadTasks.Add(Task.Run(async () =>
        {
            var uploadRequest = new UploadPartRequest
            {
                BucketName = _bucket,
                Key = _key,
                UploadId = _uploadId,
                PartNumber = partNumber,
                InputStream = stream,
                IsLastPart = isDisposing,
                PartSize = stream.Length
            };

            try
            {
                var response = await _client.UploadPartAsync(uploadRequest, _cancellationTokenSource.Token);
                _eTags.TryAdd(partNumber, response.ETag);
            }
            catch (TaskCanceledException)
            {
                // Don't report cancellation exceptions.
            }
            catch (Exception e)
            {
                _isError = true;
                if (!_cancellationTokenSource.IsCancellationRequested)
                {
                    _cancellationTokenSource.Cancel();
                }

                _exceptions.TryAdd(partNumber, e);
            }
            finally
            {
                _partsLock.Release();
            }
        }));
    }
}
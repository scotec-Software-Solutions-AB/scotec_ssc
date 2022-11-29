using System.Runtime.InteropServices;

namespace Scotec.RingBuffer;

/// <summary>
/// 
/// </summary>
/// <typeparam name="TDataType"></typeparam>
public class RingBuffer<TDataType> where TDataType : unmanaged
{
    /// <summary>
    /// Number of records within the array. Not the size in bytes. 
    /// </summary>
    private readonly long _bufferSize;

    /// <summary>
    /// Raw data
    /// </summary>
    private readonly byte[] _data;

    /// <summary>
    /// Size of the data type to store.
    /// </summary>
    private readonly int _dataTypeSize = Marshal.SizeOf(typeof(TDataType));

    /// <summary>
    /// Synchronization object.
    /// </summary>
    private readonly object _syncObject = new();

    /// <summary>
    /// The current read postion.
    /// </summary>
    private long _readPointer = -1;

    /// <summary>
    /// The number of records currently hold in the ring buffer.
    /// If this value exceeds the buffer size, an overflow occurs and the ring buffer is reset. 
    /// </summary>
    private long _readWriteCounter;

    /// <summary>
    /// The current write position.
    /// </summary>
    private long _writePointer;

    /// <summary>
    ///     Create a RingBuffer of given size.
    /// </summary>
    /// <param name="bufferSize">The size of the RingBuffer. This is the number records, not the size in bytes.</param>
    public RingBuffer(long bufferSize)
    {
        _data = new byte[bufferSize * _dataTypeSize];
        _bufferSize = bufferSize;
    }

    private long ReadPointer
    {
        get => Interlocked.Read(ref _readPointer);
        set => Interlocked.Exchange(ref _readPointer, value % _bufferSize);
    }

    private long WritePointer
    {
        get => Interlocked.Read(ref _writePointer);
        set => Interlocked.Exchange(ref _writePointer, value % _bufferSize);
    }

    /// <summary>
    ///     Writes a list of values into the buffer.
    /// </summary>
    /// <param name="values">The list of values to write.</param>
    public void Write(TDataType[] values)
    {
        Write(values, values.Length);
    }

    /// <summary>
    ///     Writes an array of values into the buffer.
    /// </summary>
    /// <param name="values">The array of values to write.</param>
    /// <param name="count">The number of values to write.</param>
    public void Write(TDataType[] values, int count)
    {
        if (count < 0 || count > values.Length)
            throw new ArgumentOutOfRangeException(nameof(count),
                "Value cannot be negative or greater than length of values");

        if (count == 0)
            return;

        // It cannot be guaranteed that enough data will be read during the write operation.
        if (Interlocked.Read(ref _readWriteCounter) + count  > _bufferSize)
        {
            // Data overflow. Reset all data.
            lock (_syncObject)
            {
                // Some data may have been read while waiting for the lock. Check again.
                if (Interlocked.Read(ref _readWriteCounter) + count > _bufferSize)
                {
                    ReadPointer = 0;
                    WritePointer = 0;
                    _readWriteCounter = 0;

                    //TODO: Send an overflow event
                }
            }
        }

        if (count < _bufferSize - WritePointer)
        {
            // Write all data at once.
            BufferCopy(values, 0, _data, WritePointer * _dataTypeSize, count * _dataTypeSize);
        }
        else
        {
            // Write data partially to the end of the buffer and write the remaining data to the begin of the buffer.
            var remainingLength = _bufferSize - WritePointer;
            var nextLength = count - remainingLength;

            // copy until the end of buffer
            BufferCopy(values, 0, _data, WritePointer * _dataTypeSize, remainingLength * _dataTypeSize);

            // copy remaining part of values into start of buffer
            BufferCopy(values, remainingLength, _data, 0, nextLength * _dataTypeSize);
        }

        WritePointer += count;
        Interlocked.Add(ref _readWriteCounter, count);

        // First data have been written to the buffer. Set the read pointer to the first record.
        if (ReadPointer == -1)
            ReadPointer = 0;
    }

    /// <summary>
    ///     Reads a list of values from the buffer.
    /// </summary>
    /// <returns>The list of values.</returns>
    public TDataType[] Read()
    {
        TDataType[] outValues;

        lock (_syncObject)
        {
            var writePointer = WritePointer;
            var readPointer = ReadPointer;

            if (readPointer == -1 || readPointer == writePointer) return null;

            long length;
            if (writePointer > readPointer)
            {
                // Buffer can be read at once.
                length = writePointer - readPointer;
                outValues = new TDataType[length];

                BufferCopy(_data, readPointer * _dataTypeSize, outValues, 0, length * _dataTypeSize);
            }
            else
            {
                // Buffer has to be copied in to steps. 
                var remainingLength = _bufferSize - readPointer;
                length = remainingLength + writePointer;
                outValues = new TDataType[length];

                // copy from current position to the end of the array.
                BufferCopy(_data, readPointer * _dataTypeSize, outValues, 0, remainingLength * _dataTypeSize);

                // copy from the beginning of the array up to the current write pointer
                BufferCopy(_data, 0, outValues, remainingLength * _dataTypeSize, writePointer * _dataTypeSize);
            }

            ReadPointer += length;
            Interlocked.Add(ref _readWriteCounter, -length);
        }

        return outValues;
    }


    private unsafe void BufferCopy<TSourceType, TDestinationType>(TSourceType[] source, long sourceOffset,
        TDestinationType[] destination, long destinationOffset, long count)
        where TSourceType : unmanaged
        where TDestinationType : unmanaged
    {
        fixed (void* sourcePointer = &source[0], destinationPointer = &destination[0])
        {
            Buffer.MemoryCopy((byte*)sourcePointer + sourceOffset, (byte*)destinationPointer + destinationOffset,
                _bufferSize * _dataTypeSize, count);
        }
    }
}
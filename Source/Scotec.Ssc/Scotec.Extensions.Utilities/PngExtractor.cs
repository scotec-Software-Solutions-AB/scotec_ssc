namespace Scotec.Extensions.Utilities;

/// <summary>
///     Provides functionality to extract PNG images from a byte array.
/// </summary>
/// <remarks>
///     This class is designed to locate and extract PNG image data embedded within a byte array.
///     It identifies the PNG data by searching for the PNG signature and end marker.
/// </remarks>
public class PngExtractor
{
    /// <summary>
    ///     Represents the PNG file signature used to identify the start of a PNG image.
    /// </summary>
    /// <remarks>
    ///     The PNG signature is a sequence of 8 bytes that uniquely identifies a PNG file.
    ///     It is used by the <see cref="PngExtractor" /> class to locate the beginning of a PNG image
    ///     within a byte array.
    /// </remarks>
    private static readonly byte[] PngSignature = [0x89, 0x50, 0x4E, 0x47, 0x0D, 0x0A, 0x1A, 0x0A];

    /// <summary>
    ///     Represents the PNG file end marker used to identify the conclusion of a PNG image.
    /// </summary>
    /// <remarks>
    ///     The PNG end marker is a specific sequence of bytes that signifies the end of a PNG file.
    ///     It is used by the <see cref="PngExtractor" /> class to locate the termination of a PNG image
    ///     within a byte array.
    /// </remarks>
    private static readonly byte[] PngEndSignature = [0x00, 0x00, 0x00, 0x00, 0x49, 0x45, 0x4E, 0x44, 0xAE, 0x42, 0x60, 0x82];

    /// <summary>
    ///     Extracts a PNG image from the specified byte array.
    /// </summary>
    /// <param name="byteArray">
    ///     The byte array containing the PNG image data. This array is expected to include
    ///     the PNG signature and end marker.
    /// </param>
    /// <returns>
    ///     A byte array representing the extracted PNG image. The returned array includes
    ///     the PNG signature, image data, and end marker.
    /// </returns>
    /// <exception cref="Exception">
    ///     Thrown if the PNG signature or end marker is not found in the provided byte array.
    /// </exception>
    /// <remarks>
    ///     This method searches the provided byte array for a PNG image by identifying the
    ///     PNG signature and end marker. If both are found, the method extracts the PNG image
    ///     data and returns it as a new byte array.
    /// </remarks>
    public static byte[] ExtractPng(byte[] byteArray)
    {
        var span = byteArray.AsSpan();
        var startIndex = GetPngStartIndex(span);
        if (startIndex == -1)
        {
            throw new Exception("PNG signature not found in the byte array.");
        }

        var endIndex = GetPngEndIndex(span, startIndex);
        if (endIndex == -1)
        {
            throw new Exception("PNG end marker not found in the byte array.");
        }

        var pngLength = endIndex - startIndex + PngEndSignature.Length;
        var pngBytes = span.Slice(startIndex, pngLength).ToArray();

        return pngBytes;
    }

    /// <summary>
    ///     Finds the starting index of the PNG signature within the specified span of bytes.
    /// </summary>
    /// <param name="span">
    ///     The span of bytes to search for the PNG signature.
    /// </param>
    /// <returns>
    ///     The zero-based index of the first occurrence of the PNG signature within the span,
    ///     or -1 if the PNG signature is not found.
    /// </returns>
    /// <remarks>
    ///     This method scans the provided span of bytes for the PNG signature, which is a unique
    ///     sequence of bytes that marks the beginning of a PNG image. If the signature is found,
    ///     the method returns the index of its first byte; otherwise, it returns -1.
    /// </remarks>
    private static int GetPngStartIndex(Span<byte> span)
    {
        for (var i = 0; i <= span.Length - PngSignature.Length; i++)
        {
            if (span.Slice(i, PngSignature.Length).SequenceEqual(PngSignature))
            {
                return i;
            }
        }

        return -1; // PNG signature not found
    }

    /// <summary>
    ///     Finds the index of the PNG end marker within the specified span of bytes, starting from the given index.
    /// </summary>
    /// <param name="span">
    ///     The span of bytes to search for the PNG end marker.
    /// </param>
    /// <param name="startIndex">
    ///     The index within the span at which to begin the search.
    /// </param>
    /// <returns>
    ///     The zero-based index of the start of the PNG end marker within the span, or -1 if the marker is not found.
    /// </returns>
    /// <remarks>
    ///     This method scans the provided span of bytes starting from the specified index to locate the PNG end marker.
    ///     The PNG end marker is identified using the predefined <see cref="PngEndSignature" />.
    /// </remarks>
    private static int GetPngEndIndex(Span<byte> span, int startIndex)
    {
        for (var i = startIndex; i <= span.Length - PngEndSignature.Length; i++)
        {
            if (span.Slice(i, PngEndSignature.Length).SequenceEqual(PngEndSignature))
            {
                return i;
            }
        }

        return -1; // PNG end marker not found
    }
}

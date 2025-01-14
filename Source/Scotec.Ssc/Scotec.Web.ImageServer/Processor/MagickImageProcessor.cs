﻿using System.Drawing;
using ImageMagick;
using Scotec.Web.ImageServer.Provider;

namespace Scotec.Web.ImageServer.Processor;

public class MagickImageProcessor : IImageProcessor
{
    public async Task<ImageResponse> ProcessImageAsync(ImageRequest request, IImageProvider imageProvider)
    {
        try
        {
            var source = await imageProvider.GetImageAsync(request.Path);
            var format = GetImageType(request.Path);

            using var image = new MagickImage(source, format);
            image.Format = request.Format.HasValue ? Convert(request.Format.Value) : format;

            if ((request.Width != null && request.Width != image.Width) ||
                (request.Height != null && request.Height != image.Height))
            {
                var newWidth = request.Width ?? image.Width * (request.Height / (decimal)image.Height);
                var newHeight = request.Height ?? image.Height * (request.Width / (decimal)image.Width);

                var copyRect =
                    CalculateCopyRegion(image.BaseWidth, image.BaseHeight, newWidth!.Value, newHeight!.Value);

                var region = new MagickGeometry(copyRect.X, copyRect.Y, (uint)copyRect.Width, (uint)copyRect.Height)
                {
                    IgnoreAspectRatio = true,
                    Greater = true,
                    Less = true
                };

                image.Crop(region);
                image.Density = new Density(96, 96);

                image.Resize((uint?)request.Width ?? image.BaseWidth, (uint?)request.Height ?? image.BaseHeight);
            }

            var response = new ImageResponse(request.Path)
            {
                Format = Convert(image.Format),
                Width = request.Width,
                Height = request.Height,
                Image = image.ToByteArray(),
            };

            return response;
        }
        catch (Exception e) when (e is not ImageServerException)
        {
            throw new ImageServerException($"Error while processing image. Path:{request.Path}", e);
        }
    }

    private static MagickFormat GetImageType(string path)
    {
        var extension = Path.GetExtension(path).ToLower();

        return extension switch
        {
            ".bmp" => MagickFormat.Bmp,
            ".jpg" => MagickFormat.Jpeg,
            ".jepg" => MagickFormat.Jpeg,
            ".gif" => MagickFormat.Gif,
            ".ico" => MagickFormat.Ico,
            ".png" => MagickFormat.Png,
            ".webp" => MagickFormat.WebP,
            _ => throw new ImageServerException($"Unsupported file format: {extension}")
        };
    }


    private MagickFormat Convert(ImageFormat format)
    {
        if (!Enum.TryParse(format.ToString(), true, out MagickFormat magickFormat))
            throw new ImageServerException($"Unsupported image format: {format}");

        return magickFormat;
    }

    private ImageFormat Convert(MagickFormat format)
    {
        if (!Enum.TryParse(format.ToString(), true, out ImageFormat imageFormat))
            throw new ImageServerException($"Unsupported image format: {format}");

        return imageFormat;
    }

    private Rectangle CalculateCopyRegion(decimal currentWidth, decimal currentHeight, decimal newWidth,
        decimal newHeight)
    {
        var ratioWidth = currentWidth / newWidth;
        var ratioHeight = currentHeight / newHeight;

        var x = 0.0m;
        var y = 0.0m;
        var width = currentWidth;
        var height = currentHeight;

        if (ratioHeight < 1)
        {
            ratioWidth /= ratioHeight;
            ratioHeight = 1;
        }

        if (ratioWidth < 1)
        {
            ratioHeight /= ratioWidth;
            ratioWidth = 1;
        }

        if (ratioWidth > 1 && ratioHeight > 1)
        {
            if (ratioWidth > ratioHeight)
            {
                ratioWidth /= ratioHeight;
                ratioHeight = 1;
            }
            else
            {
                ratioHeight /= ratioWidth;
                ratioWidth = 1;
            }
        }

        if (ratioWidth > ratioHeight)
        {
            var widthToTake = width / ratioWidth;
            x = width / 2 - widthToTake / 2;
            width = widthToTake;
        }
        else if (ratioHeight > ratioWidth)
        {
            var heightToTake = height / ratioHeight;
            y = height / 2 - heightToTake / 2;
            height = heightToTake;
        }

        return new Rectangle((int)Math.Round(x, 0), (int)Math.Round(y, 0), (int)Math.Round(width, 0),
            (int)Math.Round(height, 0));
    }
}
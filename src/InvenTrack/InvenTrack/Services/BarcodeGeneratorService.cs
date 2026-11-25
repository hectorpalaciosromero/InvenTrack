using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using ZXing;
using ZXing.Common;
using ZXing.Rendering;

namespace InvenTrack.Services;

public class BarcodeGeneratorService
{
    public BitmapSource? GenerateBarcode(string? content, int width = 300, int height = 70)
    {
        if (string.IsNullOrWhiteSpace(content)) return null;

        try
        {
            var writer = new BarcodeWriterPixelData
            {
                Format = BarcodeFormat.CODE_128,
                Options = new EncodingOptions
                {
                    Width = width,
                    Height = height,
                    Margin = 4,
                    PureBarcode = true
                }
            };

            var pixelData = writer.Write(content);
            return ToBitmapSource(pixelData);
        }
        catch
        {
            return null;
        }
    }

    public BitmapSource? GenerateQRCode(string? content, int size = 180)
    {
        if (string.IsNullOrWhiteSpace(content)) return null;

        try
        {
            var writer = new BarcodeWriterPixelData
            {
                Format = BarcodeFormat.QR_CODE,
                Options = new EncodingOptions
                {
                    Width = size,
                    Height = size,
                    Margin = 2
                }
            };

            var pixelData = writer.Write(content);
            return ToBitmapSource(pixelData);
        }
        catch
        {
            return null;
        }
    }

    private static BitmapSource ToBitmapSource(PixelData pixelData)
    {
        var bitmapSource = BitmapSource.Create(
            pixelData.Width,
            pixelData.Height,
            96, 96,
            PixelFormats.Bgra32,
            null,
            pixelData.Pixels,
            pixelData.Width * 4);

        bitmapSource.Freeze();
        return bitmapSource;
    }
}

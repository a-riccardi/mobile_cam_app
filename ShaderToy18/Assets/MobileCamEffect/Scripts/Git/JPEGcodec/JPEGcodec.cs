using System;
using System.Runtime.InteropServices;

public static class JPEGcodec
{
    #region interface

    public static void WriteJPEGfile(byte[] img, int width, int height, string filename, int quality)
    {
        _WriteJPEGfile(img, width, height, filename, quality);
    }

    #endregion

    #region method import

    [DllImport("JPEGcodec", EntryPoint = "write_JPEG_file")]
    private static extern void _WriteJPEGfile(byte[] img, int width, int height, string filename, int quality);

    #endregion
}

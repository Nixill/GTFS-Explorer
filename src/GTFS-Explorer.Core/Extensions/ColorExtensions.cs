using System.Drawing;
using Nixill.Utils;

namespace GTFS_Explorer.Core.Extensions
{
  public static class ColorExtensions
  {
    public static string ToRGBHexString(this Color color)
    {
      int rgb = color.ToArgb() & 0xFFFFFF;
      return ((rgb >= 0x100000) ? "" :
      (rgb >= 0x010000) ? "0" :
      (rgb >= 0x001000) ? "00" :
      (rgb >= 0x000100) ? "000" :
      (rgb >= 0x000010) ? "0000" : "00000")
      + NumberUtils.IntToString(rgb, 16);
    }

    // This is no substitute for a proper formatter but it works I guess
    public static string ToString(this Color color, string format)
    {
      return format.Replace("{R}", color.R.ToString())
        .Replace("{G}", color.G.ToString())
        .Replace("{B}", color.B.ToString())
        .Replace("{A}", color.A.ToString())
        .Replace("{Rf}", (color.R / 255.0).ToString())
        .Replace("{Gf}", (color.G / 255.0).ToString())
        .Replace("{Bf}", (color.B / 255.0).ToString())
        .Replace("{Af}", (color.A / 255.0).ToString())
        .Replace("{Rx}", NumberUtils.IntToString(color.R, 16))
        .Replace("{Gx}", NumberUtils.IntToString(color.G, 16))
        .Replace("{Bx}", NumberUtils.IntToString(color.B, 16))
        .Replace("{Ax}", NumberUtils.IntToString(color.A, 16))
        .Replace("{Rx2}", ((color.R < 16) ? "0" : "") + NumberUtils.IntToString(color.R, 16))
        .Replace("{Gx2}", ((color.G < 16) ? "0" : "") + NumberUtils.IntToString(color.G, 16))
        .Replace("{Bx2}", ((color.B < 16) ? "0" : "") + NumberUtils.IntToString(color.B, 16))
        .Replace("{Ax2}", ((color.A < 16) ? "0" : "") + NumberUtils.IntToString(color.A, 16));
    }
  }
}
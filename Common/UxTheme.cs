// UxTheme.cs

using System;
using System.Runtime.InteropServices;

//---------------------------------------------------------------------------//

public enum BP_BUFFERFORMAT : uint
{
    COMPATIBLEBITMAP,    // Compatible bitmap
    DIB,                 // Device-independent bitmap
    TOPDOWNDIB,          // Top-down device-independent bitmap
    TOPDOWNMONODIB       // Top-down monochrome device-independent bitmap
}

//---------------------------------------------------------------------------//

[Flags]
public enum BPPF : uint
{
    Erase     = 1 << 0,
    NoClip    = 1 << 1,
    NonClient = 1 << 2,
}

//---------------------------------------------------------------------------//

[Flags]
public enum DT : uint
{
    TOP        = 0,
    LEFT       = 0,
    CENTER     = 1 <<  0,
    RIGHT      = 1 <<  1,
    VCENTER    = 1 <<  2,
    BOTTOM     = 1 <<  3,
    WORDBREAK  = 1 <<  4,
    SINGLELINE = 1 <<  5,
    EXPANDTABS = 1 <<  6,
    TABSTOP    = 1 <<  7,
    NOCLIP     = 1 <<  8,
}

//---------------------------------------------------------------------------//

[Flags]
public enum DTT : int
{
    TEXTCOLOR    = 1 <<  0,
    BORDERCOLOR  = 1 <<  1,
    SHADOWCOLOR  = 1 <<  2,
    SHADOWTYPE   = 1 <<  3,
    SHADOWOFFSET = 1 <<  4,
    BORDERSIZE   = 1 <<  5,
    FONTPROP     = 1 <<  6,
    COLORPROP    = 1 <<  7,
    STATEID      = 1 <<  8,
    CALCRECT     = 1 <<  9,
    APPLYOVERLAY = 1 << 10,
    GLOWSIZE     = 1 << 11,
    CALLBACK     = 1 << 12,
    COMPOSITED   = 1 << 13,
}

//---------------------------------------------------------------------------//

[StructLayout(LayoutKind.Sequential)]
public struct BP_PAINTPARAMS
{
    public int    cbSize;
    public BPPF   dwFlags;
    public IntPtr prcExclude;
    public IntPtr pBlendFunction;
}

//---------------------------------------------------------------------------//

[StructLayout(LayoutKind.Sequential)]
public struct POINT
{
    public POINT(int x, int y)
    {
        this.x = x;
        this.y = y;
    }

    public int x;
    public int y;
}

//---------------------------------------------------------------------------//

[StructLayout(LayoutKind.Sequential)]
public struct RECT
{
    public int left, top, right, bottom;

    public RECT(int left, int top, int right, int bottom)
    {
        this.left  = left;  this.top    = top;
        this.right = right; this.bottom = bottom;
    }
}

//---------------------------------------------------------------------------//

[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
public struct LOGFONTW
{
    public int  lfHeigh;
    public int  lfWidth;
    public int  lfEscapement;
    public int  lfOrientation;
    public int  lfWeight;
    public byte lfItalic;
    public byte lfUnderline;
    public byte lfStrikeOut;
    public byte lfCharSet;
    public byte lfOutPrecision;
    public byte lfClipPrecision;
    public byte lfQuality;
    public byte lfPitchAndFamily;

    [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 32)]
    public string lfFaceName;
}

//---------------------------------------------------------------------------//

[StructLayout(LayoutKind.Sequential)]
public struct DTTOPTS
{
    public int    dwSize;
    public DTT    dwFlags;
    public uint   crText;
    public uint   crBorder;
    public uint   crShadow;
    public int    iTextShadowType;
    public POINT  ptShadowOffset;
    public int    iBorderSize;
    public int    iFontPropId;
    public int    iColorPropId;
    public int    iStateId;
    public bool   fApplyOverlay;
    public int    iGlowSize;
    public int     pfnDrawTextCallback;
    public IntPtr lParam;
}

//---------------------------------------------------------------------------//

internal class UxTheme
{
    [DllImport("uxtheme.dll")]
    public static extern int BufferedPaintInit();

    [DllImport("uxtheme.dll", SetLastError = true)]
    public static extern IntPtr BeginBufferedPaint
    (
        IntPtr hdcTarget, ref RECT prcTarget,
        BP_BUFFERFORMAT dwFormat, IntPtr pPaintParams,
        ref IntPtr phdc
    );

    [DllImport("uxtheme.dll")]
    public static extern int BufferedPaintSetAlpha
    (
        IntPtr hBufferedPaint, ref RECT prc, byte alpha
    );

    [DllImport("uxtheme.dll")]
    public static extern int EndBufferedPaint
    (
        IntPtr hBufferedPaint, bool fUpdateTarget
    );

    [DllImport("uxtheme.dll")]
    public static extern int BufferedPaintUnInit();


    [DllImport("uxtheme.dll", ExactSpelling = true, CharSet = CharSet.Unicode)]
    public static extern IntPtr OpenThemeData
    (
        IntPtr hWnd, String classList
    );

    [DllImport("uxtheme.dll")]
    public static extern int IsThemeActive();

    [DllImport("uxtheme.dll", ExactSpelling = true)]
    public static extern int CloseThemeData
    (
        IntPtr hTheme
    );

    [DllImport("uxtheme.dll", ExactSpelling=true)]
    public static extern int DrawThemeParentBackground
    (
        IntPtr hWnd, IntPtr hdc, ref RECT pRect
    );

    [DllImport("uxtheme.dll", ExactSpelling = true, CharSet = CharSet.Unicode)]
    public static extern void DrawThemeTextEx
    (
        IntPtr hTheme, IntPtr hdc, int iPartId, int iStateId,
        String pszText, int iCharCount, DT dwTextFlags,
        ref RECT pRect, ref DTTOPTS pOptions
    );
}

//---------------------------------------------------------------------------//

// UxTheme.cs
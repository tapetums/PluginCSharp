using System;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace PluginHost
{

    public partial class MainForm : Form
    {
        public static DTTOPTS dttopts = new DTTOPTS();

        public MainForm()
        {
            UxTheme.BufferedPaintInit();

            dttopts.dwSize    = Marshal.SizeOf(dttopts);
            dttopts.dwFlags   = DTT.COMPOSITED | DTT.TEXTCOLOR | DTT.GLOWSIZE;
            dttopts.crText    = 0xFF000000;
            dttopts.iGlowSize = 24;

            InitializeComponent();

            this.SetStyle(ControlStyles.ResizeRedraw, true);
            this.SetStyle(ControlStyles.DoubleBuffer, true);
            this.SetStyle(ControlStyles.UserPaint, true);
            this.SetStyle(ControlStyles.AllPaintingInWmPaint, true);

            if ( DwmApi.DwmIsCompositionEnabled() )
            {
                this.BackColor = Color.Black;
                var margin = new DwmApi.MARGINS(-1);
                DwmApi.DwmExtendFrameIntoClientArea(this.Handle, margin); 
            }
        }

        private void MainForm_Paint(object sender, PaintEventArgs e)
        {
            var g = e.Graphics;

            var hdc = g.GetHdc();
            {
                DrawText
                (
                    hdc, "qwertyuiopasdfghjklzxcvbnm",
                    0, 0, e.ClipRectangle.Width, e.ClipRectangle.Height
                );
            }
            g.ReleaseHdc(hdc);
        }

        private void DrawText
        (
            IntPtr hdc, string text,
            int x, int y, int w, int h
        )
        {
            RECT rc = new RECT(x, y, x + w, y + h);

            var hdc_mem = IntPtr.Zero;
            var pb = UxTheme.BeginBufferedPaint
            (
                hdc, ref rc, BP_BUFFERFORMAT.TOPDOWNDIB, IntPtr.Zero,
                ref hdc_mem
            );
            if ( IntPtr.Zero == pb )
            {
                return;
            }

            var hTheme = UxTheme.OpenThemeData(this.Handle, "WINDOW");
            if ( IntPtr.Zero != hTheme )
            {
                rc.right  += 4;
                rc.bottom += 2;
                dttopts.crText = 0xFF000000;
                dttopts.iGlowSize = 18;
                UxTheme.DrawThemeTextEx
                (
                    hTheme, hdc_mem, 0, 0,
                    text, -1,
                    DT.SINGLELINE | DT.CENTER | DT.VCENTER,
                    ref rc, ref dttopts
                );

                rc.right  -= 4;
                rc.bottom -= 2;
                dttopts.crText = 0xFFFFFFFF;
                dttopts.iGlowSize = 0;
                UxTheme.DrawThemeTextEx
                (
                    hTheme, hdc_mem, 0, 0,
                    text, -1,
                    DT.SINGLELINE | DT.CENTER | DT.VCENTER,
                    ref rc, ref dttopts
                );
            }
            UxTheme.CloseThemeData(hTheme);

            UxTheme.EndBufferedPaint(pb, true);
        }
    }
}

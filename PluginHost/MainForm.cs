using System;
using System.Drawing;
using System.Windows.Forms;

namespace PluginHost
{
    public partial class MainForm : Form
    {
        public static Image image = null;

        public MainForm()
        {
            InitializeComponent();

            this.SetStyle(ControlStyles.ResizeRedraw, true);
            this.SetStyle(ControlStyles.DoubleBuffer, true);
            this.SetStyle(ControlStyles.UserPaint, true);
            this.SetStyle(ControlStyles.AllPaintingInWmPaint, true);
        }

        private void MainForm_Paint(object sender, PaintEventArgs e)
        {
            if ( null == image )
            {
                return;
            }

            var g = e.Graphics;

            var sw = e.ClipRectangle.Width;
            var sh = e.ClipRectangle.Height;
            var bw = image.Width;
            var bh = image.Height;

            int x, y, w, h;
            if ( bw * sh < bh * sw )
            {
                w = bw * sh / bh;
                h = sh;
                x = (sw - w) / 2;
                y = 0;
            }
            else
            {
                w = sw;
                h = bh * sw / bw;
                x = 0;
                y = (sh - h) / 2;
            }

            g.DrawImage(image, x, y, w, h);
        }
    }
}

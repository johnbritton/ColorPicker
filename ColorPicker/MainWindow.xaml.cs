using Gma.System.MouseKeyHook;
using System;
using System.Drawing;
using System.Windows;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media.Imaging;

namespace ColorPicker
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        int previewWidth = 50;
        int previewHeight = 50;
        IKeyboardMouseEvents globalMouseHook;

        public MainWindow()
        {
            InitializeComponent();
            globalMouseHook = Hook.GlobalEvents();
        }

        private void MainWindow_MouseMove(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            //https://stackoverflow.com/questions/20338960/wpf-way-to-take-screenshots
            using (var screenBmp = new Bitmap(previewWidth, previewHeight, System.Drawing.Imaging.PixelFormat.Format32bppArgb))
            {
                using (var bmpGraphics = Graphics.FromImage(screenBmp))
                {
                    bmpGraphics.CopyFromScreen(e.Location.X - (previewWidth / 2), e.Location.Y - (previewHeight / 2), 0, 0, new System.Drawing.Size(previewWidth, previewHeight));
                    var bmpSource = Imaging.CreateBitmapSourceFromHBitmap(
                        screenBmp.GetHbitmap(),
                        IntPtr.Zero,
                        Int32Rect.Empty,
                        BitmapSizeOptions.FromEmptyOptions());
                    previewImg.Source = bmpSource;
                    var color = screenBmp.GetPixel(previewWidth / 2, previewWidth / 2);
                    setSelectedColor(new System.Windows.Media.Color() { R = color.R, G = color.G, B = color.B, A = color.A });
                }
            }

        }

        private void BeginCapture()
        {
            globalMouseHook = Hook.GlobalEvents();
            globalMouseHook.MouseMove += MainWindow_MouseMove;
            globalMouseHook.MouseUp += MainWindow_MouseUp;
            Mouse.OverrideCursor = Cursors.Cross;
        }

        private void EndCapture()
        {
            globalMouseHook.MouseMove -= MainWindow_MouseMove;
            globalMouseHook.MouseUp -= MainWindow_MouseUp;
            globalMouseHook.Dispose();
            Mouse.OverrideCursor = null;
        }

        private void MainWindow_MouseUp(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            EndCapture();
        }

        private void Button_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            BeginCapture();
        }

        private void setSelectedColor(System.Windows.Media.Color color)
        {
            colorCanvas.SelectedColor = color;
        }
    }
}

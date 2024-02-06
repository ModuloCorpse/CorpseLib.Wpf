using System.Windows.Media.Imaging;
using System.Windows.Threading;
using System.Windows;
using System.Drawing;

namespace CorpseLib.Wpf
{
    public class AnimatedImage : System.Windows.Controls.Image
    {
        private readonly BitmapSource[] m_BitmapSources = [];
        private int m_CurrentFrame = 0;

        public AnimatedImage(Image animatedBitmap, BitmapSource[] bitmapSources)
        {
            m_BitmapSources = bitmapSources;
            ImageAnimator.Animate(animatedBitmap, OnFrameChanged);
        }

        public AnimatedImage(Bitmap animatedBitmap)
        {
            int timeFrames = animatedBitmap.GetFrameCount(System.Drawing.Imaging.FrameDimension.Time);
            if (timeFrames > 0)
            {
                m_BitmapSources = new BitmapSource[timeFrames];
                for (int i = 0; i < timeFrames; ++i)
                {
                    animatedBitmap.SelectActiveFrame(System.Drawing.Imaging.FrameDimension.Time, i);
                    Bitmap bitmap = new(animatedBitmap);
                    bitmap.MakeTransparent();
                    m_BitmapSources[i] = System.Windows.Interop.Imaging.CreateBitmapSourceFromHBitmap(bitmap.GetHbitmap(), IntPtr.Zero, Int32Rect.Empty, BitmapSizeOptions.FromEmptyOptions());
                }
                ImageAnimator.Animate(animatedBitmap, OnFrameChanged);
            }
        }

        private void OnFrameChanged(object? o, EventArgs e) => Dispatcher.BeginInvoke(DispatcherPriority.Render, () =>
        {
            Source = m_BitmapSources[m_CurrentFrame++];
            m_CurrentFrame %= m_BitmapSources.Length;
            ImageAnimator.UpdateFrames();
        });
    }
}

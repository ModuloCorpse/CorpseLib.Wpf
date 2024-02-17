using CorpseLib.Network;
using CorpseLib.Placeholder;
using CorpseLib.Web;
using CorpseLib.Web.Http;
using System.Drawing;
using System.IO;
using System.Windows;
using System.Windows.Media.Imaging;
using Image = System.Windows.Controls.Image;

namespace CorpseLib.Wpf
{
    public static class ImageLoader
    {
        private static readonly Dictionary<string, BitmapSource> ms_LoadedStaticImage = [];
        private static readonly Dictionary<string, Tuple<System.Drawing.Image, BitmapSource[]>> ms_LoadedAnimatedImage = [];
        private static readonly Context ms_PlaceholderContext = new();

        static ImageLoader()
        {
            string strExeFilePath = System.Reflection.Assembly.GetExecutingAssembly().Location;
            ms_PlaceholderContext.AddVariable("ExePath", strExeFilePath);
            string? strWorkPath = System.IO.Path.GetDirectoryName(strExeFilePath);
            if (!string.IsNullOrEmpty(strWorkPath))
                ms_PlaceholderContext.AddVariable("ExeDir", strWorkPath);
        }

        private static System.Drawing.Image? LoadImage(string imageURL)
        {
            string url = Converter.Convert(imageURL, ms_PlaceholderContext);
            if (File.Exists(url))
                return System.Drawing.Image.FromFile(url);
            else
            {
                Response response = new URLRequest(URI.Parse(url)).Send();
                if (response.StatusCode == 200)
                {
                    MemoryStream stream = new(response.RawBody);
                    return System.Drawing.Image.FromStream(stream);
                }
            }
            return null;
        }

        public static Image? LoadStaticImage(string url)
        {
            if (ms_LoadedStaticImage.TryGetValue(url, out BitmapSource? source))
                return new Image() { Source = source };
            else
            {
                System.Drawing.Image? img = LoadImage(url);
                if (img != null)
                {
                    BitmapSource newSource = System.Windows.Interop.Imaging.CreateBitmapSourceFromHBitmap(((Bitmap)img).GetHbitmap(), IntPtr.Zero, Int32Rect.Empty, BitmapSizeOptions.FromEmptyOptions());
                    ms_LoadedStaticImage[url] = newSource;
                    return new Image() { Source = newSource };
                }
            }
            return null;
        }

        public static AnimatedImage? LoadAnimatedImage(string url)
        {
            if (ms_LoadedAnimatedImage.TryGetValue(url, out Tuple<System.Drawing.Image, BitmapSource[]>? source))
                return new AnimatedImage(source.Item1, source.Item2);
            else
            {
                System.Drawing.Image? img = LoadImage(url);
                if (img != null)
                {
                    Bitmap animatedBitmap = (Bitmap)img;
                    int timeFrames = animatedBitmap.GetFrameCount(System.Drawing.Imaging.FrameDimension.Time);
                    if (timeFrames > 0)
                    {
                        BitmapSource[] bitmapSources = new BitmapSource[timeFrames];
                        for (int i = 0; i < timeFrames; ++i)
                        {
                            animatedBitmap.SelectActiveFrame(System.Drawing.Imaging.FrameDimension.Time, i);
                            Bitmap bitmap = new(animatedBitmap);
                            bitmap.MakeTransparent();
                            bitmapSources[i] = System.Windows.Interop.Imaging.CreateBitmapSourceFromHBitmap(bitmap.GetHbitmap(), IntPtr.Zero, Int32Rect.Empty, BitmapSizeOptions.FromEmptyOptions());
                        }
                        Tuple<System.Drawing.Image, BitmapSource[]> newSource = new(img, bitmapSources);
                        ms_LoadedAnimatedImage[url] = newSource;
                        return new AnimatedImage(newSource.Item1, newSource.Item2);
                    }
                }
            }
            return null;
        }
    }
}

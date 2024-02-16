using CorpseLib.Network;
using CorpseLib.StructuredText;
using CorpseLib.Web;
using CorpseLib.Web.Http;
using System.Drawing;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media.Imaging;
using Image = System.Windows.Controls.Image;
using Section = CorpseLib.StructuredText.Section;

namespace CorpseLib.Wpf
{
    public class StructuredLabel : RichTextBox
    {
        private static readonly Dictionary<string, BitmapSource> ms_LoadedStaticImage = [];
        private static readonly Dictionary<string, Tuple<System.Drawing.Image, BitmapSource[]>> ms_LoadedAnimatedImage = [];
        private readonly List<SectionRun> m_Text = [];
        private readonly List<Image> m_Images = [];
        private double m_FontSize = 14;

        private static System.Drawing.Image? LoadImage(string url)
        {
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

        private Image? LoadStaticImage(string url)
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

        private AnimatedImage? LoadAnimatedImage(string url)
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

        public void SetText(Text text)
        {
            Paragraph paragraph = new()
            {
                Margin = new(0),
                Padding = new(0),
                TextIndent = 0
            };

            foreach (Section section in text)
            {
                if (section.SectionType == Section.Type.TEXT)
                {
                    SectionRun textInline = new(section);
                    paragraph.Inlines.Add(textInline);
                    m_Text.Add(textInline);
                }
                else
                {
                    Image? image = null;
                    if (section.SectionType == Section.Type.IMAGE)
                        image = LoadStaticImage(section.Content);
                    else if (section.SectionType == Section.Type.ANIMATED_IMAGE)
                        image = LoadAnimatedImage(section.Content);

                    if (image != null)
                    {
                        double ratioToFontSize = section.GetPropertiesOr("Ratio", 1.5);
                        double marginLeft = section.GetPropertiesOr("Margin-Left", 0.0);
                        double marginTop = section.GetPropertiesOr("Margin-Top", 0.0);
                        double marginRight = section.GetPropertiesOr("Margin-Right", 0.0);
                        double marginBottom = section.GetPropertiesOr("Margin-Bottom", 0.0);
                        image.MaxWidth = m_FontSize * ratioToFontSize;
                        image.MaxHeight = m_FontSize * ratioToFontSize;
                        image.Margin = new(marginLeft, marginTop, marginRight, marginBottom);
                        InlineUIContainer imageInline = new()
                        {
                            BaselineAlignment = BaselineAlignment.Center,
                            Child = image
                        };
                        paragraph.Inlines.Add(imageInline);
                        m_Images.Add(image);
                    }
                    else
                    {
                        SectionRun textInline = new(section);
                        paragraph.Inlines.Add(textInline);
                        m_Text.Add(textInline);
                    }
                }
            }

            Document.PagePadding = new(0);
            Document.Blocks.Clear();
            Document.Blocks.Add(paragraph);
        }

        public void SetFontSize(double fontSize)
        {
            m_FontSize = fontSize;
            foreach (SectionRun sectionRun in m_Text)
                sectionRun.SetFontSize(fontSize);
            foreach (Image image in m_Images)
            {
                image.Width = fontSize * 1.5;
                image.Height = fontSize * 1.5;
            }
        }
    }
}

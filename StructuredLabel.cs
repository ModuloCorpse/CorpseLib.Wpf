using CorpseLib.Network;
using CorpseLib.StructuredText;
using CorpseLib.Web;
using CorpseLib.Web.Http;
using System.IO;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media.Imaging;
using Section = CorpseLib.StructuredText.Section;

namespace CorpseLib.Wpf
{
    public class StructuredLabel : RichTextBox
    {
        private readonly List<SectionRun> m_Text = [];
        private readonly List<Image> m_Images = [];
        private double m_FontSize = 14;

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
                else if (section.SectionType == Section.Type.IMAGE)
                {
                    BitmapImage bitmap = new();
                    bitmap.BeginInit();
                    bitmap.UriSource = new Uri(section.Content, UriKind.Absolute);
                    bitmap.EndInit();
                    Image image = new()
                    {
                        Width = m_FontSize * 1.5,
                        Height = m_FontSize * 1.5,
                        Source = bitmap
                    };
                    InlineUIContainer imageInline = new()
                    {
                        BaselineAlignment = BaselineAlignment.Center,
                        Child = image
                    };
                    paragraph.Inlines.Add(imageInline);
                    m_Images.Add(image);
                }
                else if (section.SectionType == Section.Type.ANIMATED_IMAGE)
                {
                    System.Drawing.Image img;
                    if (File.Exists(section.Content))
                        img = System.Drawing.Image.FromFile(section.Content);
                    else
                    {
                        Response response = new URLRequest(URI.Parse(section.Content)).Send();
                        MemoryStream stream = new(response.RawBody);
                        img = System.Drawing.Image.FromStream(stream);
                    }
                    AnimatedImage animatedImage = new((System.Drawing.Bitmap)img)
                    {
                        Width = m_FontSize * 1.5,
                        Height = m_FontSize * 1.5
                    };
                    InlineUIContainer animatedImageInline = new()
                    {
                        BaselineAlignment = BaselineAlignment.Center,
                        Child = animatedImage
                    };
                    paragraph.Inlines.Add(animatedImageInline);
                    m_Images.Add(animatedImage);
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

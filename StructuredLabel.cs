using CorpseLib.StructuredText;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using Section = CorpseLib.StructuredText.Section;

namespace CorpseLib.Wpf
{
    public class StructuredLabel : RichTextBox
    {
        private readonly List<SectionRun> m_Text = [];
        private readonly List<System.Windows.Controls.Image> m_Images = [];
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
                else
                {
                    System.Windows.Controls.Image? image = null;
                    if (section.SectionType == Section.Type.IMAGE)
                        image = ImageLoader.LoadStaticImage(section.Content);
                    else if (section.SectionType == Section.Type.ANIMATED_IMAGE)
                        image = ImageLoader.LoadAnimatedImage(section.Content);

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
            foreach (System.Windows.Controls.Image image in m_Images)
            {
                image.Width = fontSize * 1.5;
                image.Height = fontSize * 1.5;
            }
        }
    }
}

using CorpseLib.StructuredText;
using System.Diagnostics.CodeAnalysis;
using System.Windows;
using System.Windows.Media;

namespace CorpseLib.Wpf
{
    public class SectionRun : System.Windows.Documents.Run
    {
        private readonly Dictionary<string, object> m_Style = [];

        public SectionRun(Section section)
        {
            Text = (section.SectionType == Section.Type.TEXT) ? section.Content : section.Alt;
            m_Style = section.Properties;

            if (section.TryGetProperties("FontSize", out double fontSize))
                FontSize = fontSize;

            BrushConverter converter = new();
            if (section.TryGetProperties("Color", out string? foreground))
            {
                Brush? foregroundBrush = (Brush?)converter.ConvertFrom(foreground);
                if (foregroundBrush != null)
                    Foreground = foregroundBrush;
            }
            if (section.TryGetProperties("BackgroundColor", out string? background))
            {
                Brush? backgroundBrush = (Brush?)converter.ConvertFrom(background);
                if (backgroundBrush != null)
                    Background = backgroundBrush;
            }

            if (section.TryGetProperties("Bold", out bool bold) && bold)
                FontWeight = FontWeights.Bold;
        }

        public void SetFontSize(double fontSize)
        {
            if (!m_Style.ContainsKey("FontSize"))
                FontSize = fontSize;
        }

        public bool TryGetStyle<T>(string key, [NotNullWhen(true)] out T? style)
        {
            if (m_Style.TryGetValue(key, out object? obj))
            {
                if (obj is T t)
                {
                    style = t;
                    return true;
                }
                else
                {

                    T? tmp = Helper.Cast<T>(obj);
                    if (tmp != null)
                    {
                        style = tmp;
                        return true;
                    }
                }
            }
            style = default;
            return false;
        }

        public bool TryGetStyle(string key, out object? style) => m_Style.TryGetValue(key, out style);
    }
}

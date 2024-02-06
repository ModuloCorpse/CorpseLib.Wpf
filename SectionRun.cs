using CorpseLib.StructuredText;
using System.Windows.Media;

namespace CorpseLib.Wpf
{
    public class SectionRun : System.Windows.Documents.Run
    {
        private readonly Dictionary<string, object?> m_Style = [];

        public SectionRun(Section section)
        {
            Text = (section.SectionType == Section.Type.TEXT) ? section.Content : section.Alt;
            m_Style = section.Properties;

            if (m_Style.TryGetValue("FontSize", out object? fontSize))
                FontSize = (double)fontSize!;

            if (m_Style.TryGetValue("Color", out object? foreground))
                Foreground = (SolidColorBrush)new BrushConverter().ConvertFrom((string)foreground!)!;
            if (m_Style.TryGetValue("BackgroundColor", out object? background))
                Background = (SolidColorBrush)new BrushConverter().ConvertFrom((string)background!)!;
        }

        public void SetFontSize(double fontSize)
        {
            if (!m_Style.ContainsKey("FontSize"))
                FontSize = fontSize;
        }

        public bool TryGetStyle<T>(string key, out T? style)
        {
            if (m_Style.TryGetValue(key, out object? ret))
            {
                if (ret != null)
                    style = Helper.Cast<T>(ret);
                else
                    style = default;
                return true;
            }

            style = default;
            return false;
        }

        public bool TryGetStyle(string key, out object? style) => m_Style.TryGetValue(key, out style);
    }
}

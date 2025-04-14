using System.Globalization;
using System.Text.RegularExpressions;
using System.Windows.Media;

namespace CorpseLib.Wpf
{
    public readonly partial struct ColorCode
    {
        [GeneratedRegex(@"^[0-9A-F]{6}([0-9A-F]{2})?$", RegexOptions.IgnoreCase)]
        private static partial Regex GenerateHexRegex();
        private static readonly Regex HexPattern = GenerateHexRegex();

        private readonly byte m_R = 255;
        private readonly byte m_G = 255;
        private readonly byte m_B = 255;
        private readonly byte m_A = 255;

        public readonly byte R => m_R;
        public readonly byte G => m_G;
        public readonly byte B => m_B;
        public readonly byte A => m_A;

        public ColorCode(byte r, byte g, byte b, byte a = 255)
        {
            m_R = r;
            m_G = g;
            m_B = b;
            m_A = a;
        }

        public ColorCode(string hex)
        {
            hex = hex.TrimStart('#');
            if (!HexPattern.IsMatch(hex))
                throw new ArgumentException("Hex code is invalid.");
            m_R = byte.Parse(hex[0..2], NumberStyles.HexNumber);
            m_G = byte.Parse(hex[2..4], NumberStyles.HexNumber);
            m_B = byte.Parse(hex[4..6], NumberStyles.HexNumber);
            m_A = (hex.Length == 8) ? byte.Parse(hex[6..8], NumberStyles.HexNumber) : (byte)255;
        }

        public readonly Brush ToBrush() => new SolidColorBrush(Color.FromArgb(m_A, m_R, m_G, m_B));
        public readonly override string ToString() => $"#{m_R:X2}{m_G:X2}{m_B:X2}{m_A:X2}";
    }
}

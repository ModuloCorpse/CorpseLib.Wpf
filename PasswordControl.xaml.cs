using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;

namespace CorpseLib.Wpf
{
    /// <summary>
    /// Interaction logic for PasswordControl.xaml
    /// </summary>
    public partial class PasswordControl : UserControl
    {
        #region IsHidden
        public static readonly DependencyProperty IsHiddenProperty = Helper.NewProperty("IsHidden", true, (PasswordControl instance, bool newValue, bool oldValue) => instance.Property_SetIsHidden(newValue, oldValue));
        [Description("Specify if the password is hidden"), Category("Common Properties")]
        public bool IsHidden { get => (bool)GetValue(IsHiddenProperty); set => SetValue(IsHiddenProperty, value); }
        internal void Property_SetIsHidden(bool newValue, bool oldValue)
        {
            if (newValue && !oldValue)
            {
                PasswordBox.HideChars = newValue;
                if (!string.IsNullOrWhiteSpace(HiddenImageSource))
                    ShowPassword.Content = new Image { SourcePath = HiddenImageSource };
            }
            else if (!newValue && oldValue)
            {
                PasswordBox.HideChars = newValue;
                if (!string.IsNullOrWhiteSpace(ShownImageSource))
                    ShowPassword.Content = new Image { SourcePath = ShownImageSource };
            }
        }
        #endregion IsHidden

        #region HiddenImageSource
        public static readonly DependencyProperty HiddenImageSourceProperty = Helper.NewProperty("HiddenImageSource", string.Empty, (PasswordControl instance, string value) => instance.Property_SetHiddenImageSource(value));
        [Description("Image of the button when password is hidden"), Category("Common Properties")]
        public string HiddenImageSource { get => (string)GetValue(HiddenImageSourceProperty); set => SetValue(HiddenImageSourceProperty, value); }
        internal void Property_SetHiddenImageSource(string source)
        {
            if (!string.IsNullOrWhiteSpace(source) && IsHidden)
                ShowPassword.Content = new Image { SourcePath = source };
        }
        #endregion HiddenImageSource

        #region ShownImageSource
        public static readonly DependencyProperty ShownImageSourceProperty = Helper.NewProperty("ShownImageSource", string.Empty, (PasswordControl instance, string value) => instance.Property_SetShownImageSource(value));
        [Description("Image of the button when password is shown"), Category("Common Properties")]
        public string ShownImageSource { get => (string)GetValue(ShownImageSourceProperty); set => SetValue(ShownImageSourceProperty, value); }
        internal void Property_SetShownImageSource(string source)
        {
            if (!string.IsNullOrWhiteSpace(source) && !IsHidden)
                ShowPassword.Content = new Image { SourcePath = source };
        }
        #endregion ShownImageSource

        public PasswordControl()
        {
            InitializeComponent();
        }

        private void ShowPassword_Click(object sender, RoutedEventArgs e) => IsHidden = !IsHidden;
    }
}

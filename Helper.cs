using System.Windows;

namespace CorpseLib.Wpf
{
    public static class Helper
    {
        public delegate void PropertyChangedDelegate<TUIElement, TPropertyType>(TUIElement instance, TPropertyType value);
        public delegate void OldPropertyChangedDelegate<TUIElement, TPropertyType>(TUIElement instance, TPropertyType newValue, TPropertyType oldValue);
        public static DependencyProperty NewProperty<TUIElement, TPropertyType>(string name, TPropertyType defaultValue, PropertyChangedDelegate<TUIElement, TPropertyType> callback) where TUIElement : DependencyObject => DependencyProperty.Register(name, typeof(TPropertyType), typeof(TUIElement), new PropertyMetadata(defaultValue, new PropertyChangedCallback((DependencyObject d, DependencyPropertyChangedEventArgs e) => callback((TUIElement)d, (TPropertyType)e.NewValue))));
        public static DependencyProperty NewProperty<TUIElement, TPropertyType>(string name, TPropertyType defaultValue, OldPropertyChangedDelegate<TUIElement, TPropertyType> callback) where TUIElement : DependencyObject => DependencyProperty.Register(name, typeof(TPropertyType), typeof(TUIElement), new PropertyMetadata(defaultValue, new PropertyChangedCallback((DependencyObject d, DependencyPropertyChangedEventArgs e) => callback((TUIElement)d, (TPropertyType)e.NewValue, (TPropertyType)e.OldValue))));
        public static DependencyProperty NewProperty<TUIElement, TPropertyType>(string name, TPropertyType defaultValue) where TUIElement : DependencyObject => DependencyProperty.Register(name, typeof(TPropertyType), typeof(TUIElement), new PropertyMetadata(defaultValue));
    }
}

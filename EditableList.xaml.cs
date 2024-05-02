using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;

namespace CorpseLib.Wpf
{
    /// <summary>
    /// Interaction logic for EditableList.xaml
    /// </summary>
    public partial class EditableList : UserControl
    {
        public delegate string ConversionDelegate(object obj);

        private class ItemWrapper(object item, ConversionDelegate? conversionDelegate)
        {
            private readonly object m_Item = item;
            private readonly ConversionDelegate? m_ConversionDelegate = conversionDelegate;

            public object Item => m_Item;

            public override bool Equals(object? obj) => obj is ItemWrapper other ? m_Item!.Equals(other.m_Item) : m_Item!.Equals(obj);
            public override int GetHashCode() => m_Item!.GetHashCode();
            public override string ToString() => m_ConversionDelegate != null ? m_ConversionDelegate(m_Item) : m_Item!.ToString()!;
        }

        #region IsSearchable
        public static readonly DependencyProperty IsSearchableProperty = Helper.NewProperty("IsSearchable", true, (EditableList instance, bool value) => instance.Property_SetIsSearchable(value));
        [Description("Specify if user can search element"), Category("Common Properties")]
        public bool IsSearchable { get => (bool)GetValue(IsSearchableProperty); set => SetValue(IsSearchableProperty, value); }
        internal void Property_SetIsSearchable(bool value)
        {
            if (value)
                SearchPanel.Visibility = Visibility.Visible;
            else
                SearchPanel.Visibility = Visibility.Collapsed;
        }
        #endregion IsSearchable

        #region IsAutoSearch
        public static readonly DependencyProperty IsAutoSearchProperty = Helper.NewProperty("IsAutoSearch", true, (EditableList instance, bool value) => instance.Property_SetIsAutoSearch(value));
        [Description("Specify if search update when entering text"), Category("Common Properties")]
        public bool IsAutoSearch { get => (bool)GetValue(IsAutoSearchProperty); set => SetValue(IsAutoSearchProperty, value); }
        internal void Property_SetIsAutoSearch(bool value) => SearchButton.Visibility = (value) ? Visibility.Collapsed : Visibility.Visible;
        #endregion IsAutoSearch

        #region IsEditOnly
        public static readonly DependencyProperty IsEditOnlyProperty = Helper.NewProperty("IsEditOnly", false, (EditableList instance, bool value) => instance.Property_SetIsEditOnly(value));
        [Description("Specify if user can add/remove element"), Category("Common Properties")]
        public bool IsEditOnly { get => (bool)GetValue(IsEditOnlyProperty); set => SetValue(IsEditOnlyProperty, value); }
        internal void Property_SetIsEditOnly(bool value)
        {
            if (value)
            {
                AddButton.Visibility = Visibility.Collapsed;
                RemoveButton.Visibility = Visibility.Collapsed;
            }
            else
            {
                AddButton.Visibility = Visibility.Visible;
                RemoveButton.Visibility = Visibility.Visible;
            }
        }
        #endregion IsEditOnly

        #region IsAddRemoveOnly
        public static readonly DependencyProperty IsAddRemoveOnlyProperty = Helper.NewProperty("IsAddRemoveOnly", false, (EditableList instance, bool value) => instance.Property_SetIsAddRemoveOnly(value));
        [Description("Specify if user can edit element"), Category("Common Properties")]
        public bool IsAddRemoveOnly { get => (bool)GetValue(IsAddRemoveOnlyProperty); set => SetValue(IsAddRemoveOnlyProperty, value); }
        internal void Property_SetIsAddRemoveOnly(bool value) => EditButton.Visibility = (value) ? Visibility.Collapsed : Visibility.Visible;
        #endregion IsAddRemoveOnly

        #region SearchImageSource
        public static readonly DependencyProperty SearchImageSourceProperty = Helper.NewProperty("SearchImageSource", string.Empty, (EditableList instance, string value) => instance.Property_SetSearchImageSource(value));
        [Description("Image of the search button"), Category("Common Properties")]
        public string SearchImageSource { get => (string)GetValue(SearchImageSourceProperty); set => SetValue(SearchImageSourceProperty, value); }
        internal void Property_SetSearchImageSource(string source)
        {
            if (!string.IsNullOrWhiteSpace(source))
            {
                SearchButton.Content = new Image
                {
                    SourcePath = source,
                    Height = 20,
                    Width = 20
                };
            }
        }
        #endregion SearchImageSource

        #region AddImageSource
        public static readonly DependencyProperty AddImageSourceProperty = Helper.NewProperty("AddImageSource", string.Empty, (EditableList instance, string value) => instance.Property_SetAddImageSource(value));
        [Description("Image of the add button"), Category("Common Properties")]
        public string AddImageSource { get => (string)GetValue(AddImageSourceProperty); set => SetValue(AddImageSourceProperty, value); }
        internal void Property_SetAddImageSource(string source)
        {
            if (!string.IsNullOrWhiteSpace(source))
            {
                AddButton.Content = new Image
                {
                    SourcePath = source,
                    Height = 20,
                    Width = 20
                };
            }
        }
        #endregion AddImageSource

        #region RemoveImageSource
        public static readonly DependencyProperty RemoveImageSourceProperty = Helper.NewProperty("RemoveImageSource", string.Empty, (EditableList instance, string value) => instance.Property_SetRemoveImageSource(value));
        [Description("Image of the remove button"), Category("Common Properties")]
        public string RemoveImageSource { get => (string)GetValue(RemoveImageSourceProperty); set => SetValue(RemoveImageSourceProperty, value); }
        internal void Property_SetRemoveImageSource(string source)
        {
            if (!string.IsNullOrWhiteSpace(source))
            {
                RemoveButton.Content = new Image
                {
                    SourcePath = source,
                    Height = 20,
                    Width = 20
                };
            }
        }
        #endregion RemoveImageSource

        #region EditImageSource
        public static readonly DependencyProperty EditImageSourceProperty = Helper.NewProperty("EditImageSource", string.Empty, (EditableList instance, string value) => instance.Property_SetEditImageSource(value));
        [Description("Image of the edit button"), Category("Common Properties")]
        public string EditImageSource { get => (string)GetValue(EditImageSourceProperty); set => SetValue(EditImageSourceProperty, value); }
        internal void Property_SetEditImageSource(string source)
        {
            if (!string.IsNullOrWhiteSpace(source))
            {
                EditButton.Content = new Image
                {
                    SourcePath = source,
                    Height = 20,
                    Width = 20
                };
            }
        }
        #endregion EditImageSource

        private readonly DictionaryTree<ItemWrapper> m_SearchTree = new();
        private ConversionDelegate? m_ConversionDelegate = null;
        private string m_Search = string.Empty;

        public event EventHandler? ItemAdded;
        public event EventHandler<object>? ItemRemoved;
        public event EventHandler<object>? ItemEdited;

        public EditableList()
        {
            InitializeComponent();
        }

        private void SearchTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (IsAutoSearch)
            {
                m_Search = SearchTextBox.Text;
                UpdateList();
            }
        }

        public void SetConversionDelegate(ConversionDelegate conversionDelegate) => m_ConversionDelegate = conversionDelegate;

        private void UpdateList()
        {
            ObjectListBox.Items.Clear();
            List<ItemWrapper> list = m_SearchTree.Search(m_Search);
            foreach (ItemWrapper item in list)
                ObjectListBox.Items.Add(item);
            ObjectListBox.Items.Refresh();
        }

        public void UpdateObject(object oldObj, object newObj)
        {
            for (int i = 0; i != ObjectListBox.Items.Count; ++i)
            {
                ItemWrapper? oldWrapper = (ItemWrapper?)ObjectListBox.Items[i];
                if (oldWrapper != null && oldWrapper.Equals(oldObj))
                {
                    m_SearchTree.Remove(oldWrapper.ToString());
                    AddObject(newObj);
                    return;
                }
            }
        }

        public void AddObjects(IEnumerable<object> items)
        {
            foreach (object item in items)
            {
                ItemWrapper itemWrapper = new(item, m_ConversionDelegate);
                m_SearchTree.Add(itemWrapper.ToString(), itemWrapper);
            }
            UpdateList();
        }

        public void AddObject(object obj)
        {
            ItemWrapper itemWrapper = new(obj, m_ConversionDelegate);
            m_SearchTree.Add(itemWrapper.ToString(), itemWrapper);
            UpdateList();
        }

        public void Clear()
        {
            m_SearchTree.Clear();
            UpdateList();
        }

        private void AddButton_Click(object sender, RoutedEventArgs e) => ItemAdded?.Invoke(this, EventArgs.Empty);

        private void RemoveButton_Click(object sender, RoutedEventArgs e)
        {
            ItemWrapper? item = (ItemWrapper?)ObjectListBox.SelectedItem;
            if (item != null)
            {
                m_SearchTree.Remove(item.ToString());
                UpdateList();
                ItemRemoved?.Invoke(this, item.Item);
            }
        }

        private void EditButton_Click(object sender, RoutedEventArgs e)
        {
            ItemWrapper? item = (ItemWrapper?)ObjectListBox.SelectedItem;
            if (item != null)
                ItemEdited?.Invoke(this, item.Item);
        }

        private void SearchButton_Click(object sender, RoutedEventArgs e)
        {
            m_Search = SearchTextBox.Text;
            UpdateList();
        }

        public List<object> GetItems()
        {
            List<object> ret = [];
            foreach (var item in ObjectListBox.Items)
            {
                if (item != null && item is ItemWrapper wrapper)
                    ret.Add(wrapper.Item);
            }
            return ret;
        }
    }
}

using System.Windows;
using System.Windows.Controls;

namespace TestTask2
{
    /// <summary>
    /// Interaction logic for AddWindow.xaml
    /// </summary>
    public partial class AddWindow
    {
        private bool _name ;
        private readonly string [] _typeStrings = { "Bool", "Int", "Double", "None" } ;
        public AddWindow()
        {
            InitializeComponent();
            foreach ( var item in _typeStrings )
                {
                    TypeBox.Items.Add ( item ) ;
                }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            MainWindow.NewTag = new TagItem ( NameBox.Text, TypeBox.SelectedItem.ToString() ) ;
            Close();
        }

        private void NameTextBoxTextChanged(object sender, TextChangedEventArgs e)
        {
            var temp = ( TextBox ) sender ;
            _name = temp.Text != "" ;
            if ( _name )
            {
                TypeBox.IsEnabled = true ;           
            }
        }

        private void TypeBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (_name)
            {
                AddButton.IsEnabled = true;
            }
        }
    }
}

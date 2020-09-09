using System ;
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
        }

        private void Add_Click(object sender, RoutedEventArgs e)
        {
            CheckBox temp = NoneCheck ;
            foreach ( CheckBox box in CheckBoxes.Children )
            {
                if ( box.IsChecked == true )
                {
                    temp = box ;
                }
            }
            MainWindow.NewTag = new TagItem ( NameBox.Text, temp.Content.ToString() ) ;
            Close();
        }

        private void NameTextBoxTextChanged(object sender, TextChangedEventArgs e)
        {
           
            var temp = ( TextBox ) sender ;

            _name = temp.Text.Length > 0 ;
            if ( _name )
                foreach ( CheckBox box in CheckBoxes.Children )
                {
                    box.IsEnabled = true ;
                }
            else
            {
                foreach (CheckBox box in CheckBoxes.Children)
                {
                    box.IsEnabled = false;
                }
            }
        }




        private void Check ( object sender, RoutedEventArgs e )
        {
            if ( _name )
            {
                foreach ( CheckBox box in CheckBoxes.Children )
                {
                    if ( !Equals ( box, sender ) )
                    {
                        box.IsChecked = false ;
                    }   
                }
                AddButton.IsEnabled = true ;
            }
        }

        private void Uncheck ( object sender, RoutedEventArgs e )
        {
            AddButton.IsEnabled = false;
        }

        private void Cancel_Click ( object sender, RoutedEventArgs e )
        {
            Close();
        }
    }
}

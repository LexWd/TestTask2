using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using TestTask1 ;

namespace TestTask2
{
    /// <summary>
    /// Interaction logic for AddWindow.xaml
    /// </summary>
    public partial class AddWindow : Window
    {
        private bool name ;
        private bool type ;
        private string [] typeStrings = new [] { "Bool", "Int", "Double", "None" } ;
        public AddWindow()
        {
            InitializeComponent();
            foreach ( var item in typeStrings )
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
            name = temp.Text != "" ;
            if ( name )
            {
                TypeBox.IsEnabled = true ;           
            }
        }

        private void TypeBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var temp = (ListBox)sender;
            type = temp.SelectedItem != null ;
            if (name)
            {
                AddButton.IsEnabled = true;
            }
        }
    }
}

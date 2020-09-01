using System.Windows;

namespace TestTask2
{
    /// <summary>
    /// Interaction logic for EditWindow.xaml
    /// </summary>
    public partial class EditWindow
    {
        public EditWindow()
        {
            InitializeComponent();
        }

        private void RenameButtonClick ( object sender, RoutedEventArgs e )
        {
            MainWindow.NewName = NewName.Text ;
            Close();
        }

        private void CancelClick ( object sender, RoutedEventArgs e )
        {
            Close();
        }
    }
}

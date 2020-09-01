using System;
using System.Diagnostics.CodeAnalysis ;
using System.Threading;
using System.Windows;
using Microsoft.Win32;

namespace TestTask2
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    [SuppressMessage ( "ReSharper", "SwitchStatementMissingSomeCases" )]
    public partial class MainWindow
    {
        private readonly TagStorage _storage;
        public static string NewName ;
        public static TagItem NewTag ;
        public MainWindow()
        {
            InitializeComponent();
            _storage = new TagStorage();
        }



        private void MainWindow_OnLoaded(object sender, RoutedEventArgs e)
        {
            _storage.TagRoot.AddChild(new TagItem("Dummy",_storage.TagRoot));
            TagView.ItemsSource = _storage.CreateDataTree();
        }



        private void LoadFileButtonClick(object sender, RoutedEventArgs e)
        {
            LoadFile();
        }

        private void LoadFile()
        {
            var thread = new Thread(Start);
            thread.Start();
            thread.Join();
            TagView.ItemsSource = _storage.CreateDataTree();
        }

        private void Start()
        {
            var openFile = new OpenFileDialog
            {
                InitialDirectory = "c:\\",
                Filter = "xml files (*.xml)|*.xml|All files (*.*)|*.*",
                FilterIndex = 1,
                RestoreDirectory = false

            };
            if (openFile.ShowDialog() != true) return;
            var filePath = openFile.FileName;
            _storage.Load(filePath);

        }


        private void SaveFileButtonClick(object sender, RoutedEventArgs e)
        {
            SaveFile();
        }

        private void SaveFile()
        {
            var thread = new Thread(Save);  
            var tItemModel = (TagItemModel)TagView.Items[0];
            _storage.TagRoot = new TagItem("Root");
            _storage.TagRoot.AddChild(tItemModel.Transform());
            thread.Start();
            thread.Join();
        }

        private void Save()
        {
            var saveFileDialog = new SaveFileDialog
            {
                InitialDirectory = "c:\\",
                Filter = "xml files (*.xml)|*.xml|All files (*.*)|*.*",
                FilterIndex = 1,
                RestoreDirectory = false
            };

            if (saveFileDialog.ShowDialog() != true) return;
            var filePath = saveFileDialog.FileName;

            _storage.Save(_storage.TagRoot, null, filePath);
        }

        private void MainWindow_OnClosed(object sender, EventArgs e)
        {
            var result = MessageBox.Show("Do you want to save?", "Save me", MessageBoxButton.YesNo);
            switch ( result )
            {
                case MessageBoxResult.Yes :
                    SaveFile () ;
                    break ;
                case MessageBoxResult.No :
                    break ;
                default :
                    throw new ArgumentOutOfRangeException () ;
            }
        }

        private void TagEdit ( object sender, RoutedEventArgs e )
        {
            var item = (TagItemModel) TagView.SelectedItem;
            NewName = item.TagName ;
            var tag = item.TagParent.FindChild ( NewName ) ;
            var temp = new EditWindow () ;
            temp.ShowDialog () ;
            tag.ReName(NewName);
            UpdateTree();
        }

        private void AddTag ( object sender, RoutedEventArgs e )
        {
            var item = (TagItemModel)TagView.SelectedItem;
            var tag = item?.TagParent.FindChild(item.TagName);
            var temp = new AddWindow();
            temp.ShowDialog () ;
            if ( NewTag != null )
            {
                tag?.AddChild(NewTag);
                NewTag = null ;
            }
            UpdateTree();
        }

        private void DeleteTag ( object sender, RoutedEventArgs e )
        {

           var temp =(TagItemModel) TagView.SelectedItem ;
           var result = MessageBox.Show ( "You sure that want to delete this?", "Deleting", MessageBoxButton.YesNo ) ;
            switch ( result )
            {
                case MessageBoxResult.Yes :
                     _storage.DeleteByModel(temp);
                      UpdateTree();
                    break ;
                case MessageBoxResult.No :
                    break ;
                default :
                    throw new ArgumentOutOfRangeException () ;
            }
               
        }

        public  void UpdateTree ()
        {
            TagView.ItemsSource = _storage.CreateDataTree();
        }
    }


}

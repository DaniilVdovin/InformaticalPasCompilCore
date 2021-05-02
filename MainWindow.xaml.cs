using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Documents;

namespace Informatical
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>

    public partial class MainWindow : Window
    {


        Core core;
        public MainWindow()
        {
            InitializeComponent();

            core = new Core();
            core.ThresholdReached += ((s, e) => {
                t_output.Content = "";
                t_output.Content = (core.LOG);
            });

        }
        private void ImagePanel_Drop(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);
                core.CompileFilePascal(files[0]);
               
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            core.printModules();
            core.TestWithModyle(core.models[0], 2);
        }
    }
}

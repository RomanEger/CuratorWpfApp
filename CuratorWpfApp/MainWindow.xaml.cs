using System.Text;
using System.Text.Json;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using CuratorWpfApp.Pages;
using CuratorWpfApp.Services;

namespace CuratorWpfApp
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            ResizeMode = ResizeMode.NoResize;

            MyFrame.frame = frameMain;

            MyFrame.frame.Navigate(new LoginPage());

        }

    }
}
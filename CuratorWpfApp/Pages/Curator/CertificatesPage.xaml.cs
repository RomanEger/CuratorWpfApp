using CuratorWpfApp.Services;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace CuratorWpfApp.Pages.Curator
{
    /// <summary>
    /// Логика взаимодействия для CertificatesPage.xaml
    /// </summary>
    public partial class CertificatesPage : Page
    {
        public CertificatesPage()
        {
            InitializeComponent();
        }

        private void btnBack_Click(object sender, RoutedEventArgs e)
        {
            MyFrame.frame.GoBack();
        }
    }
}

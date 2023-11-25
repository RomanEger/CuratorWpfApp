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

namespace CuratorWpfApp
{
    /// <summary>
    /// Логика взаимодействия для CuratorWindow.xaml
    /// </summary>
    public partial class CuratorWindow : Window
    {
        public CuratorWindow()
        {
            InitializeComponent();
        }

        private void Window_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            double x = (e.NewSize.Height-450)/50;
            spLeft.Width = 150 + x*5;
            btnReport.Margin = new Thickness(15, 15+x, 15, 15);
            frameMain.Width = e.NewSize.Width - spLeft.Width;
        }
    }
}

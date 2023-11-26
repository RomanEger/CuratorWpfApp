using CuratorWpfApp.Pages.Curator;
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
using System.Windows.Shapes;

namespace CuratorWpfApp
{
    /// <summary>
    /// Логика взаимодействия для CuratorWindow.xaml
    /// </summary>
    public partial class CuratorWindow : Window
    {
        private string groupName;
        public CuratorWindow(string GroupName)
        {
            InitializeComponent();
            groupName = GroupName;
        }

        private void Window_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            double x = (e.NewSize.Height-450)/50;
            spLeft.Width = 150 + x*5;
            //btnReport.Margin = new Thickness(15, 15+x, 15, 15);
            frameMain.Width = e.NewSize.Width - spLeft.Width;
            Style style = new Style()
            {
                TargetType = typeof(StackPanel)
            };
            style.BasedOn = (Style)Application.Current.Resources["spHeaderStyleBase"];
            style.Setters.Add(new Setter(WidthProperty, 125+x*5));
            Application.Current.Resources["spHeaderStyle"] = style;

            Style style1 = new Style()
            {
                TargetType = typeof(StackPanel)
            };
            style1.BasedOn = (Style)Application.Current.Resources["spHeaderStyleBase"];
            style1.Setters.Add(new Setter(WidthProperty, 125 + x * 5));
            Application.Current.Resources["spHeaderStyle"] = style1;
        }

        private void btnStudentList_Click(object sender, RoutedEventArgs e)
        {
            MyFrame.frame.Navigate(new StudentsListPage(groupName));
        }
    }
}

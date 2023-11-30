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
        navigateDelegate<StudentsListPage> navigate;

        public CuratorWindow(string GroupName)
        {
            InitializeComponent();

            groupName = GroupName;

            MyFrame.frame = frameMain;

            navigate = Navigate;

            navigate?.Invoke(new StudentsListPage(groupName));
        }


        private void btnStudentList_Click(object sender, RoutedEventArgs e)
        {
            if (navigate == null)
                navigate = Navigate;
            navigate?.Invoke(new StudentsListPage(groupName));
        }

        delegate bool navigateDelegate<T>(T page) where T: Page;

        public bool Navigate<T>(T page) where T : Page
        {
            return MyFrame.frame.Navigate(page);
        }
    }
}

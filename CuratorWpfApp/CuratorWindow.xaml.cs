using CuratorWpfApp.Models.ServicesDB;
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
        public bool Navigate<T>(T page) where T : Page
        {
            return MyFrame.frame.Navigate(page);
        }

        private string groupName;

        public CuratorWindow(string GroupName)
        {
            InitializeComponent();

            groupName = GroupName;

            MyFrame.frame = frameMain;

            Navigate(new StudentsListPage(groupName));
        }


        private void btnStudentList_Click(object sender, RoutedEventArgs e)
        {
            Navigate(new StudentsListPage(groupName));
        }


        private async void btnStudentProfile_Click(object sender, RoutedEventArgs e)
        {
            SqlQueryService sqlService = new SqlQueryService();
            Navigate(new StudentProfilePage(await sqlService.GetStudentsByGroupAsync(groupName)));
        }

        private void btnExitAcc_Click(object sender, RoutedEventArgs e)
        {
            if(MessageBox.Show(
                "Вы уверены, что хотите выйти из аккаунта?", 
                "Выход",
                MessageBoxButton.YesNo,
                MessageBoxImage.Question) == MessageBoxResult.Yes)
            {
                MainWindow mainWindow = new MainWindow();   
                mainWindow.Show();
                Close();
            }
        }

        private void btnCertificates_Click(object sender, RoutedEventArgs e)
        {
            Navigate(new CertificatesPage(groupName));
        }
    }
}

using CuratorWpfApp.Models.Enitities;
using CuratorWpfApp.Models.ServicesDB;
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
        SqlQueryService sqlService = new SqlQueryService();
        string groupName;
        List<CertificatesTable> certificatesList;
        public CertificatesPage(string groupName)
        {
            InitializeComponent();
            this.groupName = groupName;
            //FillingPage();
        }

        public async void FillingPage()
        {
            try
            {
                var l = await sqlService.GetCertificatesByGroupAsync(groupName);
                foreach (var item in l)
                {
                    item.Start_date = item.Start_date?.Replace(" 00:00:00", "");
                    item.End_date = item.End_date?.Replace(" 00:00:00", "");
                }
                dgCertificates.ItemsSource = l;
                certificatesList = l.ToList();
            }
            catch
            {
                MessageBox.Show("Не удалось получить список справок");
            }
        }
        private void btnBack_Click(object sender, RoutedEventArgs e)
        {
            MyFrame.frame.GoBack();
        }

        private async void btnDelCertificate_Click(object sender, RoutedEventArgs e)
        {
            if (MessageBox.Show("Вы уверены, что хотите удалить студента из списка?", "Удаление", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
            {
                await sqlService.DeleteCertificateAsync(GetId());
                FillingPage();
            }
        }

        private async void btnUpdCertificate_Click(object sender, RoutedEventArgs e)
        {
            var certificate = await sqlService.GetCertificateByIdAsync(GetId());
            MyFrame.frame.Navigate(new AddOrUpdateCertificatePage(certificate, groupName));
        }

        private int GetId()
        {
            var index = dgCertificates.SelectedIndex;
            var info = certificatesList[index];
            int id = info.Id;
            return id;
        }

        private void btnAddCertificate_Click(object sender, RoutedEventArgs e)
        {
            MyFrame.frame.Navigate(new AddOrUpdateCertificatePage(groupName));
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            FillingPage();
        }
    }
}

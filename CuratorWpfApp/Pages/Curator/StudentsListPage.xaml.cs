using CuratorWpfApp.Models.Enitities;
using CuratorWpfApp.Models.ServicesDB;
using CuratorWpfApp.Services;
using System;
using System.Collections;
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
    /// Логика взаимодействия для StudentsListPage.xaml
    /// </summary>
    public partial class StudentsListPage : Page
    {
        string groupName;
        SqlQueryService sqlService = new SqlQueryService();
        List<Students> listStudents;
        public StudentsListPage(string GroupName)
        {
            InitializeComponent();
            groupName = GroupName;
            GetStudentsAsync();
        }

        public async void GetStudentsAsync()
        {
            var list = await sqlService.GetStudentsByGroupAsync(groupName);
            listStudents = list.ToList();
            dgStudents.ItemsSource = listStudents;
        }

        private async void btnDelStudent_Click(object sender, RoutedEventArgs e)
        {
            if(MessageBox.Show("Вы уверены, что хотите удалить студента из списка?", "Удаление", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
            {
                await sqlService.DeleteStudentAsync(GetId());
                GetStudentsAsync();
            }
        }

        private async void btnUpdStudent_Click(object sender, RoutedEventArgs e)
        {
            var student = await sqlService.GetStudentByIdAsync(GetId());
            MyFrame.frame.Navigate(new AddOrUpdateStudentPage(student));
        }

        private void btnAddStudent_Click(object sender, RoutedEventArgs e)
        {
            MyFrame.frame.Navigate(new AddOrUpdateStudentPage(groupName));
        }

        private async void btnInfo_Click(object sender, RoutedEventArgs e)
        {
            MyFrame.frame.Navigate(new StudentProfilePage(await sqlService.GetStudentsByGroupAsync(groupName), dgStudents.SelectedIndex));
        }

        private int GetId()
        {
            var index = dgStudents.SelectedIndex;
            var info = listStudents[index];
            int id = info.Id;
            return id;
        }
    }
}

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
        SqlStudents sqlStudents;
        List<Students> listStudents;
        public StudentsListPage(string GroupName)
        {
            InitializeComponent();
            groupName = GroupName;
            sqlStudents = new SqlStudents();
            GetStudentsAsync();
        }

        public async void GetStudentsAsync()
        {
            var list = await sqlStudents.GetStudentsByGroupAsync(groupName);
            listStudents = list.ToList();
            dgStudents.ItemsSource = listStudents;
        }

        private void btnDelStudent_Click(object sender, RoutedEventArgs e)
        {

        }

        private void btnUpdStudent_Click(object sender, RoutedEventArgs e)
        {

        }

        private void btnAddStudent_Click(object sender, RoutedEventArgs e)
        {
            MyFrame.frame.Navigate(new AddOrUpdateStudentPage(groupName));
        }

        private void btnInfo_Click(object sender, RoutedEventArgs e)
        {
            int id = GetId();
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

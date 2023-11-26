using CuratorWpfApp.Models.ServicesDB;
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
        public StudentsListPage(string GroupName)
        {
            InitializeComponent();
            groupName = GroupName;
            sqlStudents = new SqlStudents();
            var list = GetStudentsAsync();
        }

        public async Task<IEnumerable> GetStudentsAsync()
        {
            var list = await sqlStudents.GetStudentsByGroupAsync(groupName);
            return list;
        }
    }
}

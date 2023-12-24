using CuratorWpfApp.Models.ServicesDB;
using CuratorWpfApp.Services;
using System.Windows;
using System.Windows.Controls;

namespace CuratorWpfApp.Pages.Curator
{
    /// <summary>
    /// Логика взаимодействия для JournalPage.xaml
    /// </summary>
    public partial class JournalPage : Page
    {
        string groupName;
        SqlQueryService sqlService = new SqlQueryService();
        int idDiscipline;
        int semester = 1;
        public JournalPage(string groupName)
        {
            InitializeComponent();
            this.groupName = groupName;
        }

        private void btnBack_Click(object sender, RoutedEventArgs e)
        {
            MyFrame.frame.GoBack();
        }

        private async void Page_Loaded(object sender, RoutedEventArgs e)
        {
            if (semester == 1)
                btn1Semester.IsChecked = true;
            else if (semester == 2)
                btn2Semester.IsChecked = true;
            try
            {
                var l = await sqlService.GetDisciplinesAsync(groupName);

                cmbJournal.ItemsSource = l;

            }
            catch
            {
                cmbJournal.ItemsSource = null;
            }
        }

        private async void cmbJournal_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            await FillingDg();
        }

        private async Task FillingDg()
        {
            string[] separators = { "Название: ", "Преподаватель: " };
            var arrData = cmbJournal.SelectedItem.ToString()?.Split(separators, StringSplitOptions.TrimEntries);
            List<string> newArr = new();
            foreach (var item in arrData)
            {
                if (!string.IsNullOrEmpty(item))
                    newArr.Add(item);
            }
            try
            {
                idDiscipline = await sqlService.GetIdAcademicDisciplinesAsync(groupName, newArr[0], newArr[1]);

                var l = await sqlService.GetJournalAsync(groupName, idDiscipline, semester);
                var arr = l.ToArray();
                var listId = new List<int>(arr.ElementAt(0).Id.Length/3);
                for (int i = 0; i < arr.Length; i++)
                {
                    arr[i].Grades = arr[i].Grades.Replace("</Grade><Grade>", " | ");
                    arr[i].Grades = arr[i].Grades.Replace("<Grade>", "");
                    arr[i].Grades = arr[i].Grades.Replace("</Grade>", "");
                    arr[i].Id = arr[i].Id.ToString().Replace("<Id>", "").Replace("</Id>", "");
                    foreach (var VARIABLE in arr[i].Id)
                    {
                        listId.Add(int.Parse(VARIABLE.ToString()));
                    }
                }
                dgJournal.ItemsSource = arr;

            }
            catch
            {
                MessageBox.Show("Не удалось получить данные");
                dgJournal.ItemsSource = null;
            }

        }

        private async void btn1Semester_Checked(object sender, RoutedEventArgs e)
        {
            semester = 1;
            if(cmbJournal.SelectedItem != null) 
                await FillingDg();
        }

        private async void btn2Semester_Checked(object sender, RoutedEventArgs e)
        {
            semester = 2;
            if(cmbJournal.SelectedItem != null) 
                await FillingDg();
        }

        private void ButtonBase_OnClick(object sender, RoutedEventArgs e)
        {
            MyFrame.frame.Navigate(new AddOrUpdateGradesPage(groupName));
        }
    }
}

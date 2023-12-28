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
using CuratorWpfApp.Models.Enitities;
using CuratorWpfApp.Models.ServicesDB;
using CuratorWpfApp.Services;

namespace CuratorWpfApp.Pages.Curator
{
    /// <summary>
    /// Логика взаимодействия для FinalGradesPage.xaml
    /// </summary>
    public partial class FinalGradesPage : Page
    {
        private string _groupName;
        private SqlQueryService sqlService = new();
        private IEnumerable<string> DisciplinesList;
        private IEnumerable<Students> StudentsList;
        private int semester = 1;
        public FinalGradesPage(string groupName)
        {
            InitializeComponent();
            _groupName = groupName; 
            
        }

        private async Task OnLoaded()
        {
            semester = DateTime.Now.Month < 9 ? 2 : 1;
            int course = 0;
            for (int i = 0; i < _groupName.Length; i++)
            {
                if (char.IsDigit(_groupName[i]))
                {
                    course = Convert.ToInt32(_groupName[i].ToString());
                    break;
                }
            }
            await sqlService.AddOrUpdateFinalGrades(semester, course, _groupName);
            await FillingCmbStudents();
            await FillingCmbDiscipline();
            await DefaultFillDg();
        }

        private async Task FillingCmbDiscipline()
        {
            DisciplinesList = await sqlService.GetDisciplinesAsync(_groupName);
            var list = DisciplinesList.ToList();
            list.Insert(0, "Все");
            CmbDiscipline.ItemsSource = list;
        }

        private async Task FillingCmbStudents()
        {
            StudentsList = await sqlService.GetStudentsByGroupAsync(_groupName);
            var list = StudentsList.Select(x => x.Full_name).ToList();
            list.Insert(0, "Все");
            CmbStudents.ItemsSource = list;
        }

        private void BtnBack_OnClick(object sender, RoutedEventArgs e)
        {
            MyFrame.frame.GoBack();
        }

        private async void CmbDiscipline_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            await Filter();
        }

        private async void CmbStudents_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            await Filter();
        }

        private async void FinalGradesPage_OnLoaded(object sender, RoutedEventArgs e)
        {
            await OnLoaded();
        }

        private async Task Filter()
        {
            string disciplineName;
            if (
                (CmbStudents.SelectedItem == null && CmbDiscipline.SelectedItem == null) ||
                (CmbStudents.SelectedItem?.ToString()=="Все" && CmbDiscipline.SelectedItem?.ToString() == "Все") ||
                (CmbStudents.SelectedItem == null && CmbDiscipline.SelectedItem?.ToString() == "Все") ||
                (CmbStudents.SelectedItem?.ToString() == "Все" && CmbDiscipline.SelectedItem == null)
                )
            {
                await DefaultFillDg();
                return;
            }

            if ((CmbDiscipline.SelectedItem == null || CmbDiscipline.SelectedItem.ToString() == "Все") && CmbStudents.SelectedItem != null && CmbStudents.SelectedItem.ToString() != "Все")
            {
                var l1 = await sqlService.GetFinalJournal(_groupName, CmbStudents.SelectedItem.ToString(), true);
                DgJournal.ItemsSource = l1;
                DgJournalDetailed.ItemsSource = l1;
                return;
            }

            if ((CmbStudents.SelectedItem == null || CmbStudents.SelectedItem.ToString() == "Все") && CmbDiscipline.SelectedItem != null)
            {
                GetName(out disciplineName);
                var l1 = await sqlService.GetFinalJournal(_groupName, disciplineName, false);
                DgJournal.ItemsSource = l1;
                DgJournalDetailed.ItemsSource = l1;
                return;
            }
            GetName(out disciplineName);
            var l = await sqlService.GetFinalJournal(_groupName, CmbStudents.SelectedItem.ToString(), disciplineName);
            DgJournal.ItemsSource = l;
            DgJournalDetailed.ItemsSource = l;

        }

        private void GetName(out string str)
        {
            string[] separators = { "Название: ", "Преподаватель: " };
            var arrData = CmbDiscipline.SelectedItem.ToString()?.Split(separators, StringSplitOptions.TrimEntries);
            List<string> newArr = new();
            foreach (var item in arrData)
            {
                if (!string.IsNullOrEmpty(item))
                    newArr.Add(item);
            }

            str =  newArr[0];
        }

        private async Task DefaultFillDg()
        {
            var l = await sqlService.GetFinalJournal(_groupName);
            DgJournal.ItemsSource = l;
            DgJournalDetailed.ItemsSource = l;
        }
    }
}

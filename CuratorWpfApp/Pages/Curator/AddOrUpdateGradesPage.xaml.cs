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
    /// Логика взаимодействия для AddOrUpdateGradesPage.xaml
    /// </summary>
    public partial class AddOrUpdateGradesPage : Page
    {
        private int lastFilter = 0;

        private string selectedDate;
        private string groupName;
        private IEnumerable<Students> l;
        private int studentId = 0;
        private int disciplineId = 0;
        private int grade = 1;
        private int semester = 1;
        private int DateId = 0;
        private SqlQueryService sqlService = new ();
        private DateTime semesterStart = new (DateTime.Now.Year, 9, 1);
        private DateTime semesterEnd = new (DateTime.Now.Year, 12, 31);
        private List<DateLessons> dateLessonsList;

        private List<SqlQueryService.View> arrViews = new();

        public AddOrUpdateGradesPage(string groupName)
        {
            InitializeComponent();
            this.groupName = groupName;
        }

        private void btnBack_OnClick(object sender, RoutedEventArgs e)
        {
            MyFrame.frame.GoBack();
        }

        private async void cmbJournal_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            await FillingDg();
            await Filter1();
            await Filter2();
        }

        private async void DatePicker_OnSelectedDateChanged(object? sender, SelectionChangedEventArgs e)
        {
            await Filter2();
        }

        private async Task Filter1()
        {

            if (cmbJournal.SelectedItem == null || cmbStudents.SelectedItem == null)
            {
                cmbStudents.SelectedItem = null;
                return;
            }
            if (cmbStudents.SelectedItem.ToString() == "Все")
            {
                studentId = 0;
                await FillingDg();
                return;
            } 
            tbGrade.Text = null;
            studentId = l.Where(x => x.Full_name == cmbStudents.SelectedItem.ToString()).Select(x => x.Id).First();
            if (arrViews.Count > 0)
            {
                if (!string.IsNullOrEmpty(selectedDate))
                {
                    var grade = await sqlService.GetGradeByFilters(studentId, disciplineId, DateId);
                    if (grade != null)
                    {
                        tbGrade.Text = grade.Value.ToString();
                    }
                }

                dgJournal.ItemsSource = arrViews.Where(x => x.Full_name == cmbStudents.SelectedItem.ToString());
            }

            lastFilter = 1;
        }

        private async Task Filter2()
        {
            if(LessonDatePicker.SelectedDate == null)
                return;
            if (LessonDatePicker.SelectedDate > semesterEnd ||
                LessonDatePicker.SelectedDate < semesterStart ||
                LessonDatePicker.SelectedDate.Value.DayOfWeek == DayOfWeek.Sunday)
            {
                LessonDatePicker.SelectedDate = null;
                MessageBox.Show("Некорректная дата");
                tbGrade.Text = null;
            }
            else
            {
                StringBuilder s = new(LessonDatePicker.SelectedDate.ToString().Replace('.', '/').Replace(" 0:00:00", ""));
                
                var s1 = s.ToString()[..3];
                //var s2 = s.ToString()[3..5];
                s.Remove(0, 3);
                s.Insert(3, s1);
                var ss = s.ToString();
                selectedDate = ss;
                var a = dateLessonsList.FirstOrDefault(x => x.DateL == ss);
                if (a == null)
                {
                    try
                    {
                        var r = await sqlService.AddNewDateLesson(LessonDatePicker.SelectedDate.ToString());
                        dateLessonsList = (List<DateLessons>)await sqlService.GetDateLessons();
                        a = r.First();
                    }
                    catch (Exception exception)
                    {
                        Console.WriteLine(exception);
                        throw;
                    }
                }

                DateId = a.Id;
                if (studentId > 0 && disciplineId > 0)
                {
                    var grade = await sqlService.GetGradeByFilters(studentId, disciplineId, DateId);
                    if (grade != null)
                    {
                        tbGrade.Text = grade.Value.ToString();
                    }
                }
            }

            lastFilter = 2;
        }

        private async void CmbStudents_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            await Filter1();
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
                disciplineId = await sqlService.GetIdAcademicDisciplinesAsync(groupName, newArr[0], newArr[1]);

                var l = await sqlService.GetJournalAsync(groupName, disciplineId, semester);
                var arr = l.ToArray();
                var listId = new List<int>(arr.ElementAt(0).Id.Length / 3);
                for (int i = 0; i < arr.Length; i++)
                {
                    arr[i].Grades = arr[i].Grades.Replace("</Grade><Grade>", " | ");
                    arr[i].Grades = arr[i].Grades.Replace("<Grade>", "");
                    arr[i].Grades = arr[i].Grades.Replace("</Grade>", "");
                    arr[i].Id = arr[i].Id.Replace("<Id>", "").Replace("</Id>", "");
                    foreach (var VARIABLE in arr[i].Id)
                    {
                        listId.Add(int.Parse(VARIABLE.ToString()));
                    }
                }

                arrViews = arr.ToList();
                dgJournal.ItemsSource = arrViews;

            }
            catch
            {
                MessageBox.Show("Не удалось получить данные");
                dgJournal.ItemsSource = null;
            }
        }

        private async Task FillingCmbStudents()
        {
            try
            {
                l = await sqlService.GetStudentsByGroupAsync(groupName);
                var ls = l.Select(x => x.Full_name).ToList();
                ls.Insert(0,"Все");
                cmbStudents.ItemsSource = ls;
            }
            catch (Exception e)
            {
                
            }
        }

        private async Task FillingCmbJournal()
        {
            try
            {
                var ls = await sqlService.GetDisciplinesAsync(groupName);

                cmbJournal.ItemsSource = ls;
            }
            catch (Exception e)
            {

            }
        }

        private async void AddOrUpdateGradesPage_OnLoaded(object sender, RoutedEventArgs e)
        {
            await OnLoaded();
        }

        private async Task OnLoaded()
        {
            try
            {
                if (dgJournal.ItemsSource != null)
                {
                    if (lastFilter == 1)
                        await Filter1();
                    else if (lastFilter == 2)
                        await Filter2();
                }
                dateLessonsList = (List<DateLessons>)await sqlService.GetDateLessons();
                await FillingCmbJournal();
                await FillingCmbStudents();
            }
            catch (Exception exception)
            {
                
            }
            semester = DateTime.Now.Month < 9 ? 2 : 1;
            if (semester == 1)
            {
                LessonDatePicker.DisplayDateStart = semesterStart;
                LessonDatePicker.DisplayDateEnd = semesterEnd;
            }
            else
            {
                semesterStart = new (DateTime.Now.Year, 1, 14);
                semesterEnd = new(DateTime.Now.Year, 6, 30);
                LessonDatePicker.DisplayDateStart = semesterStart;
                LessonDatePicker.DisplayDateEnd = semesterEnd;
            }

        }

        private async void ButtonBase_OnClick(object sender, RoutedEventArgs e)
        {
            if (int.TryParse(tbGrade.Text, out grade) &&
                grade is > 0 and <= 5 &&
                studentId > 0 &&
                disciplineId > 0 &&
                DateId > 0 &&
                semester == 1 || semester == 2)
            {
                await sqlService.AddOrUpdateGrade(studentId, disciplineId, DateId, semester, grade);
                MessageBox.Show("Успешно сохранено");
                await OnLoaded();
            }
        }
    }
}

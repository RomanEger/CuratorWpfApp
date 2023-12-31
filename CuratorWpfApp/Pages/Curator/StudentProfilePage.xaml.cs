﻿using CuratorWpfApp.Models.Enitities;
using CuratorWpfApp.Models.ServicesDB;
using CuratorWpfApp.Services;
using System;
using System.Collections.Generic;
using System.Globalization;
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
    /// Логика взаимодействия для StudentProfilePage.xaml
    /// </summary>
    public partial class StudentProfilePage : Page
    {
        //IEnumerable<Students> students;

        int semester;

        int studentID;

        int index;

        SqlQueryService sqlService = new SqlQueryService();

        Students[] studentsArray;

        Students currentStudent;

        List<Certificates> certificatesList;
        public StudentProfilePage(IEnumerable<Students> students, int index=0, int semester=1)
        {
            InitializeComponent();

            this.semester = semester;

            this.index = index;

            studentsArray = students.ToArray();
        }

        public async void FillingPage(Students student, int semester)
        {
            if (semester == 1)
                btn1Semester.IsChecked = true;
            else
                btn2Semester.IsChecked = true;

            dgCertificates.ItemsSource = null;
            try
            {
                var l = await sqlService.GetCertificatesByStudentIdAsync(student.Id);
                foreach(var item in l)
                {
                    item.Start_date = item.Start_date?.Replace(" 00:00:00", "");
                    item.End_date = item.End_date?.Replace(" 00:00:00", "");
                }
                certificatesList = l.ToList();
                dgCertificates.ItemsSource = certificatesList;
            }
            catch
            {

            }

            textBlockDebt.Text = string.Empty;
            textBlockDebt.Visibility = Visibility.Collapsed;
            dgDebts.Visibility = Visibility.Collapsed;
            try
            {
                var l = await sqlService.GetDebtByIStudentIdAsync(student.Id, semester);
                if(l.Count() > 0)
                {
                    textBlockDebt.Text = "Долги";
                    textBlockDebt.Visibility= Visibility.Visible;
                    dgDebts.Visibility = Visibility.Visible;
                    dgDebts.ItemsSource = l;
                }
            }
            catch
            {

            }

            if (student.Has_Military_id)
                textBlockMilId.Text = "Приписной: В наличии";
            else
                textBlockMilId.Text = "Приписной: Нет";

            textBlockAvg.Text = string.Empty;

            textBlockGrades.Text = string.Empty;

            cmbDisciplines.SelectedItem = null;

            studentID = student.Id;

            DateTime dt;

            DateTime.TryParseExact(student.Birthday, "MM/dd/yyyy", null, DateTimeStyles.None, out dt);

            textBlockFullName.Text = $"ФИО: {student.Full_name}";
            textBlockGroupName.Text = $"Группа: {student.Group_name}";
            textBlockBirthday.Text = $"День рождения: {dt.ToString("D")}";

            var list = new List<string> { "Средний балл" };
            try
            {
                list.AddRange(await sqlService.GetDisciplinesAsync(student.Group_name));
                cmbDisciplines.ItemsSource = list;
            }
            catch
            {

            }
            try
            {
                ImageService imageService = new ImageService();
                imgChel.Source = imageService.ByteToImage(imageService.GetBytes(student.Id));
            }
            catch
            {
                imgChel.Source = null;
            }
        }

        private void btnBack_Click(object sender, RoutedEventArgs e)
        {
            MyFrame.frame.GoBack();
        }

        private void btnNextProfile_Click(object sender, RoutedEventArgs e)
        {
            if (index < studentsArray.Length - 1)
            {
                currentStudent = studentsArray[++index];
                FillingPage(currentStudent, semester);
            }
            else
            {
                currentStudent = studentsArray[index = 0];
                FillingPage(currentStudent, semester);
            }
        }

        private void btnPresentProfile_Click(object sender, RoutedEventArgs e)
        {
            if (index > 0)
            {
                currentStudent = studentsArray[--index];
                FillingPage(currentStudent, semester);
            }
            else
            {
                currentStudent = studentsArray[index = studentsArray.Length - 1];
                FillingPage(currentStudent, semester);
            }

        }

        private async void cmbDisciplines_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            await cmbFunc();
        }

        private async Task cmbFunc()
        {
            if (cmbDisciplines.SelectedItem == null)
                return;

            if (cmbDisciplines.SelectedIndex != 0)
            {
                string[] separators = { "Название: ", "Преподаватель: " };
                var arr = cmbDisciplines.SelectedItem.ToString()?.Split(separators, StringSplitOptions.TrimEntries);
                List<string> newArr = new();
                foreach (var item in arr)
                {
                    if (!string.IsNullOrEmpty(item))
                        newArr.Add(item);
                }
                try
                {
                    var grades = await sqlService.GetGradesByStudentAndDiscipline(studentID, newArr[0], newArr[1], semester);
                    var gradesArr = grades.ToArray();
                    textBlockGrades.Text = "Оценки: ";
                    for (int i = 0; i < gradesArr.Length - 1; i++)
                        textBlockGrades.Text += gradesArr[i] + " | ";
                    textBlockGrades.Text += gradesArr[gradesArr.Length - 1];
                    textBlockAvg.Text = "Средний балл: " + Math.Round(gradesArr.Average(), 3).ToString();
                }
                catch
                {
                    textBlockAvg.Text = string.Empty;
                    textBlockGrades.Text = string.Empty;
                    MessageBox.Show("Не удалось получить данные");
                }
            }
            else
            {
                try
                {
                    textBlockAvg.Text = $"Средний балл: {await sqlService.GetAvgGradeByStudent(studentID, semester)}";
                }
                catch
                {
                    textBlockAvg.Text = string.Empty;
                    MessageBox.Show("Не удалось получить данные");
                }
                textBlockGrades.Text = string.Empty;
            }

        }

        private async void btn1Semester_Checked(object sender, RoutedEventArgs e)
        {
            semester = 1;
            await cmbFunc();
        }

        private async void btn2Semester_Checked(object sender, RoutedEventArgs e)
        {
            semester = 2;
            await cmbFunc();
        }

        private void btnAddCertificate_Click(object sender, RoutedEventArgs e)
        {
            MyFrame.frame.Navigate(new AddOrUpdateCertificatePage(currentStudent));

        }

        private async void btnUpdCertificate_Click(object sender, RoutedEventArgs e)
        {
            var certificate = await sqlService.GetCertificateByIdAsync(GetId());
            MyFrame.frame.Navigate(new AddOrUpdateCertificatePage(certificate, currentStudent.Group_name));
        }

        private int GetId()
        {
            var index = dgCertificates.SelectedIndex;
            var info = certificatesList[index];
            int id = info.Id;
            return id;
        }

        private async void btnDelCertificate_Click(object sender, RoutedEventArgs e)
        {
            if (MessageBox.Show("Вы уверены, что хотите удалить студента из списка?", "Удаление", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
            {
                await sqlService.DeleteCertificateAsync(GetId());
                FillingPage(currentStudent, semester);
            }
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                currentStudent = studentsArray[index];
                FillingPage(currentStudent, semester);
            }
            catch
            {
                MessageBox.Show("Не удалось получить анкету");
            }

        }
    }
}

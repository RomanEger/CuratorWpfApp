using CuratorWpfApp.Models.Enitities;
using CuratorWpfApp.Models.ServicesDB;
using CuratorWpfApp.Services;
using Microsoft.IdentityModel.Tokens;
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
    /// Логика взаимодействия для AddOrUpdateCertificatePage.xaml
    /// </summary>
    public partial class AddOrUpdateCertificatePage : Page
    {
        string groupName;
        Lazy<Students> student;
        Lazy<List<Students>> studentsList;
        Certificates certificate;
        SqlQueryService sqlService = new SqlQueryService();
        //add
        public AddOrUpdateCertificatePage(string groupName)
        {
            InitializeComponent();
            this.groupName = groupName;
            FillingPage();
        }

        //add
        public AddOrUpdateCertificatePage(Students student)
        {
            this.student = new Lazy<Students>(student);
            InitializeComponent();
            groupName = student.Group_name;
            FillingPage();
            certificate = new Certificates()
            {
                Student_id = student.Id,
            };
            cmbFullName.Visibility = Visibility.Collapsed;
            tbFullName.Visibility = Visibility.Visible;
            tbFullName.Text = student.Full_name;
            
        }

        //update
        public AddOrUpdateCertificatePage(Certificates certificate, string groupName)
        {
            InitializeComponent();
            this.certificate = certificate;
            this.groupName = groupName;
            FillingPage();
        }
        private async void FillingPage()
        {
            if(certificate == null)
            {
                cmbFullName.Visibility = Visibility.Visible;
                tbFullName.Visibility = Visibility.Collapsed;

                var l = await sqlService.GetStudentsByGroupAsync(groupName);
                cmbFullName.Items?.Clear();
                studentsList = new Lazy<List<Students>>(l.ToList());
                var fullName = l.Select(x => x.Full_name);
                cmbFullName.ItemsSource = fullName;
                certificate = new Certificates();
            }
            else
            {
                tbFullName.Visibility = Visibility.Visible;
                cmbFullName.Visibility = Visibility.Collapsed;
                try
                {
                    var student = await sqlService.GetStudentByIdAsync(certificate.Student_id);
                    tbFullName.Text = student.Full_name;
                }
                catch(Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }

                tbTitle.Text = certificate.Title;

                DateTime startDt;

                if (DateTime.TryParseExact(certificate.Start_date, "MM/dd/yyyy", null, DateTimeStyles.None, out startDt))
                    dpStart.SelectedDate = startDt;
                else
                    dpStart.SelectedDate = null;

                DateTime endDt;

                if (DateTime.TryParseExact(certificate.End_date, "MM/dd/yyyy", null, DateTimeStyles.None, out endDt))
                    dpEnd.SelectedDate = endDt;
                else
                    dpEnd.SelectedDate = null;
            }
        }

        private async void bntSave_Click(object sender, RoutedEventArgs e)
        {
            if((string.IsNullOrEmpty(tbFullName.Text) && cmbFullName.SelectedItem == null) ||
                string.IsNullOrEmpty(tbTitle.Text) ||
                dpStart.SelectedDate == null)
                MessageBox.Show("Все поля, кроме даты окончания справки, обязательны к заполнению");
            else if(dpStart?.SelectedDate > dpEnd?.SelectedDate)
                MessageBox.Show("Дата начала не может быть больше даты окончания");
            else
            {
                certificate.Title = tbTitle.Text;
                certificate.Start_date = dpStart?.Text ?? "";
                certificate.End_date = dpEnd?.Text ?? "";
                if (student?.Value != null)
                {
                    try
                    {
                        certificate.Student_id = student.Value.Id;
                        await sqlService.AddCertificateAsync(certificate);
                        MessageBox.Show("Справка успешно добавлена");
                    }
                    catch
                    {
                        MessageBox.Show("Не удалось добавить справку");
                    }
                }
                else
                {
                    try
                    {
                        await sqlService.UpdateCertificateAsync(certificate);
                        MessageBox.Show("Изменения успешно сохранены");
                    }
                    catch
                    {
                        MessageBox.Show("Не удалось изменить справку");
                    }
                }
            }
        }

        private void cmbFullName_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var FullName = cmbFullName.SelectedItem.ToString();
            student = new Lazy<Students>(studentsList?.Value.Where(x => x.Full_name == FullName).FirstOrDefault());
            certificate.Student_id = student.Value.Id;
        }

        private void btnBack_Click(object sender, RoutedEventArgs e)
        {
            MyFrame.frame.GoBack();
        }

    }
}

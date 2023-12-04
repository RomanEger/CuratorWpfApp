using CuratorWpfApp.Models.Enitities;
using CuratorWpfApp.Models.ServicesDB;
using CuratorWpfApp.Services;
using Microsoft.Win32;
using System.Globalization;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;

namespace CuratorWpfApp.Pages.Curator
{
    /// <summary>
    /// Логика взаимодействия для AddOrUpdateStudentPage.xaml
    /// </summary>
    public partial class AddOrUpdateStudentPage : Page
    {
        byte[] bytes;
        int idOperation;
        string photo;
        SqlQueryService sqlService = new SqlQueryService();
        int id;
        public AddOrUpdateStudentPage(string groupName)
        {
            InitializeComponent();

            tbGroupName.Text = groupName;

            labelTitle.Content = "Добавление студента";

            btnChangePhoto.Content = "Добавить фото";

            idOperation = 0;
        }

        public AddOrUpdateStudentPage(Students student)
        {
            InitializeComponent();

            labelTitle.Content = "Редактирование студента";

            var arr = student.Full_name.Split(' ');

            tbLName.Text = arr[0] ?? null;

            tbFName.Text = arr[1] ?? null;

            tbPatronymic.Text = arr[2] ?? null;

            DateTime dt;

            var b = DateTime.TryParseExact(student.Birthday, "MM/dd/yyyy", null, DateTimeStyles.None, out dt);

            tbBirthday.SelectedDate = dt;

            tbGroupName.Text = student.Group_name;

            cbMilID.IsChecked = student.Has_Military_id;

            btnChangePhoto.Content = "Изменить фото";

            idOperation = 1;

            id = student.Id;
            try
            {
                ImageService imageService = new ImageService();
                imgChel.Source = imageService.ByteToImage(imageService.GetBytes(student.Id));//sqlService.GetImageFromDatabase(student.Id);
            }
            catch
            {
                imgChel.Source = null;
            }
        }



        private async void btnSave_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(tbFName.Text) ||
               string.IsNullOrEmpty(tbLName.Text) ||
               string.IsNullOrEmpty(tbBirthday.Text) ||
               string.IsNullOrEmpty(tbGroupName.Text) ||
               imgChel.Source == null)
            {
                MessageBox.Show("Убедитесь, что заполнили все обязательные поля и попробуйте снова");
                return;
            }

            try
            {
                string fullName;
                bool hasMilitaryId = false;
                if (string.IsNullOrEmpty(tbPatronymic.Text))
                    fullName = tbLName.Text + ' ' + tbFName.Text;
                else
                    fullName = tbLName.Text + ' ' + tbFName.Text + ' ' + tbPatronymic.Text;
                if (cbMilID.IsChecked == true)
                    hasMilitaryId = true;
                Students student = new Students()
                {
                    Id = id,
                    Full_name = fullName,
                    Birthday = tbBirthday.Text,
                    Group_name = tbGroupName.Text,
                    Photo = bytes,
                    Has_Military_id = hasMilitaryId
                };
                if (idOperation == 0)
                {
                    var r = await sqlService.AddStudentAsync(student, photo);
                    if (r > 0)
                        MessageBox.Show("Студент добавлен");
                }
                else if (idOperation == 1)
                {
                    var r = await sqlService.UpdateStudentAsync(student, photo);
                    if (r > 0)
                        MessageBox.Show("Изменения сохранены");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void btnBack_Click(object sender, RoutedEventArgs e)
        {
            MyFrame.frame.Navigate(new StudentsListPage(tbGroupName.Text));
        }
        private void btnChangePhoto_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Image files (*.BMP, *.JPG, *.GIF, *.TIF, *.PNG, *.ICO, *.EMF, *.WMF)|*.bmp;*.jpg;*.gif; *.tif; *.png; *.ico; *.emf; *.wmf";
            if (openFileDialog.ShowDialog() == true)
            {
                bytes = File.ReadAllBytes(openFileDialog.FileName);
                photo = "0x" + BitConverter.ToString(bytes).Replace("-", "");
                imgChel.Source = new BitmapImage(new Uri(openFileDialog.FileName));
            }

        }

    }
}

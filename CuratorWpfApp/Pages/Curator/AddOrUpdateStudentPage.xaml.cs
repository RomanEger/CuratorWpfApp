using CuratorWpfApp.Models.Enitities;
using CuratorWpfApp.Services;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Resources;
using System.Text;
using System.Text.RegularExpressions;
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
    /// Логика взаимодействия для AddOrUpdateStudentPage.xaml
    /// </summary>
    public partial class AddOrUpdateStudentPage : Page
    {
        int idOperation;
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

            tbBirthday.Text = student.Birthday;
            
            tbGroupName.Text = student.Group_name;

            cbMilID.IsChecked = student.Has_Military_id;

            btnChangePhoto.Content = "Изменить фото";

            idOperation = 1;

            Uri uri = new Uri($"pack://application:,,,/CuratorWpfApp;component/{student.Photo}");
            BitmapImage bitmapImage = new BitmapImage(uri);
            imgChel.Source = bitmapImage;
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            if(idOperation == 0)
            {

            }
            else if(idOperation == 1) 
            {
            
            }
        }

        private void btnBack_Click(object sender, RoutedEventArgs e)
        {
            MyFrame.frame.GoBack();
        }

        private void btnChangePhoto_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                OpenFileDialog fileDialog = new OpenFileDialog();
                if (fileDialog.ShowDialog() == true)
                {
                    var s = fileDialog.FileName;
                    var savePath = System.IO.Path.Combine("Resources", fileDialog.SafeFileName);
                    //File.Copy(s, savePath, true);

                    Uri uri = new Uri($"pack://application:,,,/CuratorWpfApp;component/{savePath}");
                    
                    BitmapImage bitmapImage = new BitmapImage(new Uri(s));
                    imgChel.Source = bitmapImage;

                    var p = new Microsoft.Build.Evaluation.Project(@"C:\projects\BabDb\test\test.csproj");
                    p.AddItem("Compile", @"C:\projects\BabDb\test\test2\Class1.cs");
                    p.Save();
                }
            }
            catch(Exception ex)
            {
                MessageBox.Show("Изображение не найдено");
            }
        }

    }
}

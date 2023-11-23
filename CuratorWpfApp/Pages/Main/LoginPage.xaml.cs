using CuratorWpfApp.Models.ServicesDB;
using CuratorWpfApp.Pages.Main;
using CuratorWpfApp.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace CuratorWpfApp.Pages
{
    /// <summary>
    /// Логика взаимодействия для LoginPage.xaml
    /// </summary>
    public partial class LoginPage : Page
    {
        public LoginPage()
        {
            InitializeComponent();
        }

        private void CheckBox_Click(object sender, RoutedEventArgs e)
        {
            var checkBox = sender as CheckBox;
            if (checkBox.IsChecked == true)
            {
                tbPassword.Visibility = Visibility.Visible;
                tbPassword.Text = pbPassword.Password;
                pbPassword.Visibility = Visibility.Hidden;
            }
            else
            {
                pbPassword.Visibility = Visibility.Visible;
                pbPassword.Password = tbPassword.Text;
                tbPassword.Visibility = Visibility.Hidden;
            }
        }

        private void Label_MouseDown(object sender, MouseButtonEventArgs e)
        {
            MyFrame.frame.Navigate(new RegistrationPage());
        }

        private async void btnLogin_Click(object sender, RoutedEventArgs e)
        {
            string password;
            if (tbPassword.Visibility == Visibility.Visible)
                password = tbPassword.Text;
            else
                password = pbPassword.Password;

            SqlUsers sql = new SqlUsers();
            try
            {
                var r = await sql.Login(tbLogin.Text, password);
                if(r == 1)
                {
                    CuratorWindow curatorWindow = new CuratorWindow();
                    curatorWindow.Show();
                }
                else
                {
                    AdminWindow adminWindow = new AdminWindow();
                    adminWindow.Show();
                }
            }
            catch (Exception ex)
            {
                popup1.IsOpen = true;
                textBlock.Content = ex.Message;
            }
        }
    }
}

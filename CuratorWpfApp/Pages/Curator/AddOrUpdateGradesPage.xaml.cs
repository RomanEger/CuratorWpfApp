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

namespace CuratorWpfApp.Pages.Curator
{
    /// <summary>
    /// Логика взаимодействия для AddOrUpdateGradesPage.xaml
    /// </summary>
    public partial class AddOrUpdateGradesPage : Page
    {
        private int semester = 1;
        public AddOrUpdateGradesPage()
        {
            InitializeComponent();
        }

        private void btnBack_OnClick(object sender, RoutedEventArgs e)
        {
            
        }

        private void cmbJournal_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            
        }

        private void AddOrUpdateGradesPage_OnLoaded(object sender, RoutedEventArgs e)
        {
            semester = DateTime.Now.Month < 9 ? 1 : 2;
        }
    }
}

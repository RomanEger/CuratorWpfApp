using CuratorWpfApp.Models.Enitities;
using CuratorWpfApp.Models.ServicesDB;
using CuratorWpfApp.Services;
using System.Windows;
using System.Windows.Controls;

namespace CuratorWpfApp.Pages.Curator
{
    /// <summary>
    /// Логика взаимодействия для DebtPage.xaml
    /// </summary>
    public partial class DebtPage : Page
    {
        string groupName;
        private int semester ;
        public DebtPage(string groupName)
        {
            InitializeComponent();
            this.groupName = groupName;
            semester = DateTime.Now.Month < 9 ? 2 : 1;
        }

        private void btnBack_Click(object sender, RoutedEventArgs e)
        {
            MyFrame.frame.GoBack();
        }

        private async void Page_Loaded(object sender, RoutedEventArgs e)
        {
            SqlQueryService sqlService = new();

            var debtList = await sqlService.GetDebtByGroupAsync(groupName, semester);

            var index = new List<int>();

            Dictionary<string, int> dict = new Dictionary<string, int>();

            for (int i = 0; i < debtList.Count(); i++)
                if (dict.ContainsKey(debtList.ElementAt(i).DisciplineName))
                    index.Add(i);
                else
                    dict.Add(debtList.ElementAt(i).DisciplineName, i);
            

            for (int i = 0; i < debtList.Count(); i++)
                if (index.Contains(i))
                    debtList.ElementAt(i).DisciplineName = "";

            dgDebts.ItemsSource = debtList;
        }
    }
}

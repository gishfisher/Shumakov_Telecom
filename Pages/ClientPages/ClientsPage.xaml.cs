using Shumakov_Telecom.Data;
using Shumakov_Telecom.Services;
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
using Telecom.Utility;

namespace Telecom.Pages.ClientPages
{
    /// <summary>
    /// Логика взаимодействия для ClientsPage.xaml
    /// </summary>
    public partial class ClientsPage : Page
    {
        private TelecomServiceDeskEntities _db = TelecomServiceDeskEntities.GetContext();
        private ClientService _clientService = new ClientService();

        public ClientsPage()
        {
            InitializeComponent();
            DataGridClients.ItemsSource = _db.Clients.ToList();
        }

        private void ButtonAdd_Click(object sender, RoutedEventArgs e)
        {
            Manager.AppFrame.Navigate(new ClientAddEditPage(null));
        }

        private void ButtonEdit_Click(object sender, RoutedEventArgs e)
        {
            Manager.AppFrame.Navigate(new ClientAddEditPage((sender as Button).DataContext as Client));
        }

        private void ButtonDelete_Click(object sender, RoutedEventArgs e)
        {
            var clientForRemoving = (sender as Button).DataContext as Client;

            if (MessageBox.Show($"Вы уверены что хотите удалить клиента {clientForRemoving.GetFullName}?", 
                "Внимание", MessageBoxButton.YesNo) != MessageBoxResult.Yes)
                return;

            try
            {
                if (_clientService.RemoveClient(clientForRemoving))
                    MessageBox.Show("Клиент успешно удален!");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message.ToString());
            }

            DataGridClients.ItemsSource = _db.Clients.ToList();
        }

        private void Page_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (Visibility == Visibility.Visible)
            {
                _db.ChangeTracker.Entries().ToList().ForEach(p => p.Reload());

                DataGridClients.ItemsSource = _db.Clients.ToList();
            }
        }

        private void SearchBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            UpdateClients();
        }

        private void UpdateClients()
        {
            var currentRequests = _db.Clients.ToList();
            currentRequests = currentRequests.Where(x => x.GetFullName.ToLower().Contains(SearchBox.Text.ToLower())).ToList();
            DataGridClients.ItemsSource = currentRequests.OrderBy(x => x.ClientId).ToList();
        }
    }
}

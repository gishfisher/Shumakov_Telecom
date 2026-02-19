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
    /// Логика взаимодействия для ClientAddEditPage.xaml
    /// </summary>
    public partial class ClientAddEditPage : Page
    {

        private Client _currentClient;

        public ClientAddEditPage(Client selectedClient)
        {
            InitializeComponent();
            if (selectedClient != null)
            {
                _currentClient = selectedClient;
            }
            else
            {
                _currentClient = new Client();
            }

            DataContext = _currentClient;
        }

        private void btnAddClient_Click(object sender, RoutedEventArgs e)
        {
            StringBuilder errors = new StringBuilder();

            if (string.IsNullOrWhiteSpace(_currentClient.FirstName))
                errors.AppendLine("Укажите имя");
            if (string.IsNullOrWhiteSpace(_currentClient.LastName))
                errors.AppendLine("Укажите фамилию");
            if (string.IsNullOrWhiteSpace(_currentClient.Phone))
                errors.AppendLine("Укажите номер телефона");

            if  (errors.Length > 0)
            {
                MessageBox.Show(errors.ToString());
                return;
            }

            try
            {
                ClientService clientService = new ClientService();  

                if (_currentClient.ClientId == 0)
                {
                    if (clientService.AddClient(_currentClient))
                        MessageBox.Show($"Клиент {_currentClient.GetFullName}, успешно добавлен!");
                    Manager.AppFrame.GoBack();
                }
                else
                {
                    if (clientService.EditClient(_currentClient))
                        MessageBox.Show($"Клиент {_currentClient.GetFullName}, успешно отредактирован!");
                    Manager.AppFrame.GoBack();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        private void GoBack_Click(object sender, RoutedEventArgs e)
        {
            Manager.AppFrame.GoBack();
        }
    }
}

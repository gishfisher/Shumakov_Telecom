using Shumakov_Telecom.Data;
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
using Telecom.Services;
using Telecom.Utility;

namespace Telecom.Pages
{
    /// <summary>
    /// Логика взаимодействия для RequestPage.xaml
    /// </summary>
    public partial class RequestPage : Page
    {
        private TelecomServiceDeskEntities _db = TelecomServiceDeskEntities.GetContext();
        private RequestService _requestService = new RequestService();

        public RequestPage()
        {
            InitializeComponent();
            DataGridRequests.ItemsSource = _db.Requests.ToList();
        }

        private void AddRequest_Click(object sender, RoutedEventArgs e)
        {
            Manager.AppFrame.Navigate(new RequestAddEditPage(null));
        }
        private void ButtonEdit_Click(object sender, RoutedEventArgs e)
        {
            Manager.AppFrame.Navigate(new RequestAddEditPage((sender as Button).DataContext as Request));
        }

        private void ButtonDelete_Click(object sender, RoutedEventArgs e)
        {
            var requestForDelete = (sender as Button).DataContext as Request;

            if (MessageBox.Show($"Вы уверены что хотите удалить заявку №{requestForDelete.RequestId}?",
                "Внимание", MessageBoxButton.YesNo) != MessageBoxResult.Yes)
                return;

            _requestService.DeleteRequest(requestForDelete);
            MessageBox.Show($"Заявка №{requestForDelete.RequestId} успешно удалена");

            DataGridRequests.ItemsSource = _db.Requests.ToList();
        }

        private void Page_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (Visibility == Visibility.Visible)
            {
                _db.ChangeTracker.Entries().ToList().ForEach(p => p.Reload());

                DataGridRequests.ItemsSource = _db.Requests.ToList();
            }
        }
    }
}

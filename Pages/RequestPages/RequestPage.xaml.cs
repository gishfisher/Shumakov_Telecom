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
            
            var allStatuses = _db.RequestStatuses.ToList();
            var allPriotity = _db.RequestPriorities.ToList();

            allStatuses.Insert(0, new RequestStatus
            {
                Name = "Все статусы"
            });
            ComboStatus.ItemsSource = allStatuses;

            allPriotity.Insert(0, new RequestPriority
            {
                Name = "Все приоритеты"
            });
            ComboPriotity.ItemsSource = allPriotity;

            CheckClosed.IsChecked = true;
            ComboStatus.SelectedIndex = 0;
            ComboPriotity.SelectedIndex = 0;

            var currentRequest = _db.RequestStatuses.ToList();
            DataGridRequests.ItemsSource = currentRequest;

            UpdateRequests();
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

            MessageBox.Show($"Заявка №{requestForDelete.RequestId} успешно удалена");
            _requestService.DeleteRequest(requestForDelete);

            DataGridRequests.ItemsSource = _db.Requests.ToList();
        }

        private void Page_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (Visibility == Visibility.Visible)
            {
                _db.ChangeTracker.Entries().ToList().ForEach(p => p.Reload());

                DataGridRequests.ItemsSource = _db.Requests.ToList();
                UpdateRequests();
            }
        }

        private void CheckBox_Checked(object sender, RoutedEventArgs e)
        {
            UpdateRequests();
        }

        private void CheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            UpdateRequests();
        }

        private void SearchBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            UpdateRequests();
        }
        private void ComboPriotity_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            UpdateRequests();
        }

        private void ComboStatus_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            UpdateRequests();
        }
        private void CheckCurrentUser_Checked(object sender, RoutedEventArgs e)
        {
            UpdateRequests();
        }

        private void CheckCurrentUser_Unchecked(object sender, RoutedEventArgs e)
        {
            UpdateRequests();
        }

        private void UpdateRequests()
        {
            var currentUser = UserSessionService.CurrentUser?.Employees.FirstOrDefault().EmployeeId;
            var currentRequests = _db.Requests.ToList();

            if (ComboPriotity.SelectedIndex > 0)
                currentRequests = currentRequests.Where(x => x.RequestPriority == (ComboPriotity.SelectedItem as RequestPriority)).ToList();

            if (ComboStatus.SelectedIndex > 0)
                currentRequests = currentRequests.Where(x => x.RequestStatus == (ComboStatus.SelectedItem as RequestStatus)).ToList();

            currentRequests = currentRequests.Where(x => x.Client.GetFullName.ToLower().Contains(SearchBox.Text.ToLower())).ToList();

            if (CheckClosed.IsChecked.Value)
                currentRequests = currentRequests.Where(x => x.StatusId != (int)RequestStatusType.Closed).ToList();
            if (CheckCurrentUser.IsChecked.Value)
                currentRequests = currentRequests.Where(x => x.EmployeeId == currentUser).ToList();

            DataGridRequests.ItemsSource = currentRequests.OrderBy(x => x.RequestId).ToList();
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            if (UserSessionService.IsMaster)
            {
                AddRequest.Visibility = Visibility.Collapsed;
            }
        }
    }
}

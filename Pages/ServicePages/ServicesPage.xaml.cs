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
using Telecom.Services;
using Telecom.Utility;

namespace Telecom.Pages.ServicePages
{
    /// <summary>
    /// Логика взаимодействия для ServicesPage.xaml
    /// </summary>
    public partial class ServicesPage : Page
    {
        private TelecomServiceDeskEntities _db = TelecomServiceDeskEntities.GetContext();
        private ProductService _productService = new ProductService();

        public ServicesPage()
        {
            InitializeComponent();
            DataGridServices.ItemsSource = _db.Services.ToList();
        }

        private void ButtonAdd_Click(object sender, RoutedEventArgs e)
        {
            Manager.AppFrame.Navigate(new ServiceAddEditPage(null));
        }
        private void ButtonEdit_Click(object sender, RoutedEventArgs e)
        {
            Manager.AppFrame.Navigate(new ServiceAddEditPage((sender as Button).DataContext as Service));
        }

        private void ButtonDelete_Click(object sender, RoutedEventArgs e)
        {
            var serviceForRemoving = (sender as Button).DataContext as Service;

            if (MessageBox.Show($"Вы уверены что хотите удалить услугу {serviceForRemoving.Name}?",
               "Внимание", MessageBoxButton.YesNo) != MessageBoxResult.Yes)
                return;

            try
            {
                MessageBox.Show($"Услуга {serviceForRemoving.Name} успешно удалена");
                _productService.RemoveService(serviceForRemoving);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message.ToString());
            }

            DataGridServices.ItemsSource = _db.Services.ToList();
        }

        private void Page_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (Visibility == Visibility.Visible)
            {
                _db.ChangeTracker.Entries().ToList().ForEach(p => p.Reload());

                DataGridServices.ItemsSource = _db.Services.ToList();
            }
        }

        private void SearchBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            UpdateServices();
        }

        private void UpdateServices()
        {
            var currentRequests = _db.Services.ToList();
            currentRequests = currentRequests.Where(x => x.Name.ToLower().Contains(SearchBox.Text.ToLower())).ToList();
            DataGridServices.ItemsSource = currentRequests.OrderBy(x => x.ServiceId).ToList();
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            if (UserSessionService.IsMaster || UserSessionService.IsDispatch)
            {
                ButtonAdd.Visibility = Visibility.Collapsed;
            }
        }
    }
}

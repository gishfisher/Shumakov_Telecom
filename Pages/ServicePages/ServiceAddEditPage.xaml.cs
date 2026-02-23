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
    /// Логика взаимодействия для ServiceAddEditPage.xaml
    /// </summary>
    public partial class ServiceAddEditPage : Page
    {
        private Service _currentService;

        public ServiceAddEditPage(Service selectedService)
        {
            InitializeComponent();

            if (selectedService != null)
            {
                _currentService = selectedService;
            }
            else
            {
                _currentService = new Service();
            }

            DataContext = _currentService;
        }

        private void btnAddService_Click(object sender, RoutedEventArgs e)
        {
            StringBuilder errors = new StringBuilder();

            if (string.IsNullOrWhiteSpace(_currentService.Name))
                errors.AppendLine("Укажите название услуги");
            if (_currentService.Price == 0)
                errors.AppendLine("Укажите цену");
            if (_currentService.Price <= 0)
                errors.AppendLine("Цена не можеть быть отрицательной или равной нулю");

            if (errors.Length > 0)
            {
                MessageBox.Show(errors.ToString());
                return;
            }

            try
            {
                ProductService productService = new ProductService();

                if (_currentService.ServiceId == 0)
                {
                    if (productService.AddService(_currentService))
                        MessageBox.Show($"Услуга {_currentService.Name}, успешно добавлена!");
                    Manager.AppFrame.GoBack();
                }
                else
                {
                    if (productService.EditService(_currentService))
                        MessageBox.Show($"Услуга {_currentService.Name}, успешно отредактирована!");
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

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            if (UserSessionService.IsMaster || UserSessionService.IsDispatch)
            {
                LabelTitle.Content = "Просмотр услуги";
                btnAddService.Visibility = Visibility.Collapsed;
                ServiceInfoPanel.IsEnabled = false;
            }
            else if (_currentService.ServiceId > 0)
            {
                LabelTitle.Content = "Редактирование услуги";
                btnAddService.Content = "📝 Редактировать";
                btnAddService.Background = (SolidColorBrush)new BrushConverter().ConvertFrom("#2563EB");
            }
        }
    }
}

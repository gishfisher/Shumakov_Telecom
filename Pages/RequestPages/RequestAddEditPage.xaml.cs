using Shumakov_DigitalStore.ApplicationData;
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
    /// Логика взаимодействия для RequestAddEditPage.xaml
    /// </summary>
    public partial class RequestAddEditPage : Page
    {
        private Request _currentRequest;
        private readonly TelecomServiceDeskEntities _db;
        private TempStorageService _tempStorage = new TempStorageService();
        private RequestService _requestService = new RequestService();

        public RequestAddEditPage()
        {
            _db = TelecomServiceDeskEntities.GetContext();
        }

        public RequestAddEditPage(Request selectedRequest) : this()
        {
            InitializeComponent();

            if (selectedRequest != null)
            {
                _currentRequest = selectedRequest;

                foreach (var service in _currentRequest.Services)
                {
                    _tempStorage.Add(service);
                }

                DataGridServices.ItemsSource = _tempStorage.Items;
            }
            else
            {
                _currentRequest = new Request();
            }

            DataContext = _currentRequest;

            ComboServices.ItemsSource = _db.Services.ToList();
            ComboClients.ItemsSource = _db.Clients.ToList();
            ComboPriority.ItemsSource = _db.RequestPriorities.ToList();
            ComboEmployees.ItemsSource = _db.Employees.ToList();
            ComboRequestType.ItemsSource = _db.RequestTypes.ToList();

            RefreshUI();
        }

        /*
         * ПОВЕДЕНИЕ ЗАЯВКИ
         */

        // Добавление заявки
        private void AddRequest_Click(object sender, RoutedEventArgs e)
        {
            StringBuilder errors = new StringBuilder();

            if (_currentRequest.Client == null)
                errors.AppendLine("Укажите клиента!");
            if (_currentRequest.RequestPriority == null)
                errors.AppendLine("Уажите приоритет");
            if (_currentRequest.RequestType == null)
                errors.AppendLine("Укажите тип обращения!");
            if (!_tempStorage.Items.Any())
                errors.AppendLine("Укажите хотя-бы одну услугу");
            if (_currentRequest.Description != null && _currentRequest.Description.Length > 500)
                errors.AppendLine("Описание не должно превышать 100 символов!");

            if (errors.Length > 0)
            {
                MessageBox.Show(errors.ToString());
                return;
            }

            try
            {
                if (_currentRequest.RequestId == 0)
                {
                    _requestService.SaveRequest(_currentRequest,
                        _tempStorage.Items.Select(x => x.Service).ToList());

                    MessageBox.Show($"Заявка под №{_currentRequest.RequestId} успешно добавлена.");
                }
                else
                {
                    _requestService.EditRequest(_currentRequest,
                        _tempStorage.Items.Select(x => x.Service).ToList());

                    MessageBox.Show($"Заявка под №{_currentRequest.RequestId} успешно отредактирована.");
                }

                Manager.AppFrame.GoBack();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка при сохранении: " + ex.Message);
            }
        }

        // Добавление услуги в заявку
        private void AddService_Click(object sender, RoutedEventArgs e)
        {
            var selectedService = ComboServices.SelectedItem as Service;

            if (selectedService == null)
            {
                MessageBox.Show("Выберите услугу!");
                return;
            }

            if (!_tempStorage.Add(selectedService))
                MessageBox.Show("Такая услуга уже есть в списке!");

            UpdateList();
        }

        // Удаление услуги из заявки
        private void ButtonDeleteService_Click(object sender, RoutedEventArgs e)
        {
            var serviceForRemoving = (sender as Button).DataContext as TempServiceState;

            if (MessageBox.Show($"Вы уверены что хотите удалить {serviceForRemoving.Name}?", 
                "Внимание", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
            {
                _tempStorage.Remove(serviceForRemoving);
            }

            UpdateList();
        }

        // Возврат назад
        private void GoBack_Click(object sender, RoutedEventArgs e)
        {
            _tempStorage.Clear();
            Manager.AppFrame.GoBack();
        }

        // Изменение цвета приоритета при выборе
        private void ComboPriority_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ChangePriotityColor();
        }

        /*
         * ============== ЖЦ ЗАЯВКИ ==============
         */

        // Назначение
        private void AssignRequest_Click(object sender, RoutedEventArgs e)
        {
            var employee = UserSessionService.CurrentUser.Employees.FirstOrDefault();

            if (MessageBox.Show($"Назначить заявку №{_currentRequest.RequestId}?\nБудет назначен {employee.GetShortNameWithRole}.",
                "Внимание", MessageBoxButton.YesNo) != MessageBoxResult.Yes)
                return;

            if (_requestService.AssignRequest(_currentRequest, employee))
            {
                MessageBox.Show("Заявка назначена!");
                RefreshUI();
            }
            else
            {
                MessageBox.Show("Недопустимый переход статуса!");
            }
        }

        // Отмена
        private void CancelRequest_Click(object sender, RoutedEventArgs e)
        {
            if (MessageBox.Show($"Отменить заявку №{_currentRequest.RequestId}?",
                "Внимание", MessageBoxButton.YesNo) != MessageBoxResult.Yes)
                return;

            if (_requestService.CancelRequest(_currentRequest))
            {
                MessageBox.Show("Заявка отменена!");
                RefreshUI();
            }
            else
            {
                MessageBox.Show("Недопустимый переход статуса!");
            }
        }

        // Отметить как выполненую
        private void MarkRequestComplete_Click(object sender, RoutedEventArgs e)
        {
            if (MessageBox.Show($"Отметить №{_currentRequest.RequestId} как выполненую?",
                "Внимание", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
            {
                if (_requestService.TryChangeStatus(_currentRequest,
                    (int)RequestStatusType.Completed))
                {
                    RefreshUI();
                }
                else
                {
                    MessageBox.Show("Недопустимый переход статуса!");
                }
            }
        }

        // Закрыть заявку
        private void CloseRequest_Click(object sender, RoutedEventArgs e)
        {
            if (MessageBox.Show($"Закрыть заявку №{_currentRequest.RequestId}?",
                "Внимание", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
            {
                if (_requestService.TryChangeStatus(_currentRequest,
                    (int)RequestStatusType.Closed))
                {
                    RefreshUI();
                }
                else
                {
                    MessageBox.Show("Недопустимый переход статуса!");
                }
            }
        }

        // Начать работу
        private void SetInProgressRequest_Click(object sender, RoutedEventArgs e)
        {
            if (MessageBox.Show($"Начать работу?",
                "Внимание", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
            {
                if (_requestService.TryChangeStatus(_currentRequest,
                    (int)RequestStatusType.InProgress))
                {
                    RefreshUI();
                }
                else
                {
                    MessageBox.Show("Недопустимый переход статуса!");
                }
            }
        }

        /*
         * ============== Всячина ==============
         */

        // Обновить UI в зависимости от статуса заявки
        private void RefreshUI()
        {
            if (_currentRequest.StatusId != 0)
            {
                ChangeStatusColor();
                StatusText.Text = _currentRequest.RequestStatus.Name.ToString();
                ComboEmployees.SelectedItem = _currentRequest.Employee;
            }
                ChangeVisibility(_currentRequest);
        }

        // Изменить цвет приоритета
        private void ChangePriotityColor()
        {
            var selectedRequestPriority = ComboPriority.SelectedItem as RequestPriority;
            
            if (selectedRequestPriority != null)
            {
                ComboPriority.Background = (SolidColorBrush)new BrushConverter().ConvertFrom(_currentRequest?.RequestPriority.PriorityColor 
                    ?? selectedRequestPriority.PriorityColor);
            }
        }

        // Изменить цвет статуса
        private void ChangeStatusColor()
        {
            StatusEllipse.Fill = (SolidColorBrush)new BrushConverter().ConvertFrom(_currentRequest.RequestStatus.StatusColor);
            StatusText.Foreground = (SolidColorBrush)new BrushConverter().ConvertFrom(_currentRequest.RequestStatus.StatusColor);
        }

        // Обновить список услуг и общую стоимость
        private void UpdateList()
        {
            DataGridServices.ItemsSource = _tempStorage.Items.ToList();
            TBTotalAmount.Text = _tempStorage.GetTotalAmount().ToString();
        }

        // Изменить UI в зависимости от статуса заявки
        private void ChangeVisibility(Request request)
        {
            AssignRequest.Visibility = Visibility.Collapsed;
            SetInProgressRequest.Visibility = Visibility.Collapsed;
            MarkRequestComplete.Visibility = Visibility.Collapsed;
            CloseRequest.Visibility = Visibility.Collapsed;
            CancelRequest.Visibility = Visibility.Collapsed;
            AddRequest.Visibility = Visibility.Collapsed;
            EditRequest.Visibility = Visibility.Collapsed;

            if (request != null)
            {
                switch (request.StatusId)
                {
                    case (int)RequestStatusType.New:
                        AssignRequest.Visibility = Visibility.Visible;
                        EditRequest.Visibility = Visibility.Visible;
                        CancelRequest.Visibility = Visibility.Visible;
                        break;
                    case (int)RequestStatusType.Assigned:
                        SetInProgressRequest.Visibility = Visibility.Visible;
                        EditRequest.Visibility = Visibility.Visible;
                        CancelRequest.Visibility = Visibility.Visible;
                        break;
                    case (int)RequestStatusType.InProgress:
                        MarkRequestComplete.Visibility = Visibility.Visible;
                        EditRequest.Visibility = Visibility.Visible;
                        CancelRequest.Visibility = Visibility.Visible;
                        break;
                    case (int)RequestStatusType.Completed:
                        CloseRequest.Visibility = Visibility.Visible;
                        break;
                    case (int)RequestStatusType.Cancelled:
                        AssignRequest.Visibility = Visibility.Visible;
                        EditRequest.Visibility = Visibility.Visible;
                        CloseRequest.Visibility = Visibility.Visible;
                        break;
                    case (int)RequestStatusType.Closed:
                        break;
                    default:
                        AddRequest.Visibility = Visibility.Visible;
                        break;
                }
            }
        }
    }
}

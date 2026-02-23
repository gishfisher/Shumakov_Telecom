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
using TelecomCompany.ApplicationData.Crypt;

namespace Telecom.Pages.AuthPages
{
    /// <summary>
    /// Логика взаимодействия для RegPage.xaml
    /// </summary>
    public partial class RegPage : Page
    {
        private Employee _currentEmployee;
        private TelecomServiceDeskEntities _db => TelecomServiceDeskEntities.GetContext();

        public RegPage(Employee selectedEmployee)
        {
            InitializeComponent();

            if (selectedEmployee != null)
            {
                _currentEmployee = selectedEmployee;
            }
            else
            {
                _currentEmployee = new Employee
                {
                    User = new User()
                };
            }

            DataContext = _currentEmployee;
            ComboRoles.ItemsSource = _db.Roles.ToList();
        }

        private void btnCreateAccoount_Click(object sender, RoutedEventArgs e)
        {
            StringBuilder errors = new StringBuilder();

            if (string.IsNullOrWhiteSpace(_currentEmployee.FirstName))
                errors.AppendLine("Укажите имя");
            if (string.IsNullOrWhiteSpace(_currentEmployee.LastName))
                errors.AppendLine("Укажите фамилию");
            if (string.IsNullOrWhiteSpace(_currentEmployee.MiddleName))
                errors.AppendLine("Укажите отчество");
            if (string.IsNullOrWhiteSpace(_currentEmployee.User?.Login))
                errors.AppendLine("Укажите логин");
            if (string.IsNullOrWhiteSpace(psbPassword.Password) && _currentEmployee.User.Password_Hash == null)
                errors.AppendLine("Укажите пароль");
            if (_currentEmployee.User.Role == null)
                errors.AppendLine("Выберите роль");
            if (psbPassword.Password != psbPasswordConfirm.Password)
                errors.AppendLine("Пароли не совпадают");
            if (_db.Users.Any(x =>
                x.Login == _currentEmployee.User.Login
                && x.UserId != _currentEmployee.User.UserId))
            {
                errors.AppendLine("Имя пользователя занято");
            }

            if (errors.Length > 0)
            {
                MessageBox.Show(errors.ToString());
                return;
            }
           
            try
            {
                RegService regService = new RegService();

                if (_currentEmployee.EmployeeId == 0)
                {
                    if (regService.RegisterEmployee(_currentEmployee, psbPassword.Password))
                    {
                        MessageBox.Show($"Сотрудник {_currentEmployee.GetShortName} в должности " +
                            $"{_currentEmployee.User.Role.Name}, успешно добавлен!");
                        Manager.AppFrame.GoBack();
                    }
                }
                else
                {
                    if (regService.EditEmployee(_currentEmployee, psbPassword.Password))
                    {
                        MessageBox.Show($"Сотрудник {_currentEmployee.GetShortName} в должности " +
                            $"{_currentEmployee.User.Role.Name}, успешно отредактирован!");
                        Manager.AppFrame.GoBack();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка: {ex.Message.ToString()}");
            }
        }

        private void psbPasswordConfirm_PasswordChanged(object sender, RoutedEventArgs e)
        {
            CheckPassword();
        }

        private void psbPassword_PasswordChanged(object sender, RoutedEventArgs e)
        {
            CheckPassword();
        }

        private void CheckPassword()
        {
            if (psbPassword.Password != psbPasswordConfirm.Password ||
                (string.IsNullOrWhiteSpace(psbPassword.Password) || string.IsNullOrWhiteSpace(psbPasswordConfirm.Password)))
            {
                psbPassword.BorderBrush = Brushes.Red;
                psbPasswordConfirm.BorderBrush = Brushes.Red;
            }
            else
            {
                psbPassword.BorderBrush = Brushes.LightGreen;
                psbPasswordConfirm.BorderBrush = Brushes.LightGreen;
            }
        }

        private void GoBack_Click(object sender, RoutedEventArgs e)
        {
            Manager.AppFrame.GoBack();
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            if (UserSessionService.IsAdmin && _currentEmployee.EmployeeId > 0)
            {
                LabelTitle.Content = "Редактирование сотрудника";
                btnCreateAccoount.Content = "📝 Редактировать";
                btnCreateAccoount.Background = (SolidColorBrush)new BrushConverter().ConvertFrom("#2563EB");
            }
        }
    }
}

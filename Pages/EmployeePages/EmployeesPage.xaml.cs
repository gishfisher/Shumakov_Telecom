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
using Telecom.Pages.AuthPages;
using Telecom.Services;
using Telecom.Utility;

namespace Telecom.Pages.EmployeePages
{
    /// <summary>
    /// Логика взаимодействия для EmployeesPage.xaml
    /// </summary>
    public partial class EmployeesPage : Page
    {
        private TelecomServiceDeskEntities _db = TelecomServiceDeskEntities.GetContext();
        private RegService _regService = new RegService();

        public EmployeesPage()
        {
            InitializeComponent();

            var allRoles = _db.Roles.ToList();

            allRoles.Insert(0, new Role
            {
                Name = "Все роли"
            });
            ComboRole.ItemsSource = allRoles;
            ComboRole.SelectedIndex = 0;

            DataGridEmployees.ItemsSource = _db.Employees.ToList();
        }
        private void ButtonAdd_Click(object sender, RoutedEventArgs e)
        {
            Manager.AppFrame.Navigate(new RegPage(null));
        }

        private void ButtonEdit_Click(object sender, RoutedEventArgs e)
        {
            Manager.AppFrame.Navigate(new RegPage((sender as Button).DataContext as Employee));
        }

        private void ButtonDelete_Click(object sender, RoutedEventArgs e)
        {
            var employeeForDelete = (sender as Button).DataContext as Employee;

            if (MessageBox.Show($"Вы уверены что хоите удалить сотрудника {employeeForDelete.GetFullName}, " +
                $"в роли {employeeForDelete.User.Role.Name}?",
                "Внимание", MessageBoxButton.YesNo) != MessageBoxResult.Yes)
                return;

            if(_regService.DeleteEmployee(employeeForDelete))
                MessageBox.Show($"Сотрудник успешно удален");

            DataGridEmployees.ItemsSource = _db.Employees.ToList();
        }

        private void SearchBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            UpdateEmployee();
        }

        private void ComboRole_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            UpdateEmployee();
        }

        private void UpdateEmployee()
        {
            var currentRequests = _db.Employees.ToList();

            if (ComboRole.SelectedIndex > 0)
                currentRequests = currentRequests.Where(x => x.User.Role == (ComboRole.SelectedItem as Role)).ToList();

            currentRequests = currentRequests.Where(x => x.GetFullName.ToLower().Contains(SearchBox.Text.ToLower())).ToList();

            //if (CheckClosed.IsChecked.Value)
            //    currentRequests = currentRequests.Where(x => x.StatusId != (int)RequestStatusType.Closed).ToList();

            DataGridEmployees.ItemsSource = currentRequests.OrderBy(x => x.EmployeeId).ToList();
        }

        private void Page_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (Visibility == Visibility.Visible)
            {
                _db.ChangeTracker.Entries().ToList().ForEach(p => p.Reload());

                DataGridEmployees.ItemsSource = _db.Employees.ToList();
            }
        }
    }
}

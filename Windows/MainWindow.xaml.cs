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
using Telecom.Pages;
using Telecom.Services;
using Telecom.Utility;
using TelecomCompany.Pages;

namespace Telecom
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            Manager.AppFrame = MainFrame;
            MainFrame.Navigate(new AuthPage());
        }

        private void MainFrame_ContentRendered(object sender, EventArgs e)
        {
            if (!UserSessionService.IsAuthenticated)
            {
                ButtonsPanel.Visibility = Visibility.Collapsed;
                HelloPanel.Visibility = Visibility.Collapsed;
                return;
            }

            TextBlockCurrentUser.Text =
                $"👤 {UserSessionService.CurrentUser.Employees.FirstOrDefault()?.GetShortName} " +
                $"[{UserSessionService.CurrentUser.Login}]\n" +
                $"{UserSessionService.CurrentUser.Role.Name ?? "Гость"}";

            HelloPanel.Visibility = Visibility.Visible;
            ButtonsPanel.Visibility = Visibility.Visible;
        }

        private void ButtonLogout_Click(object sender, RoutedEventArgs e)
        {

            if (MessageBox.Show("Вы действительно хотите выйти из аккаунта?", "Подтверждение выхода",
                MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.No)
                return;

            if (UserSessionService.IsAuthenticated)
                UserSessionService.Logout();

            Manager.AppFrame.Navigate(new AuthPage());
        }
    }
}

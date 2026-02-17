using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
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
using TelecomCompany.ApplicationData.Crypt;

namespace TelecomCompany.Pages
{
    /// <summary>
    /// Логика взаимодействия для AuthPage.xaml
    /// </summary>
    public partial class AuthPage : Page
    {
        public AuthPage()
        {
            InitializeComponent();
        }

        private void Login_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(TBLogin.Text) || string.IsNullOrWhiteSpace(PBPassword.Password))
            {
                MessageBox.Show("Укажите логин или пароль", "Внимание",
                    MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            AuthService authService = new AuthService();

            string hashedPassword = MD5Hasher.HashPassword(PBPassword.Password);
            var authUser = authService.Authenticate(TBLogin.Text, hashedPassword);

            if (authUser == null)
            {
                MessageBox.Show("Неверный логин или пароль", "Внимание",
                    MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            string roleName = authUser.Role?.Name ?? "Пользователь";
            string fullName = authUser.Employees?.FirstOrDefault()?.GetFullName ?? string.Empty;

            MessageBox.Show($"Добро пожаловать, {roleName} {fullName}",
                "Добро пожаловать!", MessageBoxButton.OK, MessageBoxImage.Information);
            
            UserSessionService.Login(authUser);
            Manager.AppFrame.Navigate(new RequestPage());
        }

        private void Registration_Click(object sender, RoutedEventArgs e)
        {
            //Manager.AppFrame.Navigate(new RegPage());
        }
    }
}

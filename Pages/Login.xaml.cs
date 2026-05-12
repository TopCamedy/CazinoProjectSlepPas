using CazinoProjectSlepPas;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;

namespace CazinoProjectSlepPas.Pages
{
    public partial class LoginPage : Page
    {
        public LoginPage()
        {
            InitializeComponent();
            
        }

        private void RegisterLink_Click(object sender, RoutedEventArgs e)
        {
            NavigationService?.Navigate(new RegisterPage());
        }

        private void LoginBtn_Click(object sender, RoutedEventArgs e)
        {
            //NavigationService?.Navigate(new MainNavBar());

            string username = UsernameInput.Text.Trim();
            string password = PasswordInput.Password;

            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
            {
                MessageBox.Show("Введите логин и пароль", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            if (UserDatabase.CheckPassword(username, password))
            {
                MainNavBar.Account = UserDatabase.GetUser(username);
                MessageBox.Show($"Вход выполнен успешно!\n",
                                "Успешный вход",
                                MessageBoxButton.OK,
                                MessageBoxImage.Information);

                NavigationService?.Navigate(new MainNavBar());
            }
            else
            {
                MessageBox.Show("Неверный логин или пароль", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
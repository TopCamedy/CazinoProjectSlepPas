
using System;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Navigation;

namespace CazinoProjectSlepPas.Pages
{
    public partial class RegisterPage : Page
    {
        private bool _isPasswordVisible = false;
        private bool _isConfirmPasswordVisible = false;
        private TextBox _passwordTextBox;
        private TextBox _confirmPasswordTextBox;

        public RegisterPage()
        {
            InitializeComponent();
            BirthDateInput.SelectedDate = DateTime.Now.AddYears(-18);
        }

        private void ShowPasswordBtn_Click(object sender, RoutedEventArgs e)
        {
            TogglePasswordVisibility(ref _isPasswordVisible, PasswordInput, ref _passwordTextBox, ShowPasswordBtn);
        }

        private void ShowConfirmPasswordBtn_Click(object sender, RoutedEventArgs e)
        {
            TogglePasswordVisibility(ref _isConfirmPasswordVisible, ConfirmPasswordInput, ref _confirmPasswordTextBox, ShowConfirmPasswordBtn);
        }

        private void TogglePasswordVisibility(ref bool isVisible, PasswordBox passwordBox, ref TextBox textBox, Button button)
        {
            isVisible = !isVisible;

            if (isVisible)
            {
                textBox = new TextBox
                {
                    Text = passwordBox.Password,
                    Margin = passwordBox.Margin,
                    VerticalContentAlignment = VerticalAlignment.Center,
                    Padding = new Thickness(5),
                    Background = Brushes.Transparent,
                    Foreground = Brushes.White,
                    BorderBrush = Brushes.Transparent
                };

                var parent = passwordBox.Parent as Grid;
                parent.Children.Remove(passwordBox);
                Grid.SetColumn(textBox, 0);
                parent.Children.Add(textBox);

                button.Content = "✖";
            }
            else
            {
                passwordBox.Password = textBox.Text;

                var parent = textBox.Parent as Grid;
                parent.Children.Remove(textBox);
                Grid.SetColumn(passwordBox, 0);
                parent.Children.Add(passwordBox);

                button.Content = "👁";
            }
        }

        private void PasswordInput_PasswordChanged(object sender, RoutedEventArgs e)
        {
            UpdatePasswordStrength();
        }

        private void ConfirmPasswordInput_PasswordChanged(object sender, RoutedEventArgs e)
        {
            // Проверка совпадения паролей
        }

        private void UpdatePasswordStrength()
        {
            string password = _isPasswordVisible ? _passwordTextBox?.Text : PasswordInput.Password;

            if (string.IsNullOrEmpty(password))
            {
                PasswordStrengthText.Text = "не задан";
                PasswordStrengthText.Foreground = Brushes.Gray;
                return;
            }

            int strength = 0;
            var sb = new StringBuilder();

            if (password.Length >= 8) strength++;
            if (Regex.IsMatch(password, "[A-Z]")) strength++;
            if (Regex.IsMatch(password, "[a-z]")) strength++;
            if (Regex.IsMatch(password, "[0-9]")) strength++;
            if (Regex.IsMatch(password, "[^a-zA-Z0-9]")) strength++;

            switch (strength)
            {
                case 0:
                case 1:
                    sb.Append("очень слабый");
                    PasswordStrengthText.Foreground = Brushes.Red;
                    break;
                case 2:
                    sb.Append("слабый");
                    PasswordStrengthText.Foreground = Brushes.Orange;
                    break;
                case 3:
                    sb.Append("средний");
                    PasswordStrengthText.Foreground = Brushes.YellowGreen;
                    break;
                case 4:
                    sb.Append("сильный");
                    PasswordStrengthText.Foreground = Brushes.Green;
                    break;
                case 5:
                    sb.Append("очень сильный");
                    PasswordStrengthText.Foreground = Brushes.DarkGreen;
                    break;
            }

            PasswordStrengthText.Text = sb.ToString();
        }

        private void RegisterBtn_Click(object sender, RoutedEventArgs e)
        {
            ErrorMessage.Visibility = Visibility.Collapsed;

            string login = LoginInput.Text.Trim();
            string password = _isPasswordVisible ? _passwordTextBox.Text : PasswordInput.Password;
            string confirmPassword = _isConfirmPasswordVisible ? _confirmPasswordTextBox.Text : ConfirmPasswordInput.Password;
            DateTime? birthDate = BirthDateInput.SelectedDate;

            // Валидация
            if (string.IsNullOrEmpty(login))
            {
                ShowError("Введите логин!");
                return;
            }

            if (login.Length < 4)
            {
                ShowError("Логин должен содержать минимум 4 символа!");
                return;
            }

            if (!Regex.IsMatch(login, @"^[a-zA-Z0-9_]+$"))
            {
                ShowError("Логин может содержать только буквы, цифры и символ подчеркивания!");
                return;
            }

            if (string.IsNullOrEmpty(password))
            {
                ShowError("Введите пароль!");
                return;
            }

            if (password.Length < 6)
            {
                ShowError("Пароль должен содержать минимум 6 символов!");
                return;
            }

            if (password != confirmPassword)
            {
                ShowError("Пароли не совпадают!");
                return;
            }

            if (!birthDate.HasValue)
            {
                ShowError("Укажите дату рождения!");
                return;
            }

            int age = DateTime.Now.Year - birthDate.Value.Year;
            if (birthDate.Value.Date > DateTime.Now.AddYears(-age)) age--;

            if (age < 13)
            {
                ShowError("Регистрация доступна только для пользователей старше 13 лет!");
                return;
            }

            if (UserDatabase.UserExists(login))
            {
                ShowError("Пользователь с таким логином уже существует!");
                return;
            }

            UserDatabase.AddUser(login, password, birthDate.Value);

            MessageBox.Show(
                $"Регистрация успешно завершена!\n\nЛогин: {login}\nДата рождения: {birthDate.Value.ToShortDateString()}\n",
                "Успешная регистрация",
                MessageBoxButton.OK,
                MessageBoxImage.Information);

            NavigationService?.Navigate(new LoginPage());
        }

        private void LoginLink_Click(object sender, RoutedEventArgs e)
        {
            NavigationService?.Navigate(new LoginPage());
        }

        private void ShowError(string message)
        {
            ErrorMessage.Text = message;
            ErrorMessage.Visibility = Visibility.Visible;
        }

        public void ResetForm()
        {
            LoginInput.Text = "";
            PasswordInput.Password = "";
            ConfirmPasswordInput.Password = "";

            if (_isPasswordVisible)
            {
                _passwordTextBox.Text = "";
                ShowPasswordBtn_Click(ShowPasswordBtn, null);
            }

            if (_isConfirmPasswordVisible)
            {
                _confirmPasswordTextBox.Text = "";
                ShowConfirmPasswordBtn_Click(ShowConfirmPasswordBtn, null);
            }

            BirthDateInput.SelectedDate = DateTime.Now.AddYears(-18);
            PasswordStrengthText.Text = "";
            ErrorMessage.Visibility = Visibility.Collapsed;
        }
    }
}
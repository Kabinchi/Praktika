using System;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Airport.Models;

namespace Airport
{
    public partial class UserAddWindow : Window
    {
        private MyDbContext _context;
        private UsersWindow _parentWindow;

        public UserAddWindow(UsersWindow parentWindow)
        {
            InitializeComponent();
            _context = new MyDbContext();
            _parentWindow = parentWindow;
        }

        private void AddButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(NameTextBox.Text) ||
                    string.IsNullOrWhiteSpace(LoginTextBox.Text) ||
                    string.IsNullOrWhiteSpace(PasswordBox.Password) ||
                    RoleComboBox.SelectedItem == null)
                {
                    MessageBox.Show("Пожалуйста, заполните все поля.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                if (!ValidatePassword(PasswordBox.Password))
                {
                    MessageBox.Show("Пароль должен содержать минимум 6 символов, одну прописную букву, одну цифру и один символ из набора: ! @ # $ % ^.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                var user = new User
                {
                    Name = NameTextBox.Text,
                    Login = LoginTextBox.Text,
                    Password = PasswordBox.Password,
                    Role = ((ComboBoxItem)RoleComboBox.SelectedItem).Content.ToString()
                };

                _context.Users.Add(user);
                _context.SaveChanges();

                MessageBox.Show("Пользователь успешно добавлен.", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
                _parentWindow.LoadUsers();
                DialogResult = true;
                Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при добавлении пользователя: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private bool ValidatePassword(string password)
        {
            if (password.Length < 6)
                return false;
            if (!Regex.IsMatch(password, @"[A-Z]"))
                return false;
            if (!Regex.IsMatch(password, @"[0-9]"))
                return false;
            if (!Regex.IsMatch(password, @"[!@#\$%\^]"))
                return false;

            return true;
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }
        private void TextValidationTextBox(object sender, TextChangedEventArgs e)
        {
            TextBox textBox = sender as TextBox;
            if (!IsTextAllowed(textBox.Text, "^[a-zA-Zа-яА-Я]*$"))
            {
                MessageBox.Show("Пожалуйста, вводите только буквы.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                textBox.Text = Regex.Replace(textBox.Text, "[^a-zA-Zа-яА-Я]", "");
                textBox.SelectionStart = textBox.Text.Length;
            }
        }

        private static bool IsTextAllowed(string text, string pattern)
        {
            return Regex.IsMatch(text, pattern);
        }
    }
}

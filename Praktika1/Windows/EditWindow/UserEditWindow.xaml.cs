using System;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Microsoft.EntityFrameworkCore;
using Airport.Models;

namespace Airport
{
    public partial class UserEditWindow : Window
    {
        private MyDbContext _context;
        private User _user;
        private UsersWindow _parentWindow;

        public UserEditWindow(User user, UsersWindow parentWindow)
        {
            InitializeComponent();
            _context = new MyDbContext();
            _user = user;
            _parentWindow = parentWindow;

            NameTextBox.Text = _user.Name;
            LoginTextBox.Text = _user.Login;
            PasswordBox.Password = _user.Password;
            RoleComboBox.SelectedItem = RoleComboBox.Items.OfType<ComboBoxItem>().FirstOrDefault(i => i.Content.ToString() == _user.Role);
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
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

                _user.Name = NameTextBox.Text;
                _user.Login = LoginTextBox.Text;
                _user.Password = PasswordBox.Password;
                _user.Role = ((ComboBoxItem)RoleComboBox.SelectedItem).Content.ToString();

                _context.Entry(_user).State = EntityState.Modified;
                _context.SaveChanges();

                MessageBox.Show("Пользователь успешно обновлен.", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
                _parentWindow.LoadUsers();
                DialogResult = true;
                Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при обновлении пользователя: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
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

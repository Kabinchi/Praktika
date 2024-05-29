using Airport.Models;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Input;

namespace Airport
{
    public partial class LoginWindow : Window
    {
        private MyDbContext _dbContext;

        public LoginWindow()
        {
            InitializeComponent();
            _dbContext = new MyDbContext();
        }

        private void LoginButton_Click(object sender, RoutedEventArgs e)
        {
            string login = LoginTextBox.Text;
            string password = PasswordBox.Password;

            if (string.IsNullOrWhiteSpace(login) || string.IsNullOrWhiteSpace(password))
            {
                MessageBox.Show("Введите логин и пароль", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            var user = _dbContext.Users.FirstOrDefault(u => u.Login == login && u.Password == password);

            if (user != null)
            {
                CompaniesWindow companiesWindow = new CompaniesWindow(user);
                companiesWindow.Show();
                Close();
            }
            else
            {
                MessageBox.Show("Неверный логин или пароль", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void TextValidationTextBox(object sender, TextCompositionEventArgs e)
            {
                e.Handled = !IsTextAllowed(e.Text, "^[a-zA-Zа-яА-Я]+$");
            }

            private static bool IsTextAllowed(string text, string pattern)
            {
                return Regex.IsMatch(text, pattern);
            }
        }
    }


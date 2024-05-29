using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using Microsoft.EntityFrameworkCore;
using Airport.Models;
using Airport.Other;

namespace Airport
{
    public partial class UsersWindow : Window
    {
        private MyDbContext _context;
        private User _user; 

        public UsersWindow(User user)
        {
            InitializeComponent();
            _context = new MyDbContext();
            _user = user; 
            LoadUsers();
        }

        public void LoadUsers()
        {
            UsersDataGrid.ItemsSource = _context.Users.ToList();
        }

        private void AddButton_Click(object sender, RoutedEventArgs e)
        {
            if (_user.Role != "Admin")
            {
                MessageBox.Show("У вас нет прав для добавления пользователей.", "Ограничение доступа", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var addUserWindow = new UserAddWindow(this);
            addUserWindow.ShowDialog();
        }

        private void EditUserButton_Click(object sender, RoutedEventArgs e)
        {
            if (_user.Role != "Admin")
            {
                MessageBox.Show("У вас нет прав для редактирования пользователей.", "Ограничение доступа", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (UsersDataGrid.SelectedItem is User selectedUser)
            {
                var editUserWindow = new UserEditWindow(selectedUser, this);
                editUserWindow.ShowDialog();
            }
            else
            {
                MessageBox.Show("Пожалуйста, выберите пользователя для редактирования.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        private void EditRow_Click(object sender, RoutedEventArgs e)
        {
            if (_user.Role != "Admin")
            {
                MessageBox.Show("У вас нет прав для редактирования пользователей.", "Ограничение доступа", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var button = sender as Button;
            if (button != null)
            {
                int userId = (int)button.Tag;
                var user = _context.Users.FirstOrDefault(u => u.Users_ID == userId);
                if (user != null)
                {
                    _context.Attach(user);
                    UserEditWindow editWindow = new UserEditWindow(user, this);
                    if (editWindow.ShowDialog() == true)
                    {
                        LoadUsers();
                    }
                }
            }
        }

        private void DeleteRow_Click(object sender, RoutedEventArgs e)
        {
            if (_user.Role != "Admin")
            {
                MessageBox.Show("У вас нет прав для удаления пользователей.", "Ограничение доступа", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var button = sender as Button;
            if (button != null)
            {
                int userId = (int)button.Tag;
                DataOperations.DeleteRow<User>(_context, userId, LoadUsers);
            }
        }

        private void SearchButton_Click(object sender, RoutedEventArgs e)
        {
            var searchText = SearchTextBox.Text.ToLower();
            var selectedItem = ComboBoxColumn.SelectedItem as ComboBoxItem;

            if (selectedItem == null)
            {
                MessageBox.Show("Пожалуйста, выберите столбец для фильтрации.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var selectedColumn = selectedItem.Content.ToString();

            var users = _context.Users.ToList();

            switch (selectedColumn)
            {
                case "Имя":
                    users = users.Where(u => u.Name.ToLower().Contains(searchText)).ToList();
                    break;
                case "Логин":
                    users = users.Where(u => u.Login.ToLower().Contains(searchText)).ToList();
                    break;
                case "Пароль":
                    users = users.Where(u => u.Password.ToLower().Contains(searchText)).ToList();
                    break;
                case "Роль":
                    users = users.Where(u => u.Role.ToLower().Contains(searchText)).ToList();
                    break;
            }

            UsersDataGrid.ItemsSource = users;
        }

        private void LogoutButton_Click(object sender, RoutedEventArgs e)
        {
            LoginWindow loginWindow = new LoginWindow();
            loginWindow.Show();
            Close();
        }

        private void CompaniesButton_Click(object sender, RoutedEventArgs e)
        {
            CompaniesWindow companiesWindow = new CompaniesWindow(_user);
            companiesWindow.Show();
            Close();
        }

        private void PlanesButton_Click(object sender, RoutedEventArgs e)
        {
            AirplanesWindow planesWindow = new AirplanesWindow(_user);
            planesWindow.Show();
            Close();
        }

        private void CrewButton_Click(object sender, RoutedEventArgs e)
        {
            CrewMembersWindow crewmembers = new CrewMembersWindow(_user);
            crewmembers.Show();
            Close();
        }

        private void PassengersButton_Click(object sender, RoutedEventArgs e)
        {
            PassengersWindow passengersWindow = new PassengersWindow(_user);
            passengersWindow.Show();
            Close();
        }

        private void TicketsButton_Click(object sender, RoutedEventArgs e)
        {
            TicketsWindow ticketsWindow = new TicketsWindow(_user);
            ticketsWindow.Show();
            Close();
        }

        private void FlightsButton_Click(object sender, RoutedEventArgs e)
        {
            FlightsWindow ticketsWindow = new FlightsWindow(_user);
            ticketsWindow.Show();
            Close();
        }
    }
}

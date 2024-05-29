using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using DocumentFormat.OpenXml.Spreadsheet;
using Microsoft.EntityFrameworkCore;
using Airport.Models;

namespace Airport
{
    public partial class PassengersWindow : Window
    {
        private MyDbContext _context;
        private User _user; 

        public PassengersWindow(User user)
        {
            InitializeComponent();
            _context = new MyDbContext();
            _user = user; 
            LoadPassengers();
        }

        public void LoadPassengers() 
        {
            var passengers = _context.Passengers.Include(p => p.Airplane).ToList();
            PassengersDataGrid.ItemsSource = passengers;
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
            IQueryable<Passenger> query = _context.Passengers.Include(p => p.Airplane);

            switch (selectedColumn)
            {
                case "Фамилия":
                    query = query.Where(p => p.Last_Name.ToLower().Contains(searchText));
                    break;
                case "Имя":
                    query = query.Where(p => p.First_Name.ToLower().Contains(searchText));
                    break;
                case "Отчество":
                    query = query.Where(p => p.Middle_Name.ToLower().Contains(searchText));
                    break;
                case "Точка отправления":
                    query = query.Where(p => p.Departure_Point.ToLower().Contains(searchText));
                    break;
                case "Точка прибытия":
                    query = query.Where(p => p.Destination.ToLower().Contains(searchText));
                    break;
                case "Дата отправления":
                    if (DateTime.TryParse(searchText, out DateTime searchDate))
                    {
                        query = query.Where(p => p.Departure_Date.Date == searchDate.Date);
                    }
                    break;
                case "Самолёт":
                    query = query.Where(p => p.Airplane.Board_Number.ToLower().Contains(searchText));
                    break;
            }

            PassengersDataGrid.ItemsSource = query.ToList();
        }

        private void AddButton_Click(object sender, RoutedEventArgs e)
        {
            if (_user.Role != "Admin")
            {
                MessageBox.Show("У вас нет прав для добавления пассажиров.", "Ограничение доступа", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var addWindow = new PassengersAddWindow(this);
            addWindow.ShowDialog();
        }

        private void EditRow_Click(object sender, RoutedEventArgs e)
        {
            if (_user.Role != "Admin")
            {
                MessageBox.Show("У вас нет прав для редактирования пассажиров.", "Ограничение доступа", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var button = sender as Button;
            if (button != null)
            {
                var passengerId = (int)button.Tag;
                var passenger = _context.Passengers.Include(p => p.Airplane).FirstOrDefault(p => p.Passenger_ID == passengerId);
                if (passenger != null)
                {
                    var editWindow = new PassengersEditWindow(passenger, this);
                    editWindow.ShowDialog();
                }
            }
        }

        private void DeleteRow_Click(object sender, RoutedEventArgs e)
        {
            if (_user.Role != "Admin")
            {
                MessageBox.Show("У вас нет прав для удаления пассажиров.", "Ограничение доступа", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var button = sender as Button;
            if (button != null)
            {
                var passengerId = (int)button.Tag;
                using (var context = new MyDbContext())
                {
                    var passenger = context.Passengers.FirstOrDefault(p => p.Passenger_ID == passengerId);

                    if (passenger != null)
                    {
                        var hasDependencies = context.Tickets.Any(t => t.Passenger_ID == passengerId);

                        if (hasDependencies)
                        {
                            MessageBox.Show("На эту запись ссылаются другие записи. Удаление невозможно.", "Ошибка удаления", MessageBoxButton.OK, MessageBoxImage.Error);
                            return;
                        }

                        var result = MessageBox.Show("Вы уверены, что хотите удалить эту запись?", "Подтверждение удаления", MessageBoxButton.YesNo, MessageBoxImage.Warning);
                        if (result == MessageBoxResult.Yes)
                        {
                            try
                            {
                                context.Database.ExecuteSqlRaw("DELETE FROM Passengers WHERE Passenger_ID = {0}", passengerId);
                                LoadPassengers();
                            }
                            catch (Exception ex)
                            {
                                MessageBox.Show("Ошибка при удалении записи: " + ex.Message, "Ошибка удаления", MessageBoxButton.OK, MessageBoxImage.Error);
                            }
                        }
                    }
                }
            }
        }

        private void CompaniesButton_Click(object sender, RoutedEventArgs e)
        {
            var companiesWindow = new CompaniesWindow(_user);
            companiesWindow.Show();
            Close();
        }

        private void PlanesButton_Click(object sender, RoutedEventArgs e)
        {
            var airplanesWindow = new AirplanesWindow(_user);
            airplanesWindow.Show();
            Close();
        }

        private void FlightsButton_Click(object sender, RoutedEventArgs e)
        {
            var flightsWindow = new FlightsWindow(_user);
            flightsWindow.Show();
            Close();
        }

        private void CrewButton_Click(object sender, RoutedEventArgs e)
        {
            var crewmembersWindow = new CrewMembersWindow(_user);
            crewmembersWindow.Show();
            Close();
        }

        private void TicketsButton_Click(object sender, RoutedEventArgs e)
        {
            var ticketsWindow = new TicketsWindow(_user);
            ticketsWindow.Show();
            Close();
        }

        private void UsersButton_Click(object sender, RoutedEventArgs e)
        {
            if (_user.Role == "Admin")
            {
                UsersWindow usersWindow = new UsersWindow(_user);
                usersWindow.Show();
                Close();
            }
            else
            {
                MessageBox.Show("У вас нет прав для просмотра пользователей.", "Ограничение доступа", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void LogoutButton_Click(object sender, RoutedEventArgs e)
        {
            LoginWindow loginWindow = new LoginWindow();
            loginWindow.Show();
            Close();
        }


    }
}

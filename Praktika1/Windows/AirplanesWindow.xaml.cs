using System.Linq;
using Microsoft.EntityFrameworkCore;
using Airport.Models;
using System.Windows;
using System.Windows.Controls;
using DocumentFormat.OpenXml.Spreadsheet;
using Airport.Other;

namespace Airport
{
    public partial class AirplanesWindow : Window
    {

        private MyDbContext _context;
        private User _user; 

        public AirplanesWindow(User user)
        {
            InitializeComponent();
            _context = new MyDbContext();
            _user = user;  
            LoadAirplanesData();
        }

        public void LoadAirplanesData()
        {
            using (var context = new MyDbContext())
            {
                var airplanes = context.Airplanes.Include(a => a.Airline).ToList();
                AirplanesDataGrid.ItemsSource = airplanes;
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


            using (var context = new MyDbContext())
            {
                IQueryable<Airplane> query = context.Airplanes.Include(a => a.Airline);

                switch (selectedColumn)
                {
                    case "Количество мест":
                        query = query.Where(a => a.Seat_Count.ToString().Contains(searchText));
                        break;
                    case "Номер самолёта":
                        query = query.Where(a => a.Board_Number.ToLower().Contains(searchText));
                        break;
                    case "Дальность перелёта":
                        query = query.Where(a => a.Flight_Range.ToString().Contains(searchText));
                        break;
                    case "Стоимость билета":
                        query = query.Where(a => a.Ticket_Price_Class.ToString().Contains(searchText));
                        break;
                    case "Авиакомпания":
                        query = query.Where(a => a.Airline.Airline_Name.ToLower().Contains(searchText));
                        break;
                }

                AirplanesDataGrid.ItemsSource = query.ToList();
            }
        }

        private void AddButton_Click(object sender, RoutedEventArgs e)
        {
            if (_user.Role != "Admin")
            {
                MessageBox.Show("У вас нет прав для добавления записей.", "Ограничение доступа", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            var addWindow = new AirplanesAddWindow(this);
            addWindow.ShowDialog();
        }


        private void EditRow_Click(object sender, RoutedEventArgs e)
        {
            if (_user.Role != "Admin")
            {
                MessageBox.Show("У вас нет прав для редактирования записей.", "Ограничение доступа", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var button = sender as Button;
            if (button != null)
            {
                var airplaneId = (int)button.Tag;
                using (var context = new MyDbContext())
                {
                    var airplane = context.Airplanes.FirstOrDefault(a => a.Airplane_ID == airplaneId);
                    if (airplane != null)
                    {
                        var editWindow = new AirplanesEditWindow(airplane);
                        if (editWindow.ShowDialog() == true)
                        {
                            context.Airplanes.Update(editWindow.Airplane);
                            context.SaveChanges();
                            LoadAirplanesData();
                        }
                    }
                }
            }
        }
        private void DeleteRow_Click(object sender, RoutedEventArgs e)
        {
            if (_user.Role != "Admin")
            {
                MessageBox.Show("У вас нет прав для удаления записей.", "Ограничение доступа", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var button = sender as Button;
            if (button != null)
            {
                var airplaneId = (int)button.Tag;
                DataOperations.DeleteRow<Airplane>(_context, airplaneId, () =>
                {
                    LoadAirplanesData();
                });
            }
        }



        private void AirlinesButton_Click(object sender, RoutedEventArgs e)
        {
            var companiesWindow = new CompaniesWindow(_user);
            companiesWindow.Show();
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
            var crewMembersWindow = new CrewMembersWindow(_user);
            crewMembersWindow.Show();
            Close();
        }

        private void PassengersButton_Click(object sender, RoutedEventArgs e)
        {
            var passengersWindow = new PassengersWindow(_user);
            passengersWindow.Show();
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

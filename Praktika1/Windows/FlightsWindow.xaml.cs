using System.Linq;
using System.Windows;
using Microsoft.EntityFrameworkCore;
using System.Windows.Controls;
using Airport.Models;
using ClosedXML.Excel;
using Microsoft.Win32;
using Airport.Other;

namespace Airport
{
    public partial class FlightsWindow : Window
    {
        private MyDbContext _context;
        private User _user;  

        public FlightsWindow(User user)
        {
            InitializeComponent();
            _context = new MyDbContext();
            _user = user;  
            LoadFlights();
        }

        public void LoadFlights()
        {
            using (var context = new MyDbContext())
            {
                FlightsDataGrid.ItemsSource = context.Flights
                    .Include(f => f.Airline)
                    .Include(f => f.Airplane)
                    .ToList();
            }
        }

        private void AddButton_Click(object sender, RoutedEventArgs e)
        {
            if (_user.Role != "Admin")
            {
                MessageBox.Show("У вас нет прав для добавления рейсов.", "Ограничение доступа", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var addWindow = new FlightsAddWindow(this);
            addWindow.ShowDialog();
        }

        private void EditRow_Click(object sender, RoutedEventArgs e)
        {
            if (_user.Role != "Admin")
            {
                MessageBox.Show("У вас нет прав для редактирования рейсов.", "Ограничение доступа", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var flightId = (int)((Button)sender).Tag;
            using (var context = new MyDbContext())
            {
                var flight = context.Flights
                    .Include(f => f.Airline)
                    .Include(f => f.Airplane)
                    .FirstOrDefault(f => f.Flight_ID == flightId);
                if (flight != null)
                {
                    var editWindow = new FlightsEditWindow(flight);
                    if (editWindow.ShowDialog() == true)
                    {
                        LoadFlights();
                    }
                }
            }
        }

        private void DeleteRow_Click(object sender, RoutedEventArgs e)
        {
            if (_user.Role != "Admin")
            {
                MessageBox.Show("У вас нет прав для удаления рейсов.", "Ограничение доступа", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var flightId = (int)((Button)sender).Tag;
            DataOperations.DeleteRow<Flight>(_context, flightId, () =>
            {
                LoadFlights();
            });
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
                IQueryable<Flight> query = context.Flights.Include(f => f.Airline).Include(f => f.Airplane);

                switch (selectedColumn)
                {
                    case "Время вылета":
                        query = query.Where(f => f.Departure_Time.ToString().Contains(searchText));
                        break;
                    case "Время прибытия":
                        query = query.Where(f => f.Arrival_Date_Time.ToString().Contains(searchText));
                        break;
                    case "Авиакомпания":
                        query = query.Where(f => f.Airline.Airline_Name.ToLower().Contains(searchText));
                        break;
                    case "Самолёт":
                        query = query.Where(f => f.Airplane.Board_Number.ToLower().Contains(searchText));
                        break;
                }

                FlightsDataGrid.ItemsSource = query.ToList();
            }
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

        private void ExportToExcel()
        {
            var flights = FlightsDataGrid.ItemsSource as List<Flight>;
            if (flights == null || !flights.Any())
            {
                MessageBox.Show("Нет данных для экспорта.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var workbook = new XLWorkbook();
            var worksheet = workbook.Worksheets.Add("Flights");

            worksheet.Cell(1, 1).Value = "Время вылета";
            worksheet.Cell(1, 2).Value = "Время прибытия";
            worksheet.Cell(1, 3).Value = "Авиакомпания";
            worksheet.Cell(1, 4).Value = "Самолёт";

            for (int i = 0; i < flights.Count; i++)
            {
                var flight = flights[i];
                worksheet.Cell(i + 2, 1).Value = flight.Departure_Time;
                worksheet.Cell(i + 2, 2).Value = flight.Arrival_Date_Time;
                worksheet.Cell(i + 2, 3).Value = flight.Airline.Airline_Name;
                worksheet.Cell(i + 2, 4).Value = flight.Airplane.Board_Number;
            }

            var saveFileDialog = new SaveFileDialog
            {
                Filter = "Excel Files|*.xlsx",
                Title = "Сохранить как Excel файл"
            };

            if (saveFileDialog.ShowDialog() == true)
            {
                workbook.SaveAs(saveFileDialog.FileName);
                MessageBox.Show("Данные успешно экспортированы.", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        private void ExportButton_Click(object sender, RoutedEventArgs e)
        {
            ExportToExcel();
        }


    }
}

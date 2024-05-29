using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using Airport.Models;
using Airport.Other;
using Microsoft.EntityFrameworkCore;

namespace Airport
{
    public partial class TicketsWindow : Window
    {
        private MyDbContext _context;
        private User _user; 

        public TicketsWindow(User user)
        {
            InitializeComponent();
            _context = new MyDbContext();
            _user = user; 
            LoadTickets();
        }

        public void LoadTickets()
        {
            var tickets = _context.Tickets.Include(t => t.Passenger).ToList();
            TicketsDataGrid.ItemsSource = tickets;
        }

        private void EditRow_Click(object sender, RoutedEventArgs e)
        {
            if (_user.Role != "Admin")
            {
                MessageBox.Show("У вас нет прав для редактирования билетов.", "Ограничение доступа", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var button = sender as Button;
            if (button != null)
            {
                int ticketId = (int)button.Tag;
                var ticket = _context.Tickets.Include(t => t.Passenger).FirstOrDefault(t => t.Ticket_ID == ticketId);
                if (ticket != null)
                {
                    _context.Attach(ticket);
                    TicketsEditWindow editWindow = new TicketsEditWindow(ticket, this);
                    if (editWindow.ShowDialog() == true)
                    {
                        LoadTickets();
                    }
                }
            }
        }

        private void DeleteRow_Click(object sender, RoutedEventArgs e)
        {
            if (_user.Role != "Admin")
            {
                MessageBox.Show("У вас нет прав для удаления билетов.", "Ограничение доступа", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var button = sender as Button;
            if (button != null)
            {
                int ticketId = (int)button.Tag;
                DataOperations.DeleteRow<Ticket>(_context, ticketId, LoadTickets);
            }
        }


        private void AddButton_Click(object sender, RoutedEventArgs e)
        {
            if (_user.Role != "Admin")
            {
                MessageBox.Show("У вас нет прав для добавления билетов.", "Ограничение доступа", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            TicketsAddWindow addWindow = new TicketsAddWindow(this);
            if (addWindow.ShowDialog() == true)
            {
                LoadTickets();
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

            var tickets = _context.Tickets.Include(t => t.Passenger).ToList();

            switch (selectedColumn)
            {
                case "Цена билета":
                    tickets = tickets.Where(t => t.Ticket_Price.ToString().Contains(searchText)).ToList();
                    break;
                case "Класс билета":
                    tickets = tickets.Where(t => t.Ticket_Class.ToLower().Contains(searchText)).ToList();
                    break;
                case "Дата/время покупки":
                    tickets = tickets.Where(t => t.Purchase_Date_Time.ToString().ToLower().Contains(searchText)).ToList();
                    break;
                case "Пассажир":
                    tickets = tickets.Where(t => t.Passenger.Last_Name.ToLower().Contains(searchText)).ToList();
                    break;
            }

            TicketsDataGrid.ItemsSource = tickets;
        }

        private void CompaniesButton_Click(object sender, RoutedEventArgs e)
        {
            CompaniesWindow companiesWindow = new CompaniesWindow(_user);
            companiesWindow.Show();
            this.Close();
        }

        private void PlanesButton_Click(object sender, RoutedEventArgs e)
        {
            AirplanesWindow planesWindow = new AirplanesWindow(_user);
            planesWindow.Show();
            this.Close();
        }

        private void FlightsButton_Click(object sender, RoutedEventArgs e)
        {
            FlightsWindow flightsWindow = new FlightsWindow(_user);
            flightsWindow.Show();
            this.Close();
        }

        private void CrewButton_Click(object sender, RoutedEventArgs e)
        {
            CrewMembersWindow crewMembersWindow = new CrewMembersWindow(_user);
            crewMembersWindow.Show();
            this.Close();
        }

        private void PassengersButton_Click(object sender, RoutedEventArgs e)
        {
            PassengersWindow passengersWindow = new PassengersWindow(_user);
            passengersWindow.Show();
            this.Close();
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

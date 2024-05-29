using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using Airport.Models;
using System.Collections.Generic;
using System.Linq;
using System.Linq;
using Airport.Other;

namespace Airport
{
    public partial class CompaniesWindow : Window
    {

        private MyDbContext _context;
        private User _user;
        public CompaniesWindow(User user)
        {
            InitializeComponent();
            _context = new MyDbContext();
            _user = user;
            LoadAirlinesData();
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
                IQueryable<Airline> query = context.Airlines;

                switch (selectedColumn)
                {
                    case "Название авиакомпании":
                        query = query.Where(a => a.Airline_Name.ToLower().Contains(searchText));
                        break;
                    case "Страна":
                        query = query.Where(a => a.Country.ToLower().Contains(searchText));
                        break;
                    case "Количество самолётов":
                        query = query.Where(a => a.Airplane_Count.ToString().Contains(searchText));
                        break;
                    case "Тип":
                        query = query.Where(a => a.Type.ToLower().Contains(searchText));
                        break;
                }

                AirlinesDataGrid.ItemsSource = query.ToList();
            }
        }


        public void LoadAirlinesData()
        {
            using (var context = new MyDbContext())
            {
                var airlines = context.Airlines.ToList();
                AirlinesDataGrid.ItemsSource = airlines;
            }
        }


        public void RefreshAirlinesData()
        {
            LoadAirlinesData();
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

        private void TicketsButton_Click(object sender, RoutedEventArgs e)
        {
            TicketsWindow ticketsWindow = new TicketsWindow(_user);
            ticketsWindow.Show();
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



        private void EditButton_Click(object sender, RoutedEventArgs e)
        {
            var selectedAirline = AirlinesDataGrid.SelectedItem as Airline;
            if (selectedAirline != null)
            {
                CompaniesEditWindow editWindow = new CompaniesEditWindow(selectedAirline);
                if (editWindow.ShowDialog() == true)
                {
                    using (var context = new MyDbContext())
                    {
                        context.Airlines.Update(editWindow.Airline);
                        context.SaveChanges();
                        LoadAirlinesData();
                    }
                }
            }
        }

        private void AddButton_Click(object sender, RoutedEventArgs e)
        {
            if (_user.Role != "Admin")
            {
                MessageBox.Show("У вас нет прав для добавления записей.", "Ограничение доступа", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            CompaniesAddWindow addWindow = new CompaniesAddWindow();
            addWindow.Show();
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
                var airlineId = (int)button.Tag;
                using (var context = new MyDbContext())
                {
                    var airline = context.Airlines.FirstOrDefault(a => a.Airline_ID == airlineId);
                    if (airline != null)
                    {
                        CompaniesEditWindow editWindow = new CompaniesEditWindow(airline);
                        if (editWindow.ShowDialog() == true)
                        {
                            context.Airlines.Update(editWindow.Airline);
                            context.SaveChanges();
                            LoadAirlinesData();
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
                var airlineId = (int)button.Tag;
                DataOperations.DeleteRow<Airline>(_context, airlineId, () =>
                {
                    LoadAirlinesData();
                });
            }
        }



    }
}
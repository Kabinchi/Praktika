using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using Microsoft.EntityFrameworkCore;
using Airport.Models;
using Airport.Other;

namespace Airport
{
    public partial class CrewMembersWindow : Window
    {

        private MyDbContext _context;
        private User _user;  

        public CrewMembersWindow(User user)
        {
            InitializeComponent();
            _context = new MyDbContext();
            _user = user;  
            LoadCrewMembersData();
        }

        public void LoadCrewMembersData()
        {
            using (var context = new MyDbContext())
            {
                var crewMembers = context.CrewMembers.Include(cm => cm.Airplane).ToList();
                CrewMembersDataGrid.ItemsSource = crewMembers;
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
                IQueryable<CrewMember> query = context.CrewMembers.Include(cm => cm.Airplane);

                switch (selectedColumn)
                {
                    case "Фамилия":
                        query = query.Where(cm => cm.Last_Name.ToLower().Contains(searchText));
                        break;
                    case "Имя":
                        query = query.Where(cm => cm.First_Name.ToLower().Contains(searchText));
                        break;
                    case "Отчество":
                        query = query.Where(cm => cm.Middle_Name.ToLower().Contains(searchText));
                        break;
                    case "Должность":
                        query = query.Where(cm => cm.Position.ToLower().Contains(searchText));
                        break;
                    case "Самолёт":
                        query = query.Where(cm => cm.Airplane.Board_Number.ToLower().Contains(searchText));
                        break;
                }

                CrewMembersDataGrid.ItemsSource = query.ToList();
            }
        }

        private void AddButton_Click(object sender, RoutedEventArgs e)
        {
            if (_user.Role != "Admin")
            {
                MessageBox.Show("У вас нет прав для добавления записей.", "Ограничение доступа", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            var addWindow = new CrewMembersAddWindow(this);
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
                var crewMemberId = (int)button.Tag;
                using (var context = new MyDbContext())
                {
                    var crewMember = context.CrewMembers.Include(cm => cm.Airplane).AsNoTracking().FirstOrDefault(cm => cm.CrewMember_ID == crewMemberId);
                    if (crewMember != null)
                    {
                        var editWindow = new CrewMembersEditWindow(crewMember, this);
                        if (editWindow.ShowDialog() == true)
                        {
                            context.Entry(editWindow.CrewMember).State = EntityState.Modified;
                            context.SaveChanges();
                            LoadCrewMembersData();
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
                var crewMemberId = (int)button.Tag;
                DataOperations.DeleteRow<CrewMember>(_context, crewMemberId, () =>
                {
                    LoadCrewMembersData();
                });
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

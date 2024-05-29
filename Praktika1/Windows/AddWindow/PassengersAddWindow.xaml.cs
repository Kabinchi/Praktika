using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Airport.Models;

namespace Airport
{
    public partial class PassengersAddWindow : Window
    {
        private MyDbContext _context;
        private PassengersWindow _parentWindow;

        public PassengersAddWindow(PassengersWindow parentWindow)
        {
            InitializeComponent();
            _context = new MyDbContext();
            _parentWindow = parentWindow;
            AirplaneComboBox.ItemsSource = GetAirplanes();
            AirplaneComboBox.SelectedValuePath = "Airplane_ID";
            AirplaneComboBox.DisplayMemberPath = "Board_Number";
        }

        private void AddButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(LastNameTextBox.Text) ||
                    string.IsNullOrWhiteSpace(FirstNameTextBox.Text) ||
                    string.IsNullOrWhiteSpace(MiddleNameTextBox.Text) ||
                    string.IsNullOrWhiteSpace(DeparturePointTextBox.Text) ||
                    string.IsNullOrWhiteSpace(ArrivalPointTextBox.Text) ||
                    !DepartureDatePicker.SelectedDate.HasValue ||
                    AirplaneComboBox.SelectedValue == null)
                {
                    MessageBox.Show("Пожалуйста, заполните все поля.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                var newPassenger = new Passenger
                {
                    Last_Name = LastNameTextBox.Text,
                    First_Name = FirstNameTextBox.Text,
                    Middle_Name = MiddleNameTextBox.Text,
                    Departure_Point = DeparturePointTextBox.Text,
                    Destination = ArrivalPointTextBox.Text,
                    Departure_Date = DepartureDatePicker.SelectedDate.Value,
                    Airplane_ID = (int)AirplaneComboBox.SelectedValue
                };

                _context.Passengers.Add(newPassenger);
                _context.SaveChanges();

                MessageBox.Show("Пассажир успешно добавлен.", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
                _parentWindow.LoadPassengers();
                DialogResult = true;
                Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при добавлении пассажира: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }

        private List<Airplane> GetAirplanes()
        {
            return _context.Airplanes.ToList();
        }
        private void TextValidationTextBox(object sender, TextChangedEventArgs e)
        {
            TextBox textBox = sender as TextBox;
            if (!IsTextAllowed(textBox.Text, "^[a-zA-Zа-яА-Я\\s-]*$"))
            {
                MessageBox.Show("Пожалуйста, вводите только буквы", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                textBox.Text = Regex.Replace(textBox.Text, "[^a-zA-Zа-яА-Я\\s-]", "");
                textBox.SelectionStart = textBox.Text.Length;
            }
        }

        private static bool IsTextAllowed(string text, string pattern)
        {
            return Regex.IsMatch(text, pattern);
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using Airport.Models;
using Microsoft.EntityFrameworkCore;
using System.Text.RegularExpressions;
using System.Windows.Input;
using System.Windows.Controls;

namespace Airport
{
    public partial class PassengersEditWindow : Window
    {
        private MyDbContext _context;
        public Passenger Passenger { get; set; }
        private PassengersWindow _parentWindow;

        public PassengersEditWindow(Passenger passenger, PassengersWindow parentWindow)
        {
            InitializeComponent();
            _context = new MyDbContext();
            _parentWindow = parentWindow;
            Passenger = _context.Passengers.Include(p => p.Airplane).FirstOrDefault(p => p.Passenger_ID == passenger.Passenger_ID);
            DataContext = Passenger;

            LoadData();
            InitializeFields();
        }

        private void LoadData()
        {
            AirplaneComboBox.ItemsSource = GetAirplanes();
            AirplaneComboBox.SelectedValuePath = "Airplane_ID";
            AirplaneComboBox.DisplayMemberPath = "Board_Number";
        }

        private void InitializeFields()
        {
            LastNameTextBox.Text = Passenger.Last_Name;
            FirstNameTextBox.Text = Passenger.First_Name;
            MiddleNameTextBox.Text = Passenger.Middle_Name;
            DeparturePointTextBox.Text = Passenger.Departure_Point;
            ArrivalPointTextBox.Text = Passenger.Destination;
            DepartureDatePicker.SelectedDate = Passenger.Departure_Date;
            AirplaneComboBox.SelectedValue = Passenger.Airplane_ID;
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
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

                Passenger.Last_Name = LastNameTextBox.Text;
                Passenger.First_Name = FirstNameTextBox.Text;
                Passenger.Middle_Name = MiddleNameTextBox.Text;
                Passenger.Departure_Point = DeparturePointTextBox.Text;
                Passenger.Destination = ArrivalPointTextBox.Text;
                Passenger.Departure_Date = DepartureDatePicker.SelectedDate.Value;
                Passenger.Airplane_ID = (int)AirplaneComboBox.SelectedValue;

                _context.Entry(Passenger).State = EntityState.Modified;
                _context.SaveChanges();

                MessageBox.Show("Пассажир успешно обновлён.", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
                _parentWindow.LoadPassengers();
                DialogResult = true;
                Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при обновлении пассажира: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
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

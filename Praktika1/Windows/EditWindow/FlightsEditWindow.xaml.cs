using Microsoft.EntityFrameworkCore;
using Airport.Models;
using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Airport
{
    public partial class FlightsEditWindow : Window
    {
        private MyDbContext _context;
        public Flight Flight { get; set; }

        public FlightsEditWindow(Flight flight)
        {
            InitializeComponent();
            _context = new MyDbContext();
            Flight = _context.Flights.Include(f => f.Airline).Include(f => f.Airplane).FirstOrDefault(f => f.Flight_ID == flight.Flight_ID);
            DataContext = Flight;

            LoadData();
            InitializeFields();
        }

        private void LoadData()
        {
            using (var context = new MyDbContext())
            {
                CompanyComboBox.ItemsSource = context.Airlines.ToList();
                PlaneComboBox.ItemsSource = context.Airplanes.ToList();
            }
        }

        private void InitializeFields()
        {
            DepartureTimeTextBox.Text = Flight.Departure_Time.ToString(@"hh\:mm");
            ArrivalDatePicker.SelectedDate = Flight.Arrival_Date_Time.Date;
            ArrivalTimeTextBox.Text = Flight.Arrival_Date_Time.ToString("HH:mm");

            CompanyComboBox.SelectedValue = Flight.Airline_ID;
            PlaneComboBox.SelectedValue = Flight.Airplane_ID;
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(DepartureTimeTextBox.Text) ||
                    string.IsNullOrWhiteSpace(ArrivalTimeTextBox.Text) ||
                    !ArrivalDatePicker.SelectedDate.HasValue ||
                    CompanyComboBox.SelectedItem == null ||
                    PlaneComboBox.SelectedItem == null)
                {
                    MessageBox.Show("Пожалуйста, заполните все поля.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                var departureTimeText = DepartureTimeTextBox.Text;
                var arrivalTimeText = ArrivalTimeTextBox.Text;
                var arrivalDate = ArrivalDatePicker.SelectedDate.Value;

                if (!TimeSpan.TryParseExact(departureTimeText, @"hh\:mm", null, out TimeSpan departureTime))
                {
                    MessageBox.Show("Пожалуйста, введите время в формате чч:мм.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                if (!TimeSpan.TryParseExact(arrivalTimeText, @"hh\:mm", null, out TimeSpan arrivalTime))
                {
                    MessageBox.Show("Пожалуйста, введите время в формате чч:мм.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                var arrivalDateTime = arrivalDate.Add(arrivalTime);

                Flight.Departure_Time = departureTime;
                Flight.Arrival_Date_Time = arrivalDateTime;
                Flight.Airline_ID = (int)CompanyComboBox.SelectedValue;
                Flight.Airplane_ID = (int)PlaneComboBox.SelectedValue;

                using (var context = new MyDbContext())
                {
                    context.Flights.Update(Flight);
                    context.SaveChanges();
                }

                MessageBox.Show("Изменения успешно сохранены.", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
                DialogResult = true;
                Close();
            }
            catch (FormatException ex)
            {
                MessageBox.Show($"Ошибка формата времени: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при сохранении: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }



        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }

        private void TimeTextBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            e.Handled = !IsTextAllowed(e.Text);
        }

        private static bool IsTextAllowed(string text)
        {
            foreach (char c in text)
            {
                if (!char.IsDigit(c) && c != ':')
                {
                    return false;
                }
            }
            return true;
        }
    }
}

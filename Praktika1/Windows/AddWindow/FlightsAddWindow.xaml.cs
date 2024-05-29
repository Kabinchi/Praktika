using Airport.Models;
using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Airport
{
    public partial class FlightsAddWindow : Window
    {
        private FlightsWindow _parentWindow;

        public FlightsAddWindow(FlightsWindow parentWindow)
        {
            InitializeComponent();
            _parentWindow = parentWindow;
            LoadData();
        }

        private void LoadData()
        {
            using (var context = new MyDbContext())
            {
                CompanyComboBox.ItemsSource = context.Airlines.ToList();
                PlaneComboBox.ItemsSource = context.Airplanes.ToList();
            }
        }

        private void AddButton_Click(object sender, RoutedEventArgs e)
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

                if (!TimeSpan.TryParseExact(DepartureTimeTextBox.Text, "hh\\:mm", null, out TimeSpan departureTime) ||
                    !TimeSpan.TryParseExact(ArrivalTimeTextBox.Text, "hh\\:mm", null, out TimeSpan arrivalTime))
                {
                    MessageBox.Show("Пожалуйста, введите время в формате чч:мм.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                var arrivalDate = ArrivalDatePicker.SelectedDate.Value;
                var arrivalDateTime = arrivalDate.Add(arrivalTime);

                var newFlight = new Flight
                {
                    Departure_Time = departureTime,
                    Arrival_Date_Time = arrivalDateTime,
                    Airline_ID = (int)CompanyComboBox.SelectedValue,
                    Airplane_ID = (int)PlaneComboBox.SelectedValue
                };

                using (var context = new MyDbContext())
                {
                    context.Flights.Add(newFlight);
                    context.SaveChanges();
                }

                _parentWindow.LoadFlights();
                MessageBox.Show("Рейс успешно добавлен.", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
                Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при добавлении рейса: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void ExitButton_Click(object sender, RoutedEventArgs e)
        {
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

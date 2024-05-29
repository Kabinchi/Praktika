using Airport.Models;
using System;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Airport
{
    public partial class AirplanesAddWindow : Window
    {
        private AirplanesWindow _parentWindow;

        public AirplanesAddWindow(AirplanesWindow parentWindow)
        {
            InitializeComponent();
            LoadCompanies();
            _parentWindow = parentWindow;
        }

        private void LoadCompanies()
        {
            using (var context = new MyDbContext())
            {
                var companies = context.Airlines.ToList();
                CompanyComboBox.ItemsSource = companies;
            }
        }

        private void AddButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(SeatsNumberTextBox.Text) ||
                    string.IsNullOrWhiteSpace(PlaneNumberTextBox.Text) ||
                    string.IsNullOrWhiteSpace(FlightRangeTextBox.Text) ||
                    string.IsNullOrWhiteSpace(TicketClassPriceTextBox.Text) ||
                    CompanyComboBox.SelectedItem == null)
                {
                    MessageBox.Show("Пожалуйста, заполните все поля.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                if (!int.TryParse(SeatsNumberTextBox.Text, out _) ||
                    !int.TryParse(FlightRangeTextBox.Text, out _) ||
                    !int.TryParse(TicketClassPriceTextBox.Text, out _))
                {
                    MessageBox.Show("Пожалуйста, введите числовые значения для количества мест, дальности перелёта и стоимости билета.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                var newAirplane = new Airplane
                {
                    Seat_Count = int.Parse(SeatsNumberTextBox.Text),
                    Board_Number = PlaneNumberTextBox.Text,
                    Flight_Range = int.Parse(FlightRangeTextBox.Text),
                    Ticket_Price_Class = int.Parse(TicketClassPriceTextBox.Text),
                    Airline_ID = (int)CompanyComboBox.SelectedValue
                };

                using (var context = new MyDbContext())
                {
                    context.Airplanes.Add(newAirplane);
                    context.SaveChanges();
                }

                _parentWindow.LoadAirplanesData();

                MessageBox.Show("Самолёт успешно добавлен.", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
                Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при добавлении самолёта: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void ExitButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void NumberValidationTextBox(object sender, TextChangedEventArgs e)
        {
            TextBox textBox = sender as TextBox;
            if (!IsTextAllowed(textBox.Text, "^[0-9]*$"))
            {
                MessageBox.Show("Пожалуйста, вводите только цифры.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                textBox.Text = Regex.Replace(textBox.Text, "[^0-9]", "");
                textBox.SelectionStart = textBox.Text.Length; 
            }
        }

        private void AlphaNumericValidationTextBox(object sender, TextChangedEventArgs e)
        {
            TextBox textBox = sender as TextBox;
            if (!IsTextAllowed(textBox.Text, "^[a-zA-Z0-9]*$"))
            {
                MessageBox.Show("Пожалуйста, вводите только буквы и цифры.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                textBox.Text = Regex.Replace(textBox.Text, "[^a-zA-Z0-9]", "");
                textBox.SelectionStart = textBox.Text.Length; 
            }
        }

        private static bool IsTextAllowed(string text, string pattern)
        {
            return Regex.IsMatch(text, pattern);
        }
    }
}

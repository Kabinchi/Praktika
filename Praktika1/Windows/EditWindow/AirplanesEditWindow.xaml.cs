using System;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Airport.Models;

namespace Airport
{
    public partial class AirplanesEditWindow : Window
    {
        public Airplane Airplane { get; set; }
        public bool IsSaved { get; private set; }

        public AirplanesEditWindow(Airplane airplane)
        {
            InitializeComponent();
            Airplane = airplane;
            IsSaved = false;
            SeatCountTextBox.Text = airplane.Seat_Count.ToString();
            AirplaneNumberTextBox.Text = airplane.Board_Number;
            FlightRangeTextBox.Text = airplane.Flight_Range.ToString();
            TicketPriceTextBox.Text = airplane.Ticket_Price_Class.ToString();
            AirlineComboBox.ItemsSource = GetAirlines();
            AirlineComboBox.SelectedValuePath = "Airline_ID";
            AirlineComboBox.DisplayMemberPath = "Airline_Name";
            AirlineComboBox.SelectedValue = airplane.Airline_ID;
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(SeatCountTextBox.Text) ||
                    string.IsNullOrWhiteSpace(AirplaneNumberTextBox.Text) ||
                    string.IsNullOrWhiteSpace(FlightRangeTextBox.Text) ||
                    string.IsNullOrWhiteSpace(TicketPriceTextBox.Text) ||
                    AirlineComboBox.SelectedItem == null)
                {
                    MessageBox.Show("Пожалуйста, заполните все поля.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                if (!int.TryParse(SeatCountTextBox.Text, out _) ||
                    !int.TryParse(FlightRangeTextBox.Text, out _) ||
                    !int.TryParse(TicketPriceTextBox.Text, out _))
                {
                    MessageBox.Show("Пожалуйста, введите правильные числовые значения для количества мест, дальности перелёта и стоимости билета.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                var selectedAirline = AirlineComboBox.SelectedItem as Airline;
                if (selectedAirline == null)
                {
                    MessageBox.Show("Неверный формат данных для выбранной авиакомпании.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                Airplane.Seat_Count = int.Parse(SeatCountTextBox.Text);
                Airplane.Board_Number = AirplaneNumberTextBox.Text;
                Airplane.Flight_Range = int.Parse(FlightRangeTextBox.Text);
                Airplane.Ticket_Price_Class = int.Parse(TicketPriceTextBox.Text);
                Airplane.Airline_ID = selectedAirline.Airline_ID;

                using (var context = new MyDbContext())
                {
                    context.Airplanes.Update(Airplane);
                    context.SaveChanges();
                }

                MessageBox.Show("Изменения успешно сохранены.", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
                IsSaved = true;
                DialogResult = true;
                Close();
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

        private List<Airline> GetAirlines()
        {
            using (var context = new MyDbContext())
            {
                return context.Airlines.ToList();
            }
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

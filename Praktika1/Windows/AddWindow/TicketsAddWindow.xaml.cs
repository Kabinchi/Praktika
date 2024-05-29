using System;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Airport.Models;

namespace Airport
{
    public partial class TicketsAddWindow : Window
    {
        private MyDbContext _context;
        private TicketsWindow _parentWindow;

        public TicketsAddWindow(TicketsWindow parentWindow)
        {
            InitializeComponent();
            _context = new MyDbContext();
            _parentWindow = parentWindow;

            PassengerComboBox.ItemsSource = _context.Passengers.ToList();
            PassengerComboBox.SelectedValuePath = "Passenger_ID";
            PassengerComboBox.DisplayMemberPath = "Last_Name";
        }

        private void AddButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (!PurchaseDatePicker.SelectedDate.HasValue ||
                    string.IsNullOrWhiteSpace(PurchaseTimeTextBox.Text) ||
                    PassengerComboBox.SelectedValue == null ||
                    string.IsNullOrWhiteSpace(TicketPriceTextBox.Text) ||
                    TicketClassComboBox.SelectedItem == null)
                {
                    MessageBox.Show("Пожалуйста, заполните все поля.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                if (!TimeSpan.TryParseExact(PurchaseTimeTextBox.Text, "hh\\:mm", null, out TimeSpan purchaseTime))
                {
                    MessageBox.Show("Пожалуйста, введите время в формате чч:мм.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                var purchaseDate = PurchaseDatePicker.SelectedDate.Value;
                var purchaseDateTime = purchaseDate.Add(purchaseTime);

                if (!int.TryParse(TicketPriceTextBox.Text, out int ticketPrice))
                {
                    MessageBox.Show("Цена билета должна быть числом.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                var ticketClass = ((ComboBoxItem)TicketClassComboBox.SelectedItem).Content.ToString();

                var ticket = new Ticket
                {
                    Ticket_Price = ticketPrice,
                    Ticket_Class = ticketClass,
                    Purchase_Date_Time = purchaseDateTime,
                    Passenger_ID = (int)PassengerComboBox.SelectedValue
                };

                _context.Tickets.Add(ticket);
                _context.SaveChanges();

                MessageBox.Show("Билет успешно добавлен.", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
                _parentWindow.LoadTickets();
                DialogResult = true;
                Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при добавлении билета: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
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

        private static bool IsTextAllowed(string text, string pattern)
        {
            return Regex.IsMatch(text, pattern);
        }

        private void TimeTextBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            e.Handled = !IsTextTimeFormat(e.Text);
        }

        private static bool IsTextTimeFormat(string text)
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

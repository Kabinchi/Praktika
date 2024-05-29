using System;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Microsoft.EntityFrameworkCore;
using Airport.Models;

namespace Airport
{
    public partial class TicketsEditWindow : Window
    {
        private MyDbContext _context;
        private Ticket _ticket;
        private TicketsWindow _parentWindow;

        public TicketsEditWindow(Ticket ticket, TicketsWindow parentWindow)
        {
            InitializeComponent();
            _context = new MyDbContext();
            _ticket = ticket;
            _parentWindow = parentWindow;

            PassengerComboBox.ItemsSource = _context.Passengers.ToList();
            PassengerComboBox.SelectedValuePath = "Passenger_ID";
            PassengerComboBox.DisplayMemberPath = "Last_Name";
            PassengerComboBox.SelectedValue = _ticket.Passenger_ID;

            TicketPriceTextBox.Text = _ticket.Ticket_Price.ToString();
            TicketClassComboBox.SelectedItem = TicketClassComboBox.Items.OfType<ComboBoxItem>().FirstOrDefault(i => i.Content.ToString() == _ticket.Ticket_Class);
            var purchaseDateTime = _ticket.Purchase_Date_Time;
            PurchaseDatePicker.SelectedDate = purchaseDateTime.Date;
            PurchaseTimeTextBox.Text = purchaseDateTime.ToString("hh\\:mm");
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
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

                _ticket.Ticket_Price = ticketPrice;
                _ticket.Ticket_Class = ticketClass;
                _ticket.Purchase_Date_Time = purchaseDateTime;
                _ticket.Passenger_ID = (int)PassengerComboBox.SelectedValue;

                _context.Entry(_ticket).State = EntityState.Modified;
                _context.SaveChanges();

                MessageBox.Show("Билет успешно обновлен.", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
                _parentWindow.LoadTickets();
                DialogResult = true;
                Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при обновлении билета: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
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

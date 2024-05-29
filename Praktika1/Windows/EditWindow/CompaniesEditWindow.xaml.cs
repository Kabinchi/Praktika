using Airport.Models;
using System;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Airport
{
    public partial class CompaniesEditWindow : Window
    {
        public Airline Airline { get; set; }

        public CompaniesEditWindow(Airline airline)
        {
            InitializeComponent();
            Airline = airline;
            CompanyNameTextBox.Text = airline.Airline_Name;
            CountryTextBox.Text = airline.Country;
            NumberOfPlanesTextBox.Text = airline.Airplane_Count.ToString();
            TypeComboBox.SelectedItem = TypeComboBox.Items.Cast<ComboBoxItem>().FirstOrDefault(item => item.Content.ToString() == airline.Type);
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(CompanyNameTextBox.Text) ||
                    string.IsNullOrWhiteSpace(CountryTextBox.Text) ||
                    string.IsNullOrWhiteSpace(NumberOfPlanesTextBox.Text) ||
                    TypeComboBox.SelectedItem == null)
                {
                    MessageBox.Show("Пожалуйста, заполните все поля.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                if (!int.TryParse(NumberOfPlanesTextBox.Text, out _))
                {
                    MessageBox.Show("Пожалуйста, введите числовое значение для количества самолётов.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                Airline.Airline_Name = CompanyNameTextBox.Text;
                Airline.Country = CountryTextBox.Text;
                Airline.Airplane_Count = int.Parse(NumberOfPlanesTextBox.Text);
                Airline.Type = (TypeComboBox.SelectedItem as ComboBoxItem).Content.ToString();

                using (var context = new MyDbContext())
                {
                    context.Airlines.Update(Airline);
                    context.SaveChanges();
                }

                MessageBox.Show("Изменения успешно сохранены.", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
                DialogResult = true;
                Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при сохранении изменений: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
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

        private void TextValidationTextBox(object sender, TextChangedEventArgs e)
        {
            TextBox textBox = sender as TextBox;
            if (!IsTextAllowed(textBox.Text, "^[a-zA-Zа-яА-Я]*$"))
            {
                MessageBox.Show("Пожалуйста, вводите только буквы.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                textBox.Text = Regex.Replace(textBox.Text, "[^a-zA-Zа-яА-Я]", "");
                textBox.SelectionStart = textBox.Text.Length;
            }
        }

        private static bool IsTextAllowed(string text, string pattern)
        {
            return Regex.IsMatch(text, pattern);
        }
    }
}

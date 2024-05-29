using Airport.Models;
using System;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Airport
{
    public partial class CompaniesAddWindow : Window
    {
        public CompaniesAddWindow()
        {
            InitializeComponent();
        }

        private void AddButton_Click(object sender, RoutedEventArgs e)
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

                var newAirline = new Airline
                {
                    Airline_Name = CompanyNameTextBox.Text,
                    Country = CountryTextBox.Text,
                    Airplane_Count = int.Parse(NumberOfPlanesTextBox.Text),
                    Type = (TypeComboBox.SelectedItem as ComboBoxItem).Content.ToString()
                };

                using (var context = new MyDbContext())
                {
                    context.Airlines.Add(newAirline);
                    context.SaveChanges();
                }

                if (Application.Current.MainWindow is CompaniesWindow companiesWindow)
                {
                    companiesWindow.LoadAirlinesData();
                }

                MessageBox.Show("Авиакомпания успешно добавлена.", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
                Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при добавлении авиакомпании: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
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

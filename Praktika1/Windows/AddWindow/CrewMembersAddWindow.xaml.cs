using Airport.Models;
using System;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Airport
{
    public partial class CrewMembersAddWindow : Window
    {
        private CrewMembersWindow _parentWindow;

        public CrewMembersAddWindow(CrewMembersWindow parentWindow)
        {
            InitializeComponent();
            _parentWindow = parentWindow;
            LoadData();
        }

        private void LoadData()
        {
            using (var context = new MyDbContext())
            {
                PlaneComboBox.ItemsSource = context.Airplanes.ToList();
            }
        }

        private void AddButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(LastNameTextBox.Text) ||
                    string.IsNullOrWhiteSpace(FirstNameTextBox.Text) ||
                    string.IsNullOrWhiteSpace(MiddleNameTextBox.Text) ||
                    PositionComboBox.SelectedItem == null ||
                    PlaneComboBox.SelectedItem == null)
                {
                    MessageBox.Show("Пожалуйста, заполните все поля.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                var newCrewMember = new CrewMember
                {
                    Last_Name = LastNameTextBox.Text,
                    First_Name = FirstNameTextBox.Text,
                    Middle_Name = MiddleNameTextBox.Text,
                    Position = (PositionComboBox.SelectedItem as ComboBoxItem).Content.ToString(),
                    Airplane_ID = (int)PlaneComboBox.SelectedValue
                };

                using (var context = new MyDbContext())
                {
                    context.CrewMembers.Add(newCrewMember);
                    context.SaveChanges();
                }

                _parentWindow.LoadCrewMembersData();
                MessageBox.Show("Член экипажа успешно добавлен.", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
                Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при добавлении члена экипажа: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void ExitButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
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

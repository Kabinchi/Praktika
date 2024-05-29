using System;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Input;
using Microsoft.EntityFrameworkCore;
using Airport.Models;
using System.Windows.Controls;

namespace Airport
{
    public partial class CrewMembersEditWindow : Window
    {
        private MyDbContext _context;
        private CrewMembersWindow _parentWindow;
        public CrewMember CrewMember { get; set; }

        public CrewMembersEditWindow(CrewMember crewMember, CrewMembersWindow parentWindow)
        {
            InitializeComponent();
            _context = new MyDbContext();
            _parentWindow = parentWindow;
            CrewMember = _context.CrewMembers.Include(c => c.Airplane).FirstOrDefault(c => c.CrewMember_ID == crewMember.CrewMember_ID);
            DataContext = CrewMember;

            LastNameTextBox.Text = CrewMember.Last_Name;
            FirstNameTextBox.Text = CrewMember.First_Name;
            MiddleNameTextBox.Text = CrewMember.Middle_Name;
            PositionComboBox.Text = CrewMember.Position;

            AirplaneComboBox.ItemsSource = GetAirplanes();
            AirplaneComboBox.SelectedValue = CrewMember.Airplane_ID;
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(LastNameTextBox.Text) ||
                    string.IsNullOrWhiteSpace(FirstNameTextBox.Text) ||
                    string.IsNullOrWhiteSpace(MiddleNameTextBox.Text) ||
                    string.IsNullOrWhiteSpace(PositionComboBox.Text) ||
                    AirplaneComboBox.SelectedValue == null)
                {
                    MessageBox.Show("Пожалуйста, заполните все поля.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                CrewMember.Last_Name = LastNameTextBox.Text;
                CrewMember.First_Name = FirstNameTextBox.Text;
                CrewMember.Middle_Name = MiddleNameTextBox.Text;
                CrewMember.Position = PositionComboBox.Text;
                CrewMember.Airplane_ID = (int)AirplaneComboBox.SelectedValue;

                var existingEntity = _context.CrewMembers.Local.FirstOrDefault(cm => cm.CrewMember_ID == CrewMember.CrewMember_ID);
                if (existingEntity != null)
                {
                    _context.Entry(existingEntity).State = EntityState.Detached;
                }

                _context.Entry(CrewMember).State = EntityState.Modified;
                _context.SaveChanges();

                MessageBox.Show("Изменения успешно сохранены.", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
                _parentWindow.LoadCrewMembersData();
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

        private void LoadData()
        {
            AirplaneComboBox.ItemsSource = _context.Airplanes.ToList();
        }

        private List<Airplane> GetAirplanes()
        {
            return _context.Airplanes.ToList();
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

﻿using BCrypt.Net;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows;

namespace CPS2
{
    public partial class RegisterWindow : Window
    {
        public RegisterWindow()
        {
            InitializeComponent();
        }

        private void RegisterButton_Click(object sender, RoutedEventArgs e)
        {
            string password = PasswordBox.Password;
            string confirmPassword = ConfirmPasswordBox.Password;
            string username = UsernameTextBox.Text;

            // Проверка совпадения паролей
            if (password != confirmPassword)
            {
                MessageBox.Show("Пароли не совпадают!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            // Проверка сложности пароля
            if (!IsPasswordStrong(password))
            {
                MessageBox.Show("Пароль должен содержать минимум 8 символов, включая буквы, цифры и спец. символы!", 
                                "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            using var db = new AppDbContext();
            if (db.Users.Any(u => u.Username == username))
            {
                MessageBox.Show("Пользователь уже существует!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            var newUser = new User
            {
                Username = username,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(password), // Соль включена внутрь
                Role = "user",
                IsActive = true,
                RegistrationDate = DateTime.UtcNow
            };

            db.Users.Add(newUser);
            db.SaveChanges();

            MessageBox.Show("Регистрация успешна!", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
            Close();
        }

        // Метод проверки сложности пароля
        private bool IsPasswordStrong(string password)
        {
            // Длина не менее 8 символов, хотя бы одна буква, цифра и спецсимвол
            return Regex.IsMatch(password, @"^(?=.*[A-Za-z])(?=.*\d)(?=.*[\W_]).{8,}$");
        }
    }
}
﻿using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using BCrypt.Net;

namespace CPS2
{
    public partial class UserEditWindow : Window
    {
        public User User { get; set; } = new User();
        public ObservableCollection<string> Roles { get; set; } = new ObservableCollection<string> { "admin", "user" };

        public UserEditWindow()
        {
            InitializeComponent();
            DataContext = this;
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            if (User == null)
            {
                MessageBox.Show("Ошибка: пользователь не задан!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            Console.WriteLine($"Редактируемый пользователь: {User.Username}, Роль: {User.Role}");

            if (string.IsNullOrWhiteSpace(User.Username))
            {
                MessageBox.Show("Введите логин!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            using (var db = new AppDbContext())
            {
                if (db.Users.Any(u => u.Username == User.Username && u.Id != User.Id))
                {
                    MessageBox.Show("Пользователь с таким логином уже существует!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }
            }

            Console.WriteLine($"Проверка роли перед сохранением: '{User.Role}'");
            if (string.IsNullOrWhiteSpace(User.Role) || (User.Role != "admin" && User.Role != "user"))
            {
                MessageBox.Show("Некорректное значение роли!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            if (!string.IsNullOrEmpty(PasswordBox.Password))
            {
                User.PasswordHash = BCrypt.Net.BCrypt.HashPassword(PasswordBox.Password);
            }

            DialogResult = true;
            Close();
        }
    }
}
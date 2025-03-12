// RegisterWindow.xaml.cs
using BCrypt.Net;
using System.Linq;
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
            if (PasswordBox.Password != ConfirmPasswordBox.Password)
            {
                MessageBox.Show("Пароли не совпадают!");
                return;
            }

            using var db = new AppDbContext();
            if (db.Users.Any(u => u.Username == UsernameTextBox.Text))
            {
                MessageBox.Show("Пользователь уже существует!");
                return;
            }

            var newUser = new User
            {
                Username = UsernameTextBox.Text,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(PasswordBox.Password),
                Salt = BCrypt.Net.BCrypt.GenerateSalt(),
                Role = "user", // Исправлено на строчные буквы
                IsActive = true,
                RegistrationDate = DateTime.UtcNow
            };

            db.Users.Add(newUser);
            db.SaveChanges();
            
            MessageBox.Show("Регистрация успешна!");
            Close();
        }
    }
}
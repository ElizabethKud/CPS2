using BCrypt.Net;
using System.Windows;
using System.Linq;

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
            // Проверка на совпадение паролей
            if (PasswordBox.Password != ConfirmPasswordBox.Password)
            {
                MessageBox.Show("Пароли не совпадают.");
                return;
            }

            using var db = new AppDbContext();
            // Проверка, существует ли уже пользователь с таким логином
            var existingUser = db.Users.FirstOrDefault(u => u.Username == UsernameTextBox.Text);
            if (existingUser != null)
            {
                MessageBox.Show("Пользователь с таким логином уже существует.");
                return;
            }

            // Хеширование пароля перед сохранением
            string hashedPassword = BCrypt.Net.BCrypt.HashPassword(PasswordBox.Password);

            // Создание нового пользователя
            var newUser = new User
            {
                Username = UsernameTextBox.Text,
                PasswordHash = hashedPassword,
                Salt = "",  // Соль не требуется, так как она уже встроена в хеш
                RegistrationDate = DateTime.UtcNow,
                IsActive = true, // Устанавливаем активного пользователя
                Role = "User" // Или другую роль, как требуется
            };

            // Добавление в базу данных
            db.Users.Add(newUser);
            db.SaveChanges();  // Сохранение изменений

            MessageBox.Show("Регистрация успешна!");
            this.Close();
        }
    }
}
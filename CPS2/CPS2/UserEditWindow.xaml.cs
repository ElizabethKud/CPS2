using System.Linq;
using System.Windows;
using BCrypt.Net;

namespace CPS2
{
    public partial class UserEditWindow : Window
    {
        public User User { get; set; } = new User();

        public UserEditWindow()
        {
            InitializeComponent();
            DataContext = this;
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(User.Username))
            {
                MessageBox.Show("Введите логин!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            // Проверка на уникальность логина
            using (var db = new AppDbContext())
            {
                if (db.Users.Any(u => u.Username == User.Username && u.Id != User.Id)) // исключаем текущего пользователя при редактировании
                {
                    MessageBox.Show("Пользователь с таким логином уже существует!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }
            }

            // Если пароль не пустой, то хешируем его
            if (!string.IsNullOrEmpty(PasswordBox.Password))
            {
                // Генерация соли и хеширование пароля происходит внутри этого метода
                User.PasswordHash = BCrypt.Net.BCrypt.HashPassword(PasswordBox.Password);
            }

            // Если пароль пустой, сохраняем старый хеш (в случае редактирования без изменения пароля)
            if (string.IsNullOrEmpty(PasswordBox.Password) && User.PasswordHash == null)
            {
                MessageBox.Show("Пароль не может быть пустым!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            DialogResult = true;
            Close();
        }
    }
}
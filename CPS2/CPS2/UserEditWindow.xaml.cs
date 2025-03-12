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
                MessageBox.Show("Введите логин!");
                return;
            }

            // Если пароль не пустой, то хешируем его
            if (!string.IsNullOrEmpty(PasswordBox.Password))
            {
                // Генерация соли и хеширование пароля происходит внутри этого метода
                User.PasswordHash = BCrypt.Net.BCrypt.HashPassword(PasswordBox.Password);
            }

            DialogResult = true;
            Close();
        }
    }
}
using System.Linq;
using System.Windows;
using BCrypt.Net;

namespace CPS2
{
    public partial class LoginWindow : Window
    {
        public LoginWindow()
        {
            InitializeComponent();
        }

        private void LoginButton_Click(object sender, RoutedEventArgs e)
        {
            using var db = new AppDbContext();
            var user = db.Users.FirstOrDefault(u => u.Username == UsernameTextBox.Text);

            if (user != null && BCrypt.Net.BCrypt.Verify(PasswordBox.Password, user.PasswordHash))
            {
                // Проверяем, не прошло ли более недели с последнего входа
                if (user.LastLogin != null && user.LastLogin < DateTime.UtcNow.AddDays(-7))
                {
                    // Если прошло больше недели, делаем пользователя неактивным
                    user.IsActive = false;
                    db.SaveChanges();
                }

                // Обновляем дату последнего входа
                user.LastLogin = DateTime.UtcNow;
                db.SaveChanges();

                // Если пользователь активен, открываем главное окно
                if (!user.IsActive)
                {
                    MessageBox.Show("Вы не заходили в приложение более недели. С возвращением!");
                }
                var mainWindow = new MainWindow(user);
                mainWindow.Show();
                this.Close();
            }
            else
            {
                MessageBox.Show("Ошибка авторизации!");
            }
        }


        private void RegisterButton_Click(object sender, RoutedEventArgs e)
        {
            new RegisterWindow().ShowDialog();
        }
    }
}
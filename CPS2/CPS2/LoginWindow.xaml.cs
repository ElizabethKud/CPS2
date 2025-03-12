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

            if (user != null && 
                BCrypt.Net.BCrypt.Verify(PasswordBox.Password, user.PasswordHash) && 
                user.IsActive)
            {
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
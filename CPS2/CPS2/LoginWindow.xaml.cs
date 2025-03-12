using System.Windows;

namespace CPS2;

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
            var mainWindow = new MainWindow(user);
            mainWindow.Show();
            this.Close();
        }
        else
        {
            MessageBox.Show("Неверные учетные данные");
        }
    }

    private void RegisterButton_Click(object sender, RoutedEventArgs e)
    {
        var registerWindow = new RegisterWindow();
        registerWindow.ShowDialog();
    }
}
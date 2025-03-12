using System.Windows;

namespace CPS2;

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
            MessageBox.Show("Пароли не совпадают");
            return;
        }

        if (!IsPasswordValid(PasswordBox.Password))
        {
            MessageBox.Show("Пароль должен содержать минимум 8 символов, цифры и спецсимволы");
            return;
        }

        using var db = new AppDbContext();
        if (db.Users.Any(u => u.Username == UsernameTextBox.Text))
        {
            MessageBox.Show("Пользователь уже существует");
            return;
        }

        var salt = BCrypt.Net.BCrypt.GenerateSalt();
        var user = new User
        {
            Username = UsernameTextBox.Text,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(PasswordBox.Password, salt),
            Salt = salt,
            RegistrationDate = DateTime.Now,
            IsActive = true,
            Role = "user"
        };

        db.Users.Add(user);
        db.SaveChanges();
        MessageBox.Show("Регистрация успешна");
        this.Close();
    }

    private bool IsPasswordValid(string password)
    {
        // Реализация проверки сложности пароля
        return password.Length >= 8 && 
               password.Any(char.IsDigit) && 
               password.Any(c => !char.IsLetterOrDigit(c));
    }
}
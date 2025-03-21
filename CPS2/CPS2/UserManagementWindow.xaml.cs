using System.Linq;
using System.Windows;

namespace CPS2
{
    public partial class UserManagementWindow : Window
    {
        public UserManagementWindow()
        {
            InitializeComponent();
            LoadUsers();
        }

        private void LoadUsers()
        {
            using var db = new AppDbContext();
            UsersListView.ItemsSource = db.Users.ToList();
        }

        private void AddUser_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new UserEditWindow();
            if (dialog.ShowDialog() == true)
            {
                using var db = new AppDbContext();
                db.Users.Add(dialog.User);
                db.SaveChanges();
                LoadUsers();
            }
        }

        private void EditUser_Click(object sender, RoutedEventArgs e)
        {
            if (UsersListView.SelectedItem is User selectedUser)
            {
                // Создаем копию объекта для редактирования
                var userCopy = new User 
                {
                    Id = selectedUser.Id,
                    Username = selectedUser.Username,
                    Role = selectedUser.Role,
                    IsActive = selectedUser.IsActive
                };

                var dialog = new UserEditWindow { User = userCopy };
                if (dialog.ShowDialog() == true)
                {
                    try
                    {
                        using var db = new AppDbContext();
                
                        // Находим пользователя в БД
                        var dbUser = db.Users.First(u => u.Id == userCopy.Id);
                
                        // Обновляем поля
                        dbUser.Username = userCopy.Username;
                        dbUser.Role = userCopy.Role;
                        dbUser.IsActive = userCopy.IsActive;
                
                        // Если пароль был изменен
                        if (!string.IsNullOrEmpty(userCopy.PasswordHash) && 
                            userCopy.PasswordHash != dbUser.PasswordHash)
                        {
                            dbUser.PasswordHash = BCrypt.Net.BCrypt.HashPassword(userCopy.PasswordHash);
                        }

                        db.SaveChanges();
                        LoadUsers(); // Обновляем список
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Ошибка сохранения: {ex.Message}");
                    }
                }
            }
        }


        private void DeleteUser_Click(object sender, RoutedEventArgs e)
        {
            if (UsersListView.SelectedItem is User selectedUser)
            {
                using var db = new AppDbContext();
                db.Users.Remove(selectedUser);
                db.SaveChanges();
                LoadUsers();
            }
        }
    }
}
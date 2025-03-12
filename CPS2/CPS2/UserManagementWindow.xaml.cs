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
                var dialog = new UserEditWindow { User = selectedUser };
                if (dialog.ShowDialog() == true)
                {
                    using var db = new AppDbContext();
                    db.Users.Update(selectedUser);
                    db.SaveChanges();
                    LoadUsers();
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
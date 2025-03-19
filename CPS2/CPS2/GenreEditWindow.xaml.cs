using System.Windows;

namespace CPS2
{
    public partial class GenreEditWindow : Window
    {
        public GenreEditWindow()
        {
            InitializeComponent();
            Genre = new Genre(); // Создаём объект жанра перед привязкой
            DataContext = Genre; // Привязываем DataContext к объекту жанра
        }

        public Genre Genre { get; set; }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(Genre.GenreName))
            {
                MessageBox.Show("Введите название жанра!");
                return;
            }

            DialogResult = true;
            Close();
        }
    }
}
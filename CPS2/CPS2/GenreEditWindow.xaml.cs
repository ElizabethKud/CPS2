using System.Windows;

namespace CPS2;

public partial class GenreEditWindow : Window
{
    public Genre Genre { get; set; } = new Genre();

    public GenreEditWindow()
    {
        InitializeComponent();
        DataContext = this;
    }

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
using System.Windows;

namespace CPS2;

public partial class GenreEditWindow : Window
{
    public Genre Genre { get; set; }

    public GenreEditWindow()
    {
        InitializeComponent();
        Genre = new Genre();
    }

    private void SaveButton_Click(object sender, RoutedEventArgs e)
    {
        if (string.IsNullOrWhiteSpace(GenreNameTextBox.Text))
        {
            MessageBox.Show("Введите название жанра");
            return;
        }
        
        Genre.GenreName = GenreNameTextBox.Text;
        DialogResult = true;
        Close();
    }
}
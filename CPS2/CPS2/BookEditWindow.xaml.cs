using System.Windows;

namespace CPS2;

public partial class BookEditWindow : Window
{
    public Book Book { get; set; } = new Book();

    public BookEditWindow()
    {
        InitializeComponent();
        DataContext = this;
    }

    private void SaveButton_Click(object sender, RoutedEventArgs e)
    {
        if (string.IsNullOrWhiteSpace(Book.Title))
        {
            MessageBox.Show("Введите название книги");
            return;
        }
            
        if (Book.PublicationYear < 1 || Book.PublicationYear > DateTime.Now.Year)
        {
            MessageBox.Show("Некорректный год издания");
            return;
        }
            
        DialogResult = true;
        Close();
    }
}
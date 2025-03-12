using System.Windows;

namespace CPS2;

public partial class SeriesEditWindow : Window
{
    public Series Series { get; set; } = new Series();

    public SeriesEditWindow()
    {
        InitializeComponent();
        DataContext = this;
    }

    private void SaveButton_Click(object sender, RoutedEventArgs e)
    {
        if (string.IsNullOrWhiteSpace(Series.SeriesName))
        {
            MessageBox.Show("Введите название серии");
            return;
        }
        DialogResult = true;
        Close();
    }
}
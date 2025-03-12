using System.Windows.Controls;

namespace CPS2
{
    public partial class GenreDetailsPage : Page
    {
        public GenreDetailsPage(Genre genre)
        {
            InitializeComponent();
            DataContext = genre;
        }
    }
}
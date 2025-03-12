using System.Windows.Controls;

namespace CPS2
{
    public partial class BookDetailsPage : Page
    {
        public BookDetailsPage(Book book)
        {
            InitializeComponent();
            DataContext = book;
        }
    }
}
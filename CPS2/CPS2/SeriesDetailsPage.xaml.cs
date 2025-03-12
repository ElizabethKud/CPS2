using System.Windows.Controls;

namespace CPS2
{
    public partial class SeriesDetailsPage : Page
    {
        public SeriesDetailsPage(Series series)
        {
            InitializeComponent();
            DataContext = series;
        }
    }
}
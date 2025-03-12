using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace CPS2
{
    public partial class MainWindow : Window
    {
        private readonly User _currentUser;

        public MainWindow(User user)
        {
            InitializeComponent();
            _currentUser = user;
            LoadData();
            SetupPermissions();
        }

        private void SetupPermissions()
        {
            if (_currentUser?.Role != "admin")
            {
                var contextMenu = HierarchyTreeView.ContextMenu;
                if (contextMenu != null)
                {
                    foreach (var item in contextMenu.Items.OfType<MenuItem>())
                    {
                        if (item.Header?.ToString() != "Добавить книгу")
                            item.Visibility = Visibility.Collapsed;
                    }
                }
            }
        }


        private void LoadData()
        {
            using var db = new AppDbContext();
            var genres = db.Genres
                .Include(g => g.Series)
                .ThenInclude(s => s.Books)
                .ToList();

            if (genres == null)
            {
                MessageBox.Show("Не удалось загрузить жанры");
                return;
            }

            HierarchyTreeView.ItemsSource = genres;
        }


        #region Обработчики контекстного меню

        // Добавление жанра
        private void AddGenre_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new GenreEditWindow();
            if (dialog.ShowDialog() == true)
            {
                using var db = new AppDbContext();
                db.Genres.Add(dialog.Genre);
                db.SaveChanges();
                LoadData();
            }
        }

        // Добавление серии
        private void AddSeries_Click(object sender, RoutedEventArgs e)
        {
            if (HierarchyTreeView.SelectedItem is Genre selectedGenre)
            {
                var dialog = new SeriesEditWindow();
                if (dialog.ShowDialog() == true)
                {
                    using var db = new AppDbContext();
                    dialog.Series.GenreId = selectedGenre.Id;
                    db.Series.Add(dialog.Series);
                    db.SaveChanges();
                    LoadData();
                }
            }
        }

        // Добавление книги
        private void AddBook_Click(object sender, RoutedEventArgs e)
        {
            if (HierarchyTreeView.SelectedItem is Series selectedSeries)
            {
                var dialog = new BookEditWindow();
                if (dialog.ShowDialog() == true)
                {
                    using var db = new AppDbContext();
                    dialog.Book.SeriesId = selectedSeries.Id;
                    db.Books.Add(dialog.Book);
                    db.SaveChanges();
                    LoadData();
                }
            }
        }

        // Удаление выбранного элемента
        private void DeleteItem_Click(object sender, RoutedEventArgs e)
        {
            var selectedItem = HierarchyTreeView.SelectedItem;
            if (selectedItem == null) return;

            using var db = new AppDbContext();
            switch (selectedItem)
            {
                case Genre genre:
                    db.Genres.Remove(genre);
                    break;
                case Series series:
                    db.Series.Remove(series);
                    break;
                case Book book:
                    db.Books.Remove(book);
                    break;
                default:
                    return;
            }
    
            try
            {
                db.SaveChanges();
                LoadData();
            }
            catch (DbUpdateException ex)
            {
                MessageBox.Show($"Ошибка удаления: {ex.InnerException?.Message}");
            }
        }

        // Редактирование выбранного элемента
        private void EditItem_Click(object sender, RoutedEventArgs e)
        {
            dynamic selectedItem = HierarchyTreeView.SelectedItem;
            if (selectedItem == null) return;

            switch (selectedItem)
            {
                case Genre genre:
                    var genreDialog = new GenreEditWindow { Genre = genre };
                    if (genreDialog.ShowDialog() == true)
                    {
                        using var db = new AppDbContext();
                        db.Genres.Update(genre);
                        db.SaveChanges();
                        LoadData();
                    }
                    break;

                case Series series:
                    var seriesDialog = new SeriesEditWindow { Series = series };
                    if (seriesDialog.ShowDialog() == true)
                    {
                        using var db = new AppDbContext();
                        db.Series.Update(series);
                        db.SaveChanges();
                        LoadData();
                    }
                    break;

                case Book book:
                    var bookDialog = new BookEditWindow { Book = book };
                    if (bookDialog.ShowDialog() == true)
                    {
                        using var db = new AppDbContext();
                        db.Books.Update(book);
                        db.SaveChanges();
                        LoadData();
                    }
                    break;
            }
        }

        #endregion

        #region Drag&Drop

        // Обработчик перетаскивания элементов
        private void TreeViewItem_PreviewMouseMove(object sender, MouseEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed &&
                sender is TreeViewItem item &&
                item.DataContext is Book book)
            {
                DragDrop.DoDragDrop(item, book, DragDropEffects.Move);
            }
        }

        // Обработчик отпускания элемента
        private void TreeViewItem_Drop(object sender, DragEventArgs e)
        {
            if (e.Data.GetData(typeof(Book)) is Book draggedBook &&
                ((TreeViewItem)sender).DataContext is Series targetSeries)
            {
                using var db = new AppDbContext();
                draggedBook.SeriesId = targetSeries.Id;
                db.Books.Update(draggedBook);
                db.SaveChanges();
                LoadData();
            }
        }

        #endregion
    }
}

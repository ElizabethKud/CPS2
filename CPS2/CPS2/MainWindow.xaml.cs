using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace CPS2
{
    public partial class MainWindow : Window
    {
        private readonly User _currentUser;
        private AppDbContext _dbContext;

        public MainWindow(User user)
        {
            InitializeComponent();
            _currentUser = user;
            _dbContext = new AppDbContext();
            SetupPermissions();
            LoadData();
        }

        private void SetupPermissions()
        {
            if (_currentUser.Role != "admin")
            {
                AdminMenu.Visibility = Visibility.Collapsed;
                TreeContextMenu.Visibility = Visibility.Collapsed; // Скрываем меню полностью
            }
            else
            {
                TreeContextMenu.Visibility = Visibility.Visible;
            }
        }

        private void LoadData()
        {
            _dbContext.Genres
                .Include(g => g.Series)
                .ThenInclude(s => s.Books)
                .Load();

            HierarchyTreeView.ItemsSource = _dbContext.Genres.Local.ToObservableCollection();

            // Добавляем обработчики к каждому TreeViewItem
            AddEventHandlers(HierarchyTreeView);
        }

        // Добавляем обработчики для всех элементов дерева
        private void AddEventHandlers(ItemsControl parent)
        {
            foreach (var item in parent.Items)
            {
                if (parent.ItemContainerGenerator.ContainerFromItem(item) is TreeViewItem treeViewItem)
                {
                    treeViewItem.PreviewMouseMove += TreeViewItem_PreviewMouseMove;
                    treeViewItem.Drop += TreeViewItem_Drop;
                    treeViewItem.DragOver += TreeViewItem_DragOver;
                    treeViewItem.AllowDrop = true;

                    // Рекурсия для всех типов
                    if (item is Genre || item is Series)
                    {
                        AddEventHandlers(treeViewItem);
                    }
                }
            }
        }
        
        private void TreeView_PreviewMouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            var treeViewItem = ((DependencyObject)e.OriginalSource).GetSelfAndAncestors<TreeViewItem>().FirstOrDefault();
            if (treeViewItem != null) treeViewItem.IsSelected = true;
        }

        private void ManageUsers_Click(object sender, RoutedEventArgs e)
        {
            new UserManagementWindow().ShowDialog();
        }

        private void Exit_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

        #region Обработчики контекстного меню

        // Добавление жанра
        // CRUD операции и обработчики событий
        private void AddGenre_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new GenreEditWindow();
            if (dialog.ShowDialog() == true)
            {
                _dbContext.Genres.Add(dialog.Genre);
                _dbContext.SaveChanges();
                // Обновление дерева без перезагрузки всех данных
                HierarchyTreeView.ItemsSource = _dbContext.Genres.Local.ToObservableCollection();
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
                    // Обновление дерева без перезагрузки всех данных
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
                    // Обновление дерева без перезагрузки всех данных
                    LoadData();
                }
            }
        }

        // Удаление выбранного элемента
        private void DeleteItem_Click(object sender, RoutedEventArgs e)
        {
            var selectedItem = HierarchyTreeView.SelectedItem;
            if (selectedItem == null) return;

            switch (selectedItem)
            {
                case Genre genre:
                    _dbContext.Series.RemoveRange(genre.Series);  // Удаляем все серии этого жанра
                    foreach (var series in genre.Series)
                    {
                        _dbContext.Books.RemoveRange(series.Books);  // Удаляем все книги в этих сериях
                    }
                    _dbContext.Genres.Remove(genre);
                    break;

                case Series series:
                    _dbContext.Books.RemoveRange(series.Books);  // Удаляем книги этой серии
                    _dbContext.Series.Remove(series);
                    break;

                case Book book:
                    _dbContext.Books.Remove(book);
                    break;
            }

            _dbContext.SaveChanges();
            LoadData();  // Перегружаем данные, чтобы TreeView обновился
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
                        // Обновление дерева без перезагрузки всех данных
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
                        // Обновление дерева без перезагрузки всех данных
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
                        // Обновление дерева без перезагрузки всех данных
                        LoadData();
                    }
                    break;
            }
        }
        
        // Для открытия подробностей книги, серии или жанра
        private void HierarchyTreeView_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            if (e.NewValue is Book selectedBook)
            {
                // Открываем страницу с деталями книги
                DetailsFrame.Content = new BookDetailsPage(selectedBook);
            }
            else if (e.NewValue is Series selectedSeries)
            {
                // Открываем страницу с деталями серии
                DetailsFrame.Content = new SeriesDetailsPage(selectedSeries);
            }
            else if (e.NewValue is Genre selectedGenre)
            {
                // Открываем страницу с деталями жанра
                DetailsFrame.Content = new GenreDetailsPage(selectedGenre);
            }
        }

        #endregion

        #region Drag&Drop

        // Обработчик перетаскивания элементов
        private object _draggedItem;

        // Обработчик перетаскивания элементов (книги и серии)
        private void TreeViewItem_PreviewMouseMove(object sender, MouseEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed && sender is TreeViewItem item)
            {
                if (item.DataContext is Book book)
                {
                    _draggedItem = book;
                    DragDrop.DoDragDrop(item, new DataObject(typeof(Book), book), DragDropEffects.Move);
                    e.Handled = true;
                }
                else if (item.DataContext is Series series)
                {
                    _draggedItem = series;
                    DragDrop.DoDragDrop(item, new DataObject(typeof(Series), series), DragDropEffects.Move);
                    e.Handled = true;
                }
            }
        }

        // Обработка перетаскивания
        private void TreeViewItem_DragOver(object sender, DragEventArgs e)
        {
            var targetItem = sender as TreeViewItem;
            var sourceData = _draggedItem;
            var targetData = targetItem?.DataContext;

            // Проверка возможности переноса
            if (CanDrop(sourceData, targetData))
            {
                e.Effects = DragDropEffects.Move;
                targetItem.Background = Brushes.LightBlue;
            }
            else
            {
                e.Effects = DragDropEffects.None;
            }

            e.Handled = true;
        }

        // Перемещение элемента в новую категорию
        private void TreeViewItem_Drop(object sender, DragEventArgs e)
        {
            var treeViewItem = sender as TreeViewItem;
            var target = treeViewItem?.DataContext;

            if (!CanDrop(_draggedItem, target))
            {
                if (treeViewItem != null) treeViewItem.Background = Brushes.Transparent;
                return;
            }

            try
            {
                if (_draggedItem is Book draggedBook)
                {
                    if (target is Series targetSeries)
                    {
                        // Получаем старую серию
                        var oldSeries = draggedBook.Series;

                        if (oldSeries != null)
                        {
                            // Удаляем книгу из старой серии
                            oldSeries.Books.Remove(draggedBook);
                        }

                        // Обновляем связь через EF Core
                        draggedBook.SeriesId = targetSeries.Id;

                        // Добавляем книгу в новую серию
                        targetSeries.Books.Add(draggedBook);

                        _dbContext.Entry(draggedBook).State = EntityState.Modified;
                        _dbContext.SaveChanges();
                    }
                }
                else if (_draggedItem is Series draggedSeries)
                {
                    if (target is Genre targetGenre)
                    {
                        // Получаем старый жанр
                        var oldGenre = draggedSeries.Genre;

                        if (oldGenre != null)
                        {
                            // Удаляем серию из старого жанра
                            oldGenre.Series.Remove(draggedSeries);
                        }

                        // Обновляем связь через EF Core
                        draggedSeries.GenreId = targetGenre.Id;

                        // Добавляем серию в новый жанр
                        targetGenre.Series.Add(draggedSeries);

                        _dbContext.Entry(draggedSeries).State = EntityState.Modified;
                        _dbContext.SaveChanges();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка: {ex.Message}");
            }
            finally
            {
                if (treeViewItem != null)
                    treeViewItem.Background = Brushes.Transparent;

                _draggedItem = null;
                e.Handled = true;
            }
        }

        // Функция проверки возможности перетаскивания
        private bool CanDrop(object source, object target)
        {
            if (source is Book book && target is Series targetSeries)
            {
                // Разрешаем перенос книги в другую серию
                return book.Series?.Id != targetSeries.Id;
            }

            if (source is Series series && target is Genre targetGenre)
            {
                // Разрешаем перенос серии в другой жанр
                return series.Genre?.Id != targetGenre.Id;
            }

            return false;
        }
        
        #endregion
        
        protected override void OnClosed(EventArgs e)
        {
            _dbContext.Dispose();
            base.OnClosed(e);
        }
    }
}

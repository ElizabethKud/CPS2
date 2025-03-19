using System.Collections.ObjectModel;
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
                TreeContextMenu.Items.Clear();
            }
        }

        private void LoadData()
        {
            // Загружаем все данные одним запросом с включением связанных сущностей
            _dbContext.Genres
                .Include(g => g.Series)
                .ThenInclude(s => s.Books)
                .Load();

            // Устанавливаем источник данных для TreeView
            HierarchyTreeView.ItemsSource = _dbContext.Genres.Local.ToObservableCollection();

            // Добавляем обработчики для каждого элемента дерева
            AddEventHandlers(HierarchyTreeView);
        }

        // Рекурсивно добавляем обработчики ко всем элементам дерева
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
                    treeViewItem.IsExpanded = true;

                    if (item is Genre genre)
                    {
                        // Рекурсивная обработка для серий
                        AddEventHandlers(treeViewItem);
                    }
                    else if (item is Series series)
                    {
                        // Рекурсивная обработка для книг
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
                UpdateTreeViewItem(dialog.Genre);
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
                    UpdateTreeViewItem(dialog.Series);
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
                    UpdateTreeViewItem(dialog.Book);
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
                        UpdateTreeViewItem(genre);
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
                        UpdateTreeViewItem(series);
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
                        UpdateTreeViewItem(book);
                    }
                    break;
            }
        }
        
        private void UpdateTreeViewItem(object updatedItem)
        {
            var items = HierarchyTreeView.ItemsSource as ObservableCollection<Genre>;

            if (updatedItem is Genre updatedGenre)
            {
                var existingGenre = items.FirstOrDefault(g => g.Id == updatedGenre.Id);
                if (existingGenre != null)
                {
                    var index = items.IndexOf(existingGenre);
                    items.RemoveAt(index);
                    items.Insert(index, updatedGenre);
                }
                else
                {
                    items.Add(updatedGenre);
                }
            }
            else if (updatedItem is Series updatedSeries)
            {
                var parentGenre = items.FirstOrDefault(g => g.Series.Any(s => s.Id == updatedSeries.Id));
                if (parentGenre != null)
                {
                    parentGenre.Series.Remove(updatedSeries);
                    var newParent = items.FirstOrDefault(g => g.Id == updatedSeries.GenreId);
                    newParent?.Series.Add(updatedSeries);
                }
            }
            else if (updatedItem is Book updatedBook)
            {
                var parentSeries = items.SelectMany(g => g.Series)
                    .FirstOrDefault(s => s.Books.Any(b => b.Id == updatedBook.Id));
                parentSeries?.Books.Remove(updatedBook);
                var newParentSeries = items.SelectMany(g => g.Series)
                    .FirstOrDefault(s => s.Id == updatedBook.SeriesId);
                newParentSeries?.Books.Add(updatedBook);
            }

            // Принудительное обновление дерева
            HierarchyTreeView.Items.Refresh();
            CommandManager.InvalidateRequerySuggested();
        }
        
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

        private void TreeViewItem_PreviewMouseMove(object sender, MouseEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed && sender is TreeViewItem item)
            {
                // Убеждаемся, что элемент развернут
                item.IsExpanded = true;
        
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

        private void TreeViewItem_DragOver(object sender, DragEventArgs e)
        {
            var targetItem = sender as TreeViewItem;
            var sourceData = _draggedItem;
            var targetData = targetItem?.DataContext;

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

        private void TreeViewItem_Drop(object sender, DragEventArgs e)
        {
            var targetItem = sender as TreeViewItem;
            var target = targetItem?.DataContext;

            if (!CanDrop(_draggedItem, target))
            {
                if (targetItem != null) targetItem.Background = Brushes.Transparent;
                return;
            }

            try
            {
                if (_draggedItem is Book draggedBook)
                {
                    if (target is Series targetSeries)
                    {
                        var oldSeries = draggedBook.Series;
                        oldSeries?.Books.Remove(draggedBook);
                
                        draggedBook.Series = targetSeries;
                        draggedBook.SeriesId = targetSeries.Id;
                        targetSeries.Books.Add(draggedBook);

                        UpdateTreeViewItem(targetSeries);
                        UpdateTreeViewItem(oldSeries);
                    }
                }
                else if (_draggedItem is Series draggedSeries)
                {
                    if (target is Genre targetGenre)
                    {
                        var oldGenre = draggedSeries.Genre;
                        oldGenre?.Series.Remove(draggedSeries);
                
                        draggedSeries.Genre = targetGenre;
                        draggedSeries.GenreId = targetGenre.Id;
                        targetGenre.Series.Add(draggedSeries);

                        UpdateTreeViewItem(oldGenre);
                        UpdateTreeViewItem(targetGenre);
                    }
                }

                _dbContext.SaveChanges();
                _dbContext.ChangeTracker.Entries()
                    .Where(entry => entry.Entity != null)
                    .ToList()
                    .ForEach(entry => entry.Reload());
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка: {ex.Message}");
            }
    
            if (targetItem != null)
                targetItem.Background = Brushes.Transparent;
    
            _draggedItem = null;
            e.Handled = true;
        }

        private bool CanDrop(object source, object target)
        {
            return (source is Book && target is Series) ||
                   (source is Series && target is Genre);
        }
        
        #endregion
        
        protected override void OnClosed(EventArgs e)
        {
            _dbContext.Dispose();
            base.OnClosed(e);
        }
    }
}

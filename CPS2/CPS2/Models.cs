using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.CompilerServices;

namespace CPS2
{
    [Table("genres")]
    public class Genre : INotifyPropertyChanged
    {
        private string _genreName;

        [Column("id")]
        public int Id { get; set; }

        [Column("genre_name")]
        public string GenreName
        {
            get => _genreName;
            set { _genreName = value; OnPropertyChanged(); }
        }

        public virtual ObservableCollection<Series> Series { get; set; } = new();

        public event PropertyChangedEventHandler? PropertyChanged;
        protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    [Table("series")]
    public class Series
    {
        [Column("id")] public int Id { get; set; }
        [Column("series_name")] public string SeriesName { get; set; }
        [Column("genre_id")] public int GenreId { get; set; }
        public Genre? Genre { get; set; }
        public List<Book> Books { get; set; } = new();
    }

    [Table("books")]
    public class Book
    {
        [Column("id")] public int Id { get; set; }
        [Column("title")] public string? Title { get; set; }
        [Column("series_id")] public int SeriesId { get; set; }
        [Column("publication_year")] public int PublicationYear { get; set; }
        [Column("description")] public string? Description { get; set; }
        public Series? Series { get; set; }
    }

    [Table("users")]
    public class User
    {
        [Column("id")] public int Id { get; set; }
        [Column("username")] public string? Username { get; set; }
        [Column("password_hash")] public string? PasswordHash { get; set; }
        
        // Преобразуем в UTC перед сохранением в БД
        [Column("registration_date")]
        public DateTime RegistrationDate 
        { 
            get => _registrationDate.ToUniversalTime();  // Возвращаем в формате UTC
            set => _registrationDate = value.ToUniversalTime(); // Преобразуем в UTC при записи
        }
        private DateTime _registrationDate;
        
        [Column("last_login")] public DateTime LastLogin { get; set; }

        [Column("is_active")] public bool IsActive { get; set; }
        [Column("role")] public string? Role { get; set; }
    }
}
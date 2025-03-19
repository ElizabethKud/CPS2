using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.CompilerServices;

namespace CPS2
{
    [Table("genres")]
    public class Genre : INotifyPropertyChanged
    {
        private string _genreName;

        [Key, Column("id")]
        public int Id { get; set; }

        [Required, Column("genre_name")]
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
    public class Series : INotifyPropertyChanged
    {
        private string _seriesName;
        
        [Key, Column("id")]
        public int Id { get; set; }
        
        [Required, Column("series_name")]
        public string SeriesName
        {
            get => _seriesName;
            set { _seriesName = value; OnPropertyChanged(); }
        }
        
        [Column("genre_id"), ForeignKey("Genre")]
        public int GenreId { get; set; }
        
        public virtual Genre? Genre { get; set; }
        public virtual ObservableCollection<Book> Books { get; set; } = new();
        
        public event PropertyChangedEventHandler? PropertyChanged;
        protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    [Table("books")]
    public class Book : INotifyPropertyChanged
    {
        private string? _title;
        private string? _description;
        private int _publicationYear;
        
        [Key, Column("id")]
        public int Id { get; set; }
        
        [Column("title")]
        public string? Title
        {
            get => _title;
            set { _title = value; OnPropertyChanged(); }
        }
        
        [Column("series_id"), ForeignKey("Series")]
        public int SeriesId { get; set; }
        
        [Column("publication_year")]
        public int PublicationYear
        {
            get => _publicationYear;
            set { _publicationYear = value; OnPropertyChanged(); }
        }
        
        [Column("description")]
        public string? Description
        {
            get => _description;
            set { _description = value; OnPropertyChanged(); }
        }
        
        public virtual Series? Series { get; set; }
        
        public event PropertyChangedEventHandler? PropertyChanged;
        protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    [Table("users")]
    public class User : INotifyPropertyChanged
    {
        private string? _username;
        private string? _passwordHash;
        private string? _role;
        private bool _isActive;
        private DateTime _registrationDate;
        private DateTime _lastLogin;

        [Key, Column("id")]
        public int Id { get; set; }

        [Required, Column("username")]
        public string? Username
        {
            get => _username;
            set { _username = value; OnPropertyChanged(); }
        }

        [Required, Column("password_hash")]
        public string? PasswordHash
        {
            get => _passwordHash;
            set { _passwordHash = value; OnPropertyChanged(); }
        }

        [Column("registration_date")]
        public DateTime RegistrationDate
        {
            get => _registrationDate.ToUniversalTime();
            set { _registrationDate = value.ToUniversalTime(); OnPropertyChanged(); }
        }

        [Column("last_login")]
        public DateTime LastLogin
        {
            get => _lastLogin;
            set { _lastLogin = value; OnPropertyChanged(); }
        }

        [Column("is_active")]
        public bool IsActive
        {
            get => _isActive;
            set { _isActive = value; OnPropertyChanged(); }
        }

        [Column("role")]
        public string? Role
        {
            get => _role;
            set { _role = value; OnPropertyChanged(); }
        }
        
        public event PropertyChangedEventHandler? PropertyChanged;
        protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
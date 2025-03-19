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
            set
            {
                _genreName = value;
                OnPropertyChanged();
            }
        }

        public virtual ObservableCollection<Series> Series { get; set; } = new();

        public event PropertyChangedEventHandler? PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }

    [Table("series")]
    public class Series : INotifyPropertyChanged
    {
        private string _seriesName;
        private Genre? _genre;
        private int _genreId;

        [Key, Column("id")]
        public int Id { get; set; }

        [Required, Column("series_name")]
        public string SeriesName
        {
            get => _seriesName;
            set
            {
                _seriesName = value;
                OnPropertyChanged();
            }
        }

        [Column("genre_id"), ForeignKey("Genre")]
        public int GenreId
        {
            get => _genreId;
            set
            {
                if (_genreId != value)
                {
                    _genreId = value;
                    OnPropertyChanged();
                }
            }
        }

        public virtual Genre? Genre
        {
            get => _genre;
            set
            {
                if (_genre != value)
                {
                    _genre = value;
                    OnPropertyChanged();
                }
            }
        }

        public virtual ObservableCollection<Book> Books { get; set; } = new();

        public event PropertyChangedEventHandler? PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }

    [Table("books")]
    public class Book : INotifyPropertyChanged
    {
        private string? _author;
        private string? _title;
        private string? _description;
        private int _publicationYear;
        private Series? _series;
        private int _seriesId;

        [Key, Column("id")]
        public int Id { get; set; }

        [Column("title")]
        public string? Title
        {
            get => _title;
            set
            {
                _title = value;
                OnPropertyChanged();
            }
        }

        [Column("author")] // Добавьте поле для автора
        public string? Author
        {
            get => _author;
            set
            {
                _author = value;
                OnPropertyChanged();
            }
        }

        [Column("series_id"), ForeignKey("Series")]
        public int SeriesId
        {
            get => _seriesId;
            set
            {
                if (_seriesId != value)
                {
                    _seriesId = value;
                    OnPropertyChanged();
                    OnPropertyChanged(nameof(Series)); // Важно обновить и Series
                }
            }
        }

        public virtual Series? Series
        {
            get => _series;
            set
            {
                if (_series != value)
                {
                    _series = value;
                    OnPropertyChanged();
                }
            }
        }

        [Column("publication_year")]
        public int PublicationYear
        {
            get => _publicationYear;
            set
            {
                _publicationYear = value;
                OnPropertyChanged();
            }
        }

        [Column("description")]
        public string? Description
        {
            get => _description;
            set
            {
                _description = value;
                OnPropertyChanged();
            }
        }


        public event PropertyChangedEventHandler? PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
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
            set
            {
                _username = value;
                OnPropertyChanged();
            }
        }

        [Required, Column("password_hash")]
        public string? PasswordHash
        {
            get => _passwordHash;
            set
            {
                _passwordHash = value;
                OnPropertyChanged();
            }
        }

        [Column("registration_date")]
        public DateTime RegistrationDate
        {
            get => _registrationDate; 
            set
            {
                // Проверка на некорректные значения
                if (value == DateTime.MinValue || value == DateTime.MaxValue)
                {
                    _registrationDate = DateTime.UtcNow; // Устанавливаем текущую дату в случае некорректного значения
                }
                else
                {
                    _registrationDate = value;
                }
                OnPropertyChanged();
            }
        }

        [Column("last_login")]
        public DateTime LastLogin
        {
            get => _lastLogin;
            set
            {
                _lastLogin = value;
                OnPropertyChanged();
            }
        }

        [Column("is_active")]
        public bool IsActive
        {
            get => _isActive;
            set
            {
                _isActive = value;
                OnPropertyChanged();
            }
        }

        [Column("role")]
        public string? Role
        {
            get => _role;
            set
            {
                _role = value;
                OnPropertyChanged();
            }
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
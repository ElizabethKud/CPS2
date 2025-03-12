namespace CPS2;

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

public class Genre
{
    public int Id { get; set; }
    
    [Required]
    [StringLength(255)]
    public string GenreName { get; set; }
    
    public ICollection<Series> Series { get; set; } = new List<Series>();
}

public class Series
{
    public int Id { get; set; }
    
    [Required]
    [StringLength(255)]
    public string SeriesName { get; set; }
    
    public int GenreId { get; set; }
    public Genre Genre { get; set; }
    
    public ICollection<Book> Books { get; set; } = new List<Book>();
}

public class Book
{
    public int Id { get; set; }
    
    [Required]
    [StringLength(255)]
    public string Title { get; set; }
    
    public int SeriesId { get; set; }
    public Series Series { get; set; }
    
    [Range(1, 2024)]
    public int PublicationYear { get; set; }
    
    public string Description { get; set; }
}

public class User
{
    public int Id { get; set; }
    
    [Required]
    [StringLength(100)]
    public string Username { get; set; }
    
    [Required]
    public string PasswordHash { get; set; }
    
    [Required]
    public string Salt { get; set; }
    
    public DateTime RegistrationDate { get; set; }
    
    public bool IsActive { get; set; }
    
    [Required]
    public string Role { get; set; }
}
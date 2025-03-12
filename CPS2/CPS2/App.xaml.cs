using System.Configuration;
using System.Data;
using System.Windows;

namespace CPS2;

public partial class App : Application
{
    protected override void OnStartup(StartupEventArgs e)
    {
        base.OnStartup(e);
            
        // Инициализация базы данных
        using var db = new AppDbContext();
        db.Database.EnsureCreated();
    }
}
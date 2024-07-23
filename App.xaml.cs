using System.Configuration;
using System.Data;
using System.Windows;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.Windows;
using game_tools.Helpers;
using Microsoft.EntityFrameworkCore;
using Microsoft.Data.Sqlite;
using game_tools.Views;


namespace game_tools
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {

        private IHost _host;

        public App()
        {
            _host = Host.CreateDefaultBuilder()
                .ConfigureServices((context, services) =>
                {
                    var sqliteConnectionString = new SqliteConnectionStringBuilder("Filename=evaluate.db")
                    {
                        Mode = SqliteOpenMode.ReadWriteCreate,
                        Password = "123456",
                        //DataSource = "resource/t1.db", // 数据库文件路径
                    }.ToString();
                    services.AddDbContext<SQLiteDb>(options =>
                        options.UseSqlite(sqliteConnectionString));

                    // Register other services
                    services.AddTransient<MainWindow>();
                })
                .Build();
        }

        protected override void OnStartup(StartupEventArgs e)
        {
            _host.Start();
            var mainWindow = _host.Services.GetRequiredService<MainWindow>();
            mainWindow.Show();
            base.OnStartup(e);
        }

        protected override void OnExit(ExitEventArgs e)
        {
            _host.StopAsync().Wait();
            _host.Dispose();
            base.OnExit(e);
        }
    }

}

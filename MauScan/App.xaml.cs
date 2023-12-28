using MauScan.Data;

namespace MauScan
{
    public partial class App : Application
    {
        public static Database Database { get; set; }
        public App(Database database)
        {
            InitializeComponent();

            MainPage = new AppShell();

            Database = database;
        }
    }
}

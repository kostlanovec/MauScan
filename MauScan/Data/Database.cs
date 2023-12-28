using MAUISql.Models;
using SQLite;

namespace MauScan.Data
{
    public class Database
    {
        string _dbPath;
        private SQLiteConnection conn;

        public Database(string dbPath)
        {
            _dbPath = dbPath;
        }

        public void Init()
        {
            conn = new SQLiteConnection(_dbPath);
            conn.CreateTable<QRCode>();
        }

        public List<QRCode> GetQRCode()
        {
            return conn.Table<QRCode>().ToList();
        }

        public void addQRCode(QRCode qrCode)
        {
            conn = new SQLiteConnection(_dbPath);
            conn.Insert(qrCode);
        }

        public void deleteQRCode(QRCode qrCode)
        {
            conn = new SQLiteConnection(_dbPath);
            conn.Delete(qrCode);
        }

    }
}

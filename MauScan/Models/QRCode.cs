using SQLite;

namespace MAUISql.Models
{
    [Table("QRCode")]
    public class QRCode
    {
        [PrimaryKey, AutoIncrement, Column("Id")]
        public int Id { get; set; }
        public DateTime TimeScan { get; set; }

        public string Text { get; set; }

    }
}
// ============================================================
// الملف    : IMigration.cs
// الغرض    : واجهة برمجية لتعريف عقود ترحيل قاعدة البيانات (Migrations)
// ============================================================

using System.Data.SQLite;

namespace Sales.Database.Migrations
{
    public interface IMigration
    {
        int Version { get; }
        string Description { get; }
        void Apply(SQLiteConnection con);
    }
}

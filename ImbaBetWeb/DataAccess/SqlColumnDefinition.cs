using System.Data;

namespace ImbaBetWeb.DataAccess
{
    public class SqlColumnDefinition(string columnName, SqlDbType dbType, int size = 0, bool isPrimary = false, bool isNullable = false)
    {
        public string ColumnName { get; } = columnName;
        public SqlDbType DbType { get; } = dbType;
        public int Size { get; } = size;
        public bool IsPrimaryKey { get; set; } = isPrimary;
        public bool IsNullable { get; set; } = isNullable;
    }
}

using SQL.Shaper.Common;
using SQL.Shaper.Interfaces;

namespace SQL.Shaper.Builder;

public partial class SqlQueryBuilder
{
    public IQueryBuilder Delete(string tableName)
    {
        AppendLine($"{SqlKeywords.Delete} {SqlKeywords.From} {FormatSelector(tableName)} ");
        return this;
    }

    public IQueryBuilder Delete<TEntity>()
    {
        var type = typeof(TEntity);
        return Delete(type.Name);
    }
}
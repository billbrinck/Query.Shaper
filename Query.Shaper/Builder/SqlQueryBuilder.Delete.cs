using Query.Shaper.Common;
using Query.Shaper.Interfaces;

namespace Query.Shaper.Builder;

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
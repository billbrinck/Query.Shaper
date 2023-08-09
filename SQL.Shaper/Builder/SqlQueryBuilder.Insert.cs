using SQL.Shaper.Common;
using SQL.Shaper.Interfaces;

namespace SQL.Shaper.Builder;

public partial class SqlQueryBuilder
{
    public IQueryBuilder Insert<TEntity>(TEntity entity, string idColumnName = DefaultIdColumnName)
    {
        var type = typeof(TEntity);
        var properties = type.GetProperties();
        return Insert(entity, idColumnName, properties.Select(e => e.Name).ToArray());
    }

    public IQueryBuilder Insert<TEntity>(TEntity entity, string idColumnName = DefaultIdColumnName,
        params string[] columnNames)
    {
        var type = typeof(TEntity);
        return Insert(entity, type.Name, idColumnName, columnNames);
    }

    public IQueryBuilder Insert<TEntity>(TEntity entity, string tableName, string idColumnName = DefaultIdColumnName,
        params string[] columnNames)
    {
        var type = typeof(TEntity);
        AppendLine($"{SqlKeywords.Insert} {SqlKeywords.Into} {tableName}({FormatSelectors(columnNames)}) ");
        _insertColumnNames = columnNames;

        OutputInserted(idColumnName);

        return Values(columnNames.Select(colName => type.GetProperty(colName)?.GetValue(entity)).ToArray());
    }

    public IQueryBuilder Insert<TEntity>(IEnumerable<TEntity> entities)
    {
        var type = typeof(TEntity);
        var properties = type.GetProperties();
        return Insert(entities, properties.Select(e => e.Name).ToArray());
    }

    public IQueryBuilder Insert<TEntity>(IEnumerable<TEntity> entities, params string[] columnNames)
    {
        var type = typeof(TEntity);
        return Insert(entities, type.Name, columnNames);
    }

    public IQueryBuilder Insert<TEntity>(IEnumerable<TEntity> entities, string tableName, params string[] columnNames)
    {
        var type = typeof(TEntity);
        AppendLine($"{SqlKeywords.Insert} {SqlKeywords.Into} {tableName}({FormatSelectors(columnNames)}) ");
        _insertColumnNames = columnNames;

        foreach (var entity in entities)
        {
            Values(columnNames.Select(colName => type.GetProperty(colName)?.GetValue(entity)).ToArray());
        }

        return this;
    }

}
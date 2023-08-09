using SQL.Shaper.Common;
using SQL.Shaper.Interfaces;

namespace SQL.Shaper.Builder;

public partial class SqlQueryBuilder
{
    private bool _updateSetAttached;

    // TODO: Give the option to build a Update query manually, or at least provide custom Where clauses

    public IQueryBuilder Update<TEntity>(TEntity entity, string idColumnName = DefaultIdColumnName)
    {
        var type = typeof(TEntity);
        var properties = type.GetProperties();

        return Update(entity, type.Name, idColumnName, columnNames: properties.Select(e => e.Name).ToArray());
    }

    public IQueryBuilder Update<TEntity>(TEntity entity, string tableName, string idColumnName = DefaultIdColumnName,
        params string[] columnNames) => Update(entity, tableName, idColumnName, default, columnNames);

    public IQueryBuilder Update<TEntity>(TEntity entity, string tableName, string idColumnName = DefaultIdColumnName,
        int? index = default, params string[] columnNames)
    {
        var type = typeof(TEntity);

        Append(SqlKeywords.Update);
        Append(FormatSelector(tableName));

        foreach (var columnName in columnNames)
        {
            Set(columnName, type.GetProperty(columnName)?.GetValue(entity), index);
        }

        return WhereEquals(idColumnName, type.GetProperty(idColumnName)?.GetValue(entity));
    }

    public IQueryBuilder Update<TEntity>(IEnumerable<TEntity> entities,
        string idColumnName = DefaultIdColumnName,
        params string[] columnNames)
    {
        var entitiesList = entities.ToList();
        var type = typeof(TEntity);

        for (var updateIndex = 0; updateIndex < entitiesList.Count; updateIndex++)
        {
            var entity = entitiesList.ElementAt(updateIndex);
            Update(entity, type.Name, idColumnName, updateIndex, columnNames);
            EndQuery();
        }

        return this;
    }

    public IQueryBuilder Update<TEntity>(IEnumerable<TEntity> entities, string tableName,
        string idColumnName = DefaultIdColumnName,
        params string[] columnNames)
    {
        var entitiesList = entities.ToList();
        for (var updateIndex = 0; updateIndex < entitiesList.Count; updateIndex++)
        {
            var entity = entitiesList.ElementAt(updateIndex);
            Update(entity, tableName, idColumnName, updateIndex, columnNames);
            EndQuery();
        }

        return this;
    }

    private void Set(string columnName, object? value, int? index = default)
    {
        if (!_updateSetAttached)
        {
            Append(SqlKeywords.Set);
            _updateSetAttached = true;
        }
        else Append(",");

        var formattedParameterName = FormatParameterName($"{columnName}{index}");

        Append($"{FormatSelector(columnName)} = {formattedParameterName}");

        AddParameter(formattedParameterName, value);
    }
}
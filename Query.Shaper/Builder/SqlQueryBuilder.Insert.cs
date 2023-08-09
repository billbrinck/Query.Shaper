using Query.Shaper.Common;
using Query.Shaper.Interfaces;

namespace Query.Shaper.Builder;

public partial class SqlQueryBuilder
{
    private string[] _insertColumnNames = Array.Empty<string>();
    private bool _insertValuesAttached;

    // When calling Values() we need to maintain a index so that we can add the parameters
    private int _insertValuesIndex;

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


    public IQueryBuilder Values(params object?[] values)
    {
        _insertValuesIndex++;

        if (!_insertValuesAttached)
        {
            Append(SqlKeywords.Values);

            _insertValuesAttached = true;
        }
        else
        {
            Append(",");
        }

        Append("(");
        try
        {
            for (var i = 0; i < _insertColumnNames.Length; i++)
            {
                var columnName = _insertColumnNames[i];

                var parameterName = $"{columnName}{_insertValuesIndex}";

                Append(parameterName);

                if (i < _insertColumnNames.Length - 1) Append(", ");

                AddParameter(parameterName, values[i]);
            }

            Append(")");
        }
        catch (IndexOutOfRangeException)
        {
            throw new Exception("Number of insert parameters do not match number of values provided");
        }

        return this;
    }
}
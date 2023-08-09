using SQL.Shaper.Common;
using SQL.Shaper.Interfaces;

namespace SQL.Shaper.Builder;

public partial class SqlQueryBuilder
{
    private bool _whereClauseAttached;

    public IQueryBuilder WhereIn<TEntity>(string columnName, ClauseOperator clauseOperator = ClauseOperator.And,
        params TEntity[] valueParams)
    {
        if (string.IsNullOrEmpty(columnName)) return this;

        SetWhere();

        var values = new List<TEntity>(valueParams);

        var suffix = $"{FormatSelector(columnName)} {SqlKeywords.In}(";
        for (var i = 0; i < values.Count; i++)
        {
            var paramName = $"wherein{columnName}{i}";
            AddParameter(paramName, values[i]);

            suffix += paramName;

            if (i < values.Count - 1) suffix += ",";
        }

        suffix += $") ";

        return Where(suffix, clauseOperator: clauseOperator);
    }

    public IQueryBuilder WhereNotIn<TEntity>(string columnName, ClauseOperator clauseOperator = ClauseOperator.And,
        params TEntity[] valueParams)
    {
        if (string.IsNullOrEmpty(columnName)) return this;

        SetWhere();

        var values = new List<TEntity>(valueParams);

        var suffix = $"{FormatSelector(columnName)} {SqlKeywords.Not} {SqlKeywords.In}(";
        for (var i = 0; i < values.Count; i++)
        {
            var paramName = $"wherenotin{columnName}{i}";
            AddParameter(paramName, values[i]);

            suffix += paramName;

            if (i < values.Count - 1) suffix += $",";
        }

        suffix += $") ";

        return Where(suffix, clauseOperator: clauseOperator);
    }

    private void SetWhere()
    {
        if (!_whereClauseAttached)
        {
            Append($"{Where}");
            _whereClauseAttached = true;
        }
    }

    public IQueryBuilder Where(string whereClause, object? value = default, string? parameterName = default,
        ClauseOperator clauseOperator = ClauseOperator.And)
    {
        if (_whereClauseAttached)
        {
            AppendLine(
                $"{(clauseOperator == ClauseOperator.Empty ? string.Empty : clauseOperator.ToString())}{whereClause}");
        }
        else
        {
            Append(SqlKeywords.Where);
            AppendLine(whereClause);
            _whereClauseAttached = true;
        }

        if (value != default && parameterName != default) AddParameter(parameterName, value);

        return this;
    }

    public IQueryBuilder WhereStringLike(string columnName, string value, string? parameterName = default,
        ClauseOperator clauseOperator = ClauseOperator.And)
    {
        if (string.IsNullOrEmpty(parameterName)) parameterName = columnName;
        var suffix =
            $"{FormatSelector(columnName)} {SqlKeywords.Like} {FormatParameterName(parameterName)}";
        return Where(suffix, value, parameterName, clauseOperator);
    }

    public IQueryBuilder WhereGreaterThan(string columnName, object value, string? parameterName = default,
        ClauseOperator clauseOperator = ClauseOperator.And)
    {
        if (string.IsNullOrEmpty(parameterName)) parameterName = columnName;
        var suffix = $"{FormatSelector(columnName)} > {FormatParameterName(parameterName)}";
        return Where(suffix, value, parameterName, clauseOperator);
    }

    public IQueryBuilder WhereLessThan(string columnName, object value, string? parameterName = default,
        ClauseOperator clauseOperator = ClauseOperator.And)
    {
        if (string.IsNullOrEmpty(parameterName)) parameterName = columnName;
        var suffix = $"{FormatSelector(columnName)} < {FormatParameterName(parameterName)}";
        return Where(suffix, value, parameterName, clauseOperator);
    }

    public IQueryBuilder WhereGreaterThanOrEquals(string columnName, object value, string? parameterName = default,
        ClauseOperator clauseOperator = ClauseOperator.And)
    {
        if (string.IsNullOrEmpty(parameterName)) parameterName = columnName;
        var suffix = $"{FormatSelector(columnName)} >= {FormatParameterName(parameterName)}";
        return Where(suffix, value, parameterName, clauseOperator);
    }

    public IQueryBuilder WhereLessThanOrEquals(string columnName, object value, string? parameterName = default,
        ClauseOperator clauseOperator = ClauseOperator.And)
    {
        if (string.IsNullOrEmpty(parameterName)) parameterName = columnName;
        var suffix = $"{FormatSelector(columnName)} <= {FormatParameterName(parameterName)}";
        return Where(suffix, value, parameterName, clauseOperator);
    }

    public IQueryBuilder CustomWhere(string clause, object? value = default, string? parameterName = default,
        ClauseOperator clauseOperator = ClauseOperator.And) => Where(clause, value, parameterName, clauseOperator);

    public IQueryBuilder WhereEquals(string columnName, object? value, string? parameterName = default,
        ClauseOperator clauseOperator = ClauseOperator.And)
    {
        if (string.IsNullOrEmpty(parameterName)) parameterName = columnName;
        var suffix = $"{FormatSelector(columnName)} = {FormatParameterName(parameterName)}";
        return Where(suffix, value, parameterName, clauseOperator);
    }

    public IQueryBuilder WhereIsNotNull(string columnName, ClauseOperator clauseOperator = ClauseOperator.And)
    {
        var suffix = $"{FormatSelector(columnName)} IS NOT NULL";
        return Where(suffix, clauseOperator: clauseOperator);
    }

    public IQueryBuilder WhereIsNull(string columnName, ClauseOperator clauseOperator = ClauseOperator.And)
    {
        var suffix = $"{FormatSelector(columnName)} IS NULL";
        return Where(suffix, clauseOperator: clauseOperator);
    }
}
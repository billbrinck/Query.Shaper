using System.Text;
using System.Text.RegularExpressions;
using SQL.Shaper.Common;
using SQL.Shaper.Interfaces;
using SQL.Shaper.Models;

namespace SQL.Shaper.Builder;

public partial class SqlQueryBuilder : IQueryBuilder
{
    private const string QuerySeparator = ";";
    private const string DefaultIdColumnName = "Id";

    private readonly IDictionary<string, object?> _parameters = new Dictionary<string, object?>();

    private readonly StringBuilder _query = new();

    private string[] _insertColumnNames = Array.Empty<string>();
    private bool _insertValuesAttached;

    // When calling Values() we need to maintain a index so that we can add the parameters
    private int _insertValuesIndex;
    private bool _ordered;
    private bool _whereClauseAttached;

    private const char ParameterIdentifierSymbol = '@';

    public IQueryBuilder GroupStart(ClauseOperator clauseOperator = ClauseOperator.And)
    {
        // Before we can use groups, we need to append WHERE Clause
        if (!_whereClauseAttached) Where(string.Empty, clauseOperator: ClauseOperator.Empty);
        Append((clauseOperator == ClauseOperator.Empty ? string.Empty : clauseOperator.ToString()) + "(");
        return this;
    }

    private void AddParameter(string parameterName, object? value) =>
        _parameters.Add(FormatParameterName(parameterName), value);

    private string FormatParameterName(string parameterName) =>
        parameterName.StartsWith(ParameterIdentifierSymbol)
            ? parameterName
            : $"{ParameterIdentifierSymbol}{parameterName}";

    private void Append(string query) => _query.Append($"{query} ");
    private void AppendLine(string query) => _query.AppendLine($"{query} ");
    private void AppendLine() => _query.AppendLine();

    private void EndQuery() => AppendLine(";");

    public IQueryBuilder GroupEnd()
    {
        Append(") ");
        return this;
    }

    public IQueryBuilder Count(string columnName)
    {
        Append($"{Count}({FormatSelector(columnName)}) ");
        return this;
    }

    public IQueryBuilder CountAll()
    {
        Append($"{Count}(*) ");
        return this;
    }

    private string[] FormatSelectors(IEnumerable<string> columnNames) => columnNames.Select(FormatSelector).ToArray();

    private string FormatSelector(string selector)
    {
        return selector.Trim().Equals("*") || selector.Contains('[') || selector.Contains(']')
            ? selector
            : $"[{selector}]";
    }

    public IQueryBuilder Select(params string[] columnNames)
    {
        Append($"{SqlKeywords.Select} {string.Join(", ", FormatSelectors(columnNames))} ");
        return this;
    }

    public IQueryBuilder SelectTop(int numberOfRows, params string[] columnNames)
    {
        AppendLine(
            $"{SqlKeywords.Select} {SqlKeywords.Top} {numberOfRows} {string.Join(",", FormatSelectors(columnNames))} ");
        return this;
    }

    public IQueryBuilder SelectAll() => Select("*");

    public IQueryBuilder SelectTopAll(int numberOfRows) => SelectTop(numberOfRows, "*");


    public IQueryBuilder From(params string[] tableNames)
    {
        AppendLine($"{SqlKeywords.From} {string.Join(",", FormatSelectors(tableNames))} ");
        return this;
    }

    private IQueryBuilder Join(JoinTypes joinType, string joinTable, string joinTableColumnName, string parentTable,
        string parentTableColumnName)
    {
        Append(joinType.ToSqlKeyword());

        Append(SqlKeywords.Join);

        Append(FormatSelector(joinTable));

        Append(SqlKeywords.On);


        Append($"{FormatSelector(joinTable)}.{FormatSelector(joinTableColumnName)}");
        Append(" = ");
        Append($"{FormatSelector(parentTable)}.{FormatSelector(parentTableColumnName)}");
        AppendLine();

        return this;
    }

    public IQueryBuilder InnerJoin(string joinTable, string joinTableColumnName, string parentTable,
        string parentTableColumnName) => Join(JoinTypes.Inner, joinTable, joinTableColumnName, parentTable,
        parentTableColumnName);

    public IQueryBuilder LeftJoin(string joinTable, string joinTableColumnName, string parentTable,
        string parentTableColumnName) =>
        Join(JoinTypes.Left, joinTable, joinTableColumnName, parentTable, parentTableColumnName);


    public IQueryBuilder RightJoin(string joinTable, string joinTableColumnName, string parentTable,
        string parentTableColumnName) => Join(JoinTypes.Right, joinTable, joinTableColumnName, parentTable,
        parentTableColumnName);

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


    public IQueryBuilder OutputInserted(string columnName)
    {
        AppendLine($"{SqlKeywords.Output} {SqlKeywords.Inserted}.{FormatSelector(columnName)}");
        return this;
    }

    public IQueryBuilder Order(string sorting, SortingDirection direction = SortingDirection.Asc,
        string? defaultSorting = default, SortingDirection defaultSortingDirection = SortingDirection.Asc)
    {
        if (string.IsNullOrEmpty(defaultSorting)) return this;

        // Sort by default
        direction = defaultSortingDirection;
        sorting = defaultSorting;

        var ascending = direction == SortingDirection.Asc;
        if (string.IsNullOrEmpty(sorting)) return this;
        OrderBy(sorting, ascending);
        return this;
    }

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

    public IQueryBuilder Paginate(int pageNumber, int pageSize)
    {
        var startParam = "start";
        var pageSizeParam = "pageSize";

        var start = Math.Max(0, pageNumber - 1) * pageSize;
        if (!_ordered)
        {
            AppendLine($"{SqlKeywords.Order} {SqlKeywords.By} 1");
            _ordered = true;
        }

        AppendLine(
            $"{SqlKeywords.Offset} {FormatParameterName(startParam)} {SqlKeywords.Rows} {SqlKeywords.Fetch} {SqlKeywords.Next} {FormatParameterName(pageSizeParam)} {SqlKeywords.Rows} {SqlKeywords.Only}");

        AddParameter(startParam, start);
        AddParameter(pageSizeParam, pageSize);
        return this;
    }

    public IQueryBuilder OrderBy(string columnName, bool ascending)
    {
        var direction = ascending ? SqlKeywords.Ascending : SqlKeywords.Descending;
        if (_ordered)
        {
            AppendLine($", {FormatSelector(columnName)} {direction}");
        }
        else
        {
            AppendLine($"{SqlKeywords.Order} {SqlKeywords.By} {FormatSelector(columnName)} {direction}");
            _ordered = true;
        }

        return this;
    }

    public IQueryBuilder ThenBy(string columnName, bool ascending)
    {
        if (!_ordered)
        {
            OrderBy(columnName, ascending);
        }
        else
        {
            var direction = ascending ? SqlKeywords.Ascending : SqlKeywords.Descending;
            AppendLine($", {FormatSelector(columnName)} {direction}");
        }

        return this;
    }

    public IQueryBuilder AddCountQuery()
    {
        var finalQuery = _query.ToString().Trim();
        if (string.IsNullOrEmpty(finalQuery)) return this;
        if (!finalQuery.EndsWith(QuerySeparator)) AppendLine(QuerySeparator);

        AppendLine(GetCountQuery());

        return this;
    }

    private string GetCountQuery()
    {
        var countQuery = _query.ToString().Trim();
        if (string.IsNullOrEmpty(countQuery)) return string.Empty;

        // Replace the Select CLAUSE
        var regex = new Regex(@$"{SqlKeywords.Select}[\S\s]*? {SqlKeywords.From}", RegexOptions.IgnoreCase);
        countQuery = regex.Replace(countQuery, @$"{SqlKeywords.Select} {SqlKeywords.Count}(*) {SqlKeywords.From}",
            1);

        // Replace everything behind Where
        if (_ordered)
        {
            var oderByIndex = countQuery.IndexOf($"{SqlKeywords.Order} {SqlKeywords.By}",
                StringComparison.OrdinalIgnoreCase);
            if (oderByIndex >= 0) countQuery = countQuery.Substring(0, oderByIndex);
        }

        // Replace everything behind OFFSET
        var offsetIndex = countQuery.IndexOf(SqlKeywords.Offset, StringComparison.OrdinalIgnoreCase);
        if (offsetIndex >= 0) countQuery = countQuery.Substring(0, offsetIndex);


        return countQuery;
    }

    public ParameterizedQuery Build() => new()
    {
        Query = _query.ToString(),
        Parameters = _parameters
    };
}
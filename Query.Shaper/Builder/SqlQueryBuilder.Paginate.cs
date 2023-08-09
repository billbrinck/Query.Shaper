using System.Text.RegularExpressions;
using Query.Shaper.Common;
using Query.Shaper.Interfaces;

namespace Query.Shaper.Builder;

public partial class SqlQueryBuilder
{
    private bool _ordered;

    public IQueryBuilder Order(string sorting, SortingDirection direction = SortingDirection.Asc,
        string? defaultSorting = default)
    {
        var finalSorting = !string.IsNullOrEmpty(sorting) ? sorting : defaultSorting;

        var ascending = direction == SortingDirection.Asc;
        if (string.IsNullOrEmpty(finalSorting)) return this;
        OrderBy(finalSorting, ascending);
        return this;
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
}
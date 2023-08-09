using SQL.Shaper.Common;
using SQL.Shaper.Interfaces;

namespace SQL.Shaper.Builder;

public partial class SqlQueryBuilder
{
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
}
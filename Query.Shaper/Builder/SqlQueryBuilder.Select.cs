using Query.Shaper.Common;
using Query.Shaper.Interfaces;

namespace Query.Shaper.Builder;

public partial class SqlQueryBuilder
{
    public IQueryBuilder Select(params string[] columnNames)
    {
        Append($"{SqlKeywords.Select} {SeparateByCommas(FormatSelectors(columnNames))} ");
        return this;
    }

    public IQueryBuilder SelectTop(int numberOfRows, params string[] columnNames)
    {
        AppendLine(
            $"{SqlKeywords.Select} {SqlKeywords.Top} {numberOfRows} {SeparateByCommas(FormatSelectors(columnNames))} ");
        return this;
    }

    public IQueryBuilder SelectAll() => Select(AllSelector);

    public IQueryBuilder SelectTopAll(int numberOfRows) => SelectTop(numberOfRows, AllSelector);

    public IQueryBuilder From(params string[] tableNames)
    {
        AppendLine($"{SqlKeywords.From} {SeparateByCommas(FormatSelectors(tableNames))} ");
        return this;
    }

    public IQueryBuilder Count(string columnName)
    {
        Append($"{Count}({FormatSelector(columnName)}) ");
        return this;
    }

    public IQueryBuilder CountAll()
    {
        Append($"{Count}({AllSelector}) ");
        return this;
    }
}
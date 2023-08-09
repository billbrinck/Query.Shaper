using System.Text;
using SQL.Shaper.Common;
using SQL.Shaper.Interfaces;
using SQL.Shaper.Models;

namespace SQL.Shaper.Builder;

public partial class SqlQueryBuilder : IQueryBuilder
{
    private const string QuerySeparator = ";";
    private const string DefaultIdColumnName = "Id";
    private const char ParameterIdentifierSymbol = '@';

    private readonly IDictionary<string, object?> _parameters = new Dictionary<string, object?>();
    private readonly StringBuilder _query = new();

    private void AddParameter(string parameterName, object? value) =>
        _parameters.Add(FormatParameterName(parameterName), value);

    private void Append(string query) => _query.Append($"{query} ");
    private void AppendLine(string query) => _query.AppendLine($"{query} ");
    private void AppendLine() => _query.AppendLine();
    private void EndQuery() => AppendLine(";");

    public IQueryBuilder OutputInserted(string columnName)
    {
        AppendLine($"{SqlKeywords.Output} {SqlKeywords.Inserted}.{FormatSelector(columnName)}");
        return this;
    }

    private string[] FormatSelectors(IEnumerable<string> columnNames) => columnNames.Select(FormatSelector).ToArray();

    private string FormatParameterName(string parameterName) =>
        parameterName.StartsWith(ParameterIdentifierSymbol)
            ? parameterName
            : $"{ParameterIdentifierSymbol}{parameterName}";

    private string FormatSelector(string selector) =>
        selector.Trim().Equals("*") || selector.Contains('[') || selector.Contains(']')
            ? selector
            : $"[{selector}]";


    public ParameterizedQuery Build() => new()
    {
        Query = _query.ToString(),
        Parameters = _parameters
    };
}
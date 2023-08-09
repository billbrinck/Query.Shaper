using SQL.Shaper.Common;
using SQL.Shaper.Interfaces;

namespace SQL.Shaper.Builder;

public partial class SqlQueryBuilder
{
    private IQueryBuilder Join(JoinTypes joinType, string joinTable, string joinTableColumnName, string parentTable,
        string parentTableColumnName)
    {
        Append($"{joinType.ToSqlKeyword()} {SqlKeywords.Join} {FormatSelector(joinTable)} {SqlKeywords.On}");

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
}
using SQL.Shaper.Common;
using SQL.Shaper.Interfaces;

namespace SQL.Shaper.Builder;

public partial class SqlQueryBuilder
{
    public IQueryBuilder GroupStart(ClauseOperator clauseOperator = ClauseOperator.And)
    {
        // Before we can use groups, we need to append WHERE Clause
        if (!_whereClauseAttached) Where(string.Empty, clauseOperator: ClauseOperator.Empty);
        Append((clauseOperator == ClauseOperator.Empty ? string.Empty : clauseOperator.ToString()) + "(");
        return this;
    }

    public IQueryBuilder GroupEnd()
    {
        Append(") ");
        return this;
    }
}
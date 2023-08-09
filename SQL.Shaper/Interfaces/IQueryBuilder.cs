using SQL.Shaper.Common;
using SQL.Shaper.Models;

namespace SQL.Shaper.Interfaces;

public interface IQueryBuilder
{
    public ParameterizedQuery Build();

    public IQueryBuilder GroupStart(ClauseOperator clauseOperator = ClauseOperator.And);
    public IQueryBuilder GroupEnd();
    public IQueryBuilder Count(string columnName);
    public IQueryBuilder CountAll();
    public IQueryBuilder Select(params string[] columnNames);
    public IQueryBuilder SelectTop(int numberOfRows, params string[] columnNames);
    public IQueryBuilder SelectAll();
    public IQueryBuilder SelectTopAll(int numberOfRows);
    public IQueryBuilder From(params string[] tableNames);
    public IQueryBuilder Delete(string tableName);
    public IQueryBuilder Delete<TEntity>();

    public IQueryBuilder Insert<TEntity>(TEntity entity, string idColumnName);
    public IQueryBuilder Insert<TEntity>(TEntity entity, string idColumnName, params string[] columnNames);

    public IQueryBuilder Insert<TEntity>(TEntity entity, string tableName, string idColumnName,
        params string[] columnNames);

    public IQueryBuilder Insert<TEntity>(IEnumerable<TEntity> entities);
    public IQueryBuilder Insert<TEntity>(IEnumerable<TEntity> entities, params string[] columnNames);
    public IQueryBuilder Insert<TEntity>(IEnumerable<TEntity> entities, string tableName, params string[] columnNames);

    public IQueryBuilder InnerJoin(string joinTable, string joinTableColumnName, string parentTable,
        string parentTableColumnName);

    public IQueryBuilder LeftJoin(string joinTable, string joinTableColumnName, string parentTable,
        string parentTableColumnName);

    public IQueryBuilder RightJoin(string joinTable, string joinTableColumnName, string parentTable,
        string parentTableColumnName);


    public IQueryBuilder Update<TEntity>(TEntity entity, string idColumnName);

    public IQueryBuilder Update<TEntity>(TEntity entity, string tableName, string idColumnName,
        params string[] columnNames) => Update(entity, tableName, idColumnName, default, columnNames);

    public IQueryBuilder Update<TEntity>(TEntity entity, string tableName, string idColumnName,
        int? index = default, params string[] columnNames);

    public IQueryBuilder Update<TEntity>(IEnumerable<TEntity> entities, string idColumnName,
        params string[] columnNames);

    public IQueryBuilder Update<TEntity>(IEnumerable<TEntity> entities, string tableName, string idColumnName,
        params string[] columnNames);


    public IQueryBuilder OutputInserted(string columnName);

    public IQueryBuilder Order(string sorting, SortingDirection direction = SortingDirection.Asc,
        string? defaultSorting = default);

    public IQueryBuilder WhereIn<TEntity>(string columnName, ClauseOperator clauseOperator = ClauseOperator.And,
        params TEntity[] valueParams);

    public IQueryBuilder WhereNotIn<TEntity>(string columnName, ClauseOperator clauseOperator = ClauseOperator.And,
        params TEntity[] valueParams);

    public IQueryBuilder WhereStringLike(string columnName, string value, string? parameterName = default,
        ClauseOperator clauseOperator = ClauseOperator.And);

    public IQueryBuilder WhereGreaterThan(string columnName, object value, string? parameterName = default,
        ClauseOperator clauseOperator = ClauseOperator.And);

    public IQueryBuilder WhereLessThan(string columnName, object value, string? parameterName = default,
        ClauseOperator clauseOperator = ClauseOperator.And);

    public IQueryBuilder WhereGreaterThanOrEquals(string columnName, object value, string? parameterName = default,
        ClauseOperator clauseOperator = ClauseOperator.And);

    public IQueryBuilder WhereLessThanOrEquals(string columnName, object value, string? parameterName = default,
        ClauseOperator clauseOperator = ClauseOperator.And);

    public IQueryBuilder CustomWhere(string clause, object? value = default, string? parameterName = default,
        ClauseOperator clauseOperator = ClauseOperator.And);

    public IQueryBuilder WhereEquals(string columnName, object? value, string? parameterName = default,
        ClauseOperator clauseOperator = ClauseOperator.And);

    public IQueryBuilder WhereIsNotNull(string columnName, ClauseOperator clauseOperator = ClauseOperator.And);

    public IQueryBuilder WhereIsNull(string columnName, ClauseOperator clauseOperator = ClauseOperator.And);

    public IQueryBuilder Paginate(int pageNumber, int pageSize);

    public IQueryBuilder OrderBy(string columnName, bool ascending);

    public IQueryBuilder ThenBy(string columnName, bool ascending);
    public IQueryBuilder AddCountQuery();
}
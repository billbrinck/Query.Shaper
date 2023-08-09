# Query.Shaper
A .NET library to generate SQL queries with a fluent API

## IQueryBuilder Interface
The IQueryBuilder interface defines a set of methods for constructing parameterized SQL queries with various clauses and operators. This interface provides a flexible and structured way to build queries for database operations such as SELECT, INSERT, UPDATE, and DELETE. Below is an overview of the methods and their functionalities:

## Methods
### Query Building
- ```Build()```: Constructs and returns the final parameterized query.
### Grouping Clauses
- ```GroupStart(clauseOperator)```: Begins a grouped clause with the specified operator (default: AND).
- ```GroupEnd()```: Ends the current grouped clause.
### SELECT Queries
- ```Count(columnName)```: Generates a COUNT aggregation query for the specified column.
- ```CountAll()```: Generates a COUNT aggregation query for all rows.
- ```Select(columnNames)```: Generates a SELECT query for the specified columns.
- ```SelectTop(numberOfRows, columnNames)```: Generates a SELECT query for the top N rows with specified columns.
- ```SelectAll()```: Generates a SELECT query for all columns.
- ```SelectTopAll(numberOfRows)```: Generates a SELECT query for the top N rows with all columns.
- ```From(tableNames)```: Specifies the tables to query from.
### DELETE Queries
- ```Delete(tableName)```: Generates a DELETE query for the specified table.
- ```Delete<TEntity>()```: Generates a DELETE query for a specified entity.
### INSERT Queries
Several methods for generating INSERT queries with different options:
- ```Insert<TEntity>(entity, idColumnName)```: Inserts a single entity with an optional ID column.
- ```Insert<TEntity>(entity, idColumnName, columnNames)```: Inserts a single entity with specified columns.
- ```Insert<TEntity>(entity, tableName, idColumnName, columnNames)```: Inserts a single entity into a specified table.
- ```Insert<TEntity>(entities)```: Inserts multiple entities.
- ```Insert<TEntity>(entities, columnNames)```: Inserts multiple entities with specified columns.
- ```Insert<TEntity>(entities, tableName, columnNames)```: Inserts multiple entities into a specified table.
### JOIN Queries
- ```InnerJoin(joinTable, joinTableColumnName, parentTable, parentTableColumnName)```: Generates an INNER JOIN clause.
- ```LeftJoin(joinTable, joinTableColumnName, parentTable, parentTableColumnName)```: Generates a LEFT JOIN clause.
- ```RightJoin(joinTable, joinTableColumnName, parentTable, parentTableColumnName)```: Generates a RIGHT JOIN clause.
### UPDATE Queries
Several methods for generating UPDATE queries with different options:
- ```Update<TEntity>(entity, idColumnName)```: Updates a single entity based on an ID column.
- ```Update<TEntity>(entity, tableName, idColumnName, columnNames)```: Updates a single entity with specified columns.
- ```Update<TEntity>(entities, idColumnName, columnNames)```: Updates multiple entities based on an ID column.
- ```Update<TEntity>(entities, tableName, idColumnName, columnNames)```: Updates multiple entities with specified columns.
### Filtering and Conditions
Methods for adding various WHERE conditions and filtering:
- ```WhereIn<TEntity>(columnName, clauseOperator, valueParams)```: Adds a WHERE IN clause.
- ```WhereNotIn<TEntity>(columnName, clauseOperator, valueParams)```: Adds a WHERE NOT IN clause.
Methods for various comparison conditions, string matching, null checks, and custom conditions.
### Sorting and Pagination
- ```OrderBy(columnName, ascending)```: Adds an ORDER BY clause.
- ```ThenBy(columnName, ascending)```: Adds a secondary sorting criterion.
- ```Paginate(pageNumber, pageSize)```: Adds pagination to the query.
### Other
- ```OutputInserted(columnName)```: Specifies an OUTPUT INSERTED clause for certain database systems.
- ```AddCountQuery()```: Adds a separate query to count results.
### Summary
The IQueryBuilder interface offers a comprehensive set of methods for building parameterized SQL queries with various clauses and operators. It is a versatile tool for constructing database queries tailored to your specific needs, providing a structured and organized approach to query construction. This interface simplifies the process of creating complex queries while maintaining flexibility and readability.
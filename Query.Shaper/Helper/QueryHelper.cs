using Query.Shaper.Common;

namespace Query.Shaper.Helper
{
    public static class QueryHelper
    {
        private const string QueryDataOnlyFormat = "yyyy-MM-dd";
        private const string QueryDataTimeFormat = "yyyy-MM-dd HH:mm:ss";

        public static string FormatStringLike(string value) => $"%{value}%";

        public static string FormatDateForQuery(DateTime date, string format = QueryDataTimeFormat) =>
            date.ToString(format);

        public static string FormatDateOnlyForQuery(DateTime date) => date.Date.ToString(QueryDataOnlyFormat);
        public static string FormatDateTimeForQuery(DateTime date) => date.ToString(QueryDataTimeFormat);

        public static SortingDirection GetSortingDirection(string direction) =>
            direction.Equals("ASC", StringComparison.OrdinalIgnoreCase) ? SortingDirection.Asc : SortingDirection.Desc;

        public static string FormatDateForQuery(DateOnly date, string format = QueryDataTimeFormat) =>
            date.ToString(format);

        public static string FormatDateOnlyForQuery(DateOnly date) => date.ToString(QueryDataOnlyFormat);
        public static string FormatDateTimeForQuery(DateOnly date) => date.ToString(QueryDataTimeFormat);
    }
}
namespace SQL.Shaper.Common;

public enum JoinTypes
{
    Right,
    Left,
    Inner
}

public static class JoinExtensions
{
    public static string ToSqlKeyword(this JoinTypes joinType)
        => joinType switch
        {
            JoinTypes.Inner => SqlKeywords.Inner,
            JoinTypes.Left => SqlKeywords.Left,
            JoinTypes.Right => SqlKeywords.Right,
            _ => throw new ArgumentOutOfRangeException(nameof(joinType), joinType, null)
        };
}
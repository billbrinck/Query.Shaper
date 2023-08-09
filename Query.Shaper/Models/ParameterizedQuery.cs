
namespace Query.Shaper.Models
{
    public class ParameterizedQuery
    {
        public string? Query { get; set; }
        public IDictionary<string, object?> Parameters { get; set; } = new Dictionary<string, object?>();
    }
}

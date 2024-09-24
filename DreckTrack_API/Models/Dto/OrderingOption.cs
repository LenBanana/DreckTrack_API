using System.Linq.Expressions;

namespace DreckTrack_API.Models.Dto;

public class OrderingOption<T>
{
    public string Key { get; set; } // Unique identifier for the ordering option
    public string DisplayName { get; set; } // Human-readable name
    public Expression<Func<T, object>> OrderExpression { get; set; } // Expression to apply ordering
}
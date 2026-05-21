using System.Reflection;
namespace AuthService
{

    public record PagedResult<T>(int Total, int Offset, int Size, IEnumerable<T> Items)
    {
        public static PagedResult<T> Example
        {
            get
            {
                object? exampleObj = typeof(T).GetProperty("Example", BindingFlags.Public | BindingFlags.Static)?.GetValue(null);
                if (exampleObj is T item)
                {
                    return new PagedResult<T>(50, 0, 20, [item]);
                }

                return new PagedResult<T>(0, 0, 0, Array.Empty<T>());
            }
        }
    };
}

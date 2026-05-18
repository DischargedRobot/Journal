namespace AuthService
{
    public record PagedResult<T>(int Total, int Offset, int Size, IEnumerable<T> Items);
}

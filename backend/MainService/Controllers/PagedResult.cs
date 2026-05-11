using System.Collections.Generic;

namespace MainService
{
    public record PagedResult<T>(int Total, int Offset, int Size, IEnumerable<T> Items);
}

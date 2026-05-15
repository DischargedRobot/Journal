using System.ComponentModel.DataAnnotations;

namespace MainService
{
    public abstract class BaseEntity
    {
        [ConcurrencyCheck]
        public int Version { get; set; } = 0;
    }
}

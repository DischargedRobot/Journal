using System.ComponentModel.DataAnnotations;

namespace AuthService.Model
{
    public abstract class BaseEntity
    {
        [ConcurrencyCheck]
        public int Version { get; set; } = 0;
    }
}

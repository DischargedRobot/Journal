using System.ComponentModel.DataAnnotations;

namespace AuthService.Models
{
    public abstract class BaseEntity
    {
        [ConcurrencyCheck]
        public int Version { get; set; } = 0;
    }
}

using System.ComponentModel.DataAnnotations;

namespace SampleDotNet.Models.Entities
{
    public class RefreshToken
    {
        [Key]
        public string Jti { get; set; } = default!;
        public DateTimeOffset ExpiresIn { get; set; } = default!;
    }
}

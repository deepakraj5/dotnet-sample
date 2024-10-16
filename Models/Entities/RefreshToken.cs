using System.ComponentModel.DataAnnotations;

namespace SampleDotNet.Models.Entities
{
    public class RefreshToken
    {
        [Key]
        public string Jti { get; set; } = default!;
        public DateTime ExpiresIn { get; set; } = default!;
    }
}

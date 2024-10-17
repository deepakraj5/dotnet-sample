using System.ComponentModel.DataAnnotations;

namespace SampleDotNet.Models.Entities
{
    public class UserAccToken
    {
        [Key]
        public string? Token { get; set; }
        public string? UserId { get; set; }
        public bool IsBlackListed { get; set; }
    }
}

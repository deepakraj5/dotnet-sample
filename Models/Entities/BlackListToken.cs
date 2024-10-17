using System.ComponentModel.DataAnnotations;

namespace SampleDotNet.Models.Entities
{
    public class BlackListToken
    {
        [Key]
        public string? token { get; set; }
    }
}

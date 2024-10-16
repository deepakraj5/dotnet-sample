namespace SampleDotNet.Models
{
    public class CreateRefreshTokenDto
    {
        public string Jti { get; set; } = default!;
        public DateTime ExpiresIn { get; set; } = default!;
    }
}

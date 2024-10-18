namespace SampleDotNet.Models
{
    public class CreateRefreshTokenDto
    {
        public string Jti { get; set; } = default!;
        public DateTimeOffset ExpiresIn { get; set; } = default!;
    }
}

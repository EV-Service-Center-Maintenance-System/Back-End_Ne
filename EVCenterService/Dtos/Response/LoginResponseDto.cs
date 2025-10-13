namespace EVCenterService.Dtos.Response
{
    public class LoginResponseDto
    {
        public string Token { get; set; }
        public DateTime Expiration { get; set; }
        public string Role { get; set; }
        public string UserName { get; set; }
    }
}

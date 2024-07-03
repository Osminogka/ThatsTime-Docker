namespace webapi.Models.User
{
    public class LoginResponse
    {
        public bool Success { get; set; } = false;
        public string Message { get; set; } = "";
        public string Token { get; set; } = "";
    }
}

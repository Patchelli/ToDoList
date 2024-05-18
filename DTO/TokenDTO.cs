namespace ToDo.API.DTO
{
    public class TokenDTO
    {
        public string Token { get; set; }
        public DateTime ExpirationToken { get; set; }
        public string RefreshToken { get; set; }
        public DateTime ExpirationRefreshToken { get; set; }
    }
}

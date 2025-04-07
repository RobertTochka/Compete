namespace compete_poco.Dto
{
    public class GetUserRequest
    {
        public long UserId { get; set; }
        public bool IncludeFriends { get;set; }
    }
}

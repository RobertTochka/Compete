namespace compete_platform.Dto.Common
{
    public interface IMatchableUser
    {
        public int Matches { get; set; }
        public List<string> LastResults { get; set; }
    }
}

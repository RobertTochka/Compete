using compete_poco.Dto;

namespace compete_platform.Dto.Common
{
    public interface ISteamUserBasedDto<T> where T : ISteamUserBasedDto<T>
    {
        public string SteamId { get; set; }
        public string Name { get; set; }
        public string AvatarUrl { get; set; }
        public List<T> Friends { get; set; }
    }
}

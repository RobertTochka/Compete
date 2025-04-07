namespace compete_platform.Dto.Admin
{
    public sealed record class BannerFile(string name, MemoryStream stream);
    public class UpdateBannersDto
    {
        public List<string> Banners { get; set; } = null!;
        public List<BannerFile> BannersFiles { get; set;} = null!;
    }
}

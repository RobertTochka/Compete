using compete_platform.Dto.Admin;

namespace compete_platform.Infrastructure.Services
{
    public interface IConfigService
    {
        public Task UpdateContacts(GetContacts contacts);
        public Task UpdateSupportCover(GetSupportCover supportCover);
        public Task UpdateBanners(UpdateBannersDto banners);
        public Task<GetContacts> GetContactConfig();
        public Task<GetSupportCover> GetSupportCover();
        public Task<GetBannersDto> GetBanners();
        public Task CreateConfigsByDefault();
    }
}

using compete_platform.Dto.Admin;

namespace compete_platform.Infrastructure.Services.FileStorage
{
    public interface IFileStorage
    {
        public Task<string> SaveFile(BannerFile file);
        public Task DeleteFile(string name);
    }
}

using compete_platform.Dto.Admin;
using compete_platform.Infrastructure.Services.FileStorage;
using Compete_POCO_Models.Infrastrcuture.Data;
using Compete_POCO_Models.Models;
using Newtonsoft.Json;

namespace compete_platform.Infrastructure.Services
{
    public class ConfigService : IConfigService
    {
        private readonly IFileStorage _fileSrc;
        private readonly CConfigRepository _cfgSrc;
        private static JsonSerializerSettings _jsonSerializerOptions = new()
        { TypeNameHandling = TypeNameHandling.All };

        public ConfigService(CConfigRepository cfgSrc, IFileStorage fileSrc) 
        {
            _fileSrc = fileSrc;
            _cfgSrc = cfgSrc;
        }
        public async Task UpdateBanners(UpdateBannersDto banners)
        {
           var cfg = await _cfgSrc.GetConfigByName(AppDictionary.Banners) 
                ?? throw new ArgumentNullException();
            var config = await GetFormattedConfigByName<GetBannersDto>(AppDictionary.Banners);
            var bannersToDelete = config.Banners.Except(banners.Banners)
                .Select(s => Path.GetFileName(s));
            var tasksToDelete = bannersToDelete.Select(s => _fileSrc.DeleteFile(s));
            await Task.WhenAll(tasksToDelete);
            var tasksToAdd = banners.BannersFiles.Select(s => _fileSrc.SaveFile(s));
            await Task.WhenAll(tasksToAdd);
            var finalFiles = tasksToAdd.Select(s => s.Result).ToList();
            finalFiles.AddRange(banners.Banners);
            await UpdateConfigContent<GetBannersDto>(AppDictionary.Banners, new() { Banners = finalFiles });
        }
        private async Task UpdateConfigContent<T>(string name , T config)
        {
            var cfg = await _cfgSrc.GetConfigByName(name) ??
              throw new ArgumentNullException();
            cfg.Content = JsonConvert.SerializeObject(config, _jsonSerializerOptions);
            await _cfgSrc.SaveChangesAsync();
        }
        public async Task UpdateContacts(GetContacts contacts)
        => await UpdateConfigContent(AppDictionary.Contacts, contacts);

        public async Task UpdateSupportCover(GetSupportCover supportCover)
            => await UpdateConfigContent(AppDictionary.SupportCover, supportCover);
        private async Task<T> GetFormattedConfigByName<T>(string name)
        {
            var cfg = await _cfgSrc.GetConfigByName(name) ?? 
                throw new ArgumentNullException();
            return JsonConvert.DeserializeObject<T>(cfg.Content, _jsonSerializerOptions) ??
                throw new ArgumentNullException();
        }

        public async Task<GetContacts> GetContactConfig()
            => await GetFormattedConfigByName<GetContacts>(AppDictionary.Contacts);

        public async Task<GetSupportCover> GetSupportCover()
            => await GetFormattedConfigByName<GetSupportCover>(AppDictionary.SupportCover);

        public async Task<GetBannersDto> GetBanners() 
            => await GetFormattedConfigByName<GetBannersDto>(AppDictionary.Banners);

        public async Task CreateConfigsByDefault()
        {
            var configsToAdd = new Config[] 
            { 
                new()
                {
                    Name = AppDictionary.Banners, 
                    Content = JsonConvert.SerializeObject(new GetBannersDto()) 
                }, 
                new()
                {
                    Name = AppDictionary.SupportCover, 
                    Content = JsonConvert.SerializeObject(new GetSupportCover())
                },
                new()
                {
                    Name = AppDictionary.Contacts ,
                    Content = JsonConvert.SerializeObject(new GetContacts())
                }
            };
            foreach (var config in configsToAdd)
                await _cfgSrc.AddConfig(config);
            await _cfgSrc.SaveChangesAsync();
        }
    }
}

using AngleSharp.Io;
using compete_platform.Dto.Admin;
using compete_poco.Infrastructure.Data;
using Compete_POCO_Models.Infrastrcuture.Data;

namespace compete_platform.Infrastructure.Services.FileStorage
{
    public class FileStorage : IFileStorage
    {
        private readonly IWebHostEnvironment _env;
        private readonly AppConfig _cfg;

        public FileStorage(IWebHostEnvironment env, AppConfig cfg)
        {
            _env = env;
            _cfg = cfg;
        }
        public async Task DeleteFile(string name)
        {
            var filePath = Path.Combine(_env.ContentRootPath, AppDictionary.UploadedFiles, name);
            await Task.Run(() =>
            {
                File.Delete(filePath);
            });
        }

        public async Task<string> SaveFile(BannerFile file)
        {
            var filePath = Path.Combine(_env.ContentRootPath, AppDictionary.UploadedFiles, file.name);
            using(var fileStream = new FileStream(filePath, FileMode.Create))
            {
                file.stream.Seek(0, SeekOrigin.Begin);
                await file.stream.CopyToAsync(fileStream);
            }
            var fileUrl = $"{_cfg.Host}{AppConfig.RelativeFilePath}/{file.name}";
            return fileUrl;
        }
    }
}

using compete_platform.Dto;
using compete_poco.Infrastructure.Services;
using Compete_POCO_Models.Infrastrcuture.Data;
using Compete_POCO_Models.Models;

namespace compete_platform.Infrastructure.Services.PayRepository
{
    public abstract class CPayRepository : CRepository
    {
        public CPayRepository(ApplicationContext ctx) : base(ctx)
        {
        }
        public abstract Task<Pay> CreatePayAsync(Pay pay);
        public abstract Task<GetPayDto[]> GetUserPaysGroupByDateAsync(long userId,
            int page,
            int pageSize);
    }
}

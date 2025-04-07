using AutoMapper;
using AutoMapper.QueryableExtensions;
using compete_platform.Dto;
using Compete_POCO_Models.Infrastrcuture.Data;
using Compete_POCO_Models.Models;
using Microsoft.EntityFrameworkCore;

namespace compete_platform.Infrastructure.Services.PayRepository
{
    public class PayRepository : CPayRepository
    {
        private readonly IDbContextFactory<ApplicationContext> _factory;
        private readonly IMapper _mapper;

        public PayRepository(ApplicationContext ctx, 
            IDbContextFactory<ApplicationContext> factory, IMapper mapper) : base(ctx)
        {
            _factory = factory;
            _mapper = mapper;
        }

        public async override Task<Pay> CreatePayAsync(Pay pay)
        {
            await _ctx.Pays.AddAsync(pay);
            return pay;
        }

        public async override Task<GetPayDto[]> GetUserPaysGroupByDateAsync(long userId, 
            int page, int pageSize)
        {
           
            using var ctx = await _factory.CreateDbContextAsync();

            var pays = await ctx.Pays
                .Where(p => p.UserId.Equals(userId))
                .OrderByDescending(p => p.CreationTime)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ProjectTo<GetPayDto>(_mapper.ConfigurationProvider)
                .ToArrayAsync();
            return pays;
        }
    }
}

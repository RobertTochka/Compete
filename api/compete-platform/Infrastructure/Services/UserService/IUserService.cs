using compete_platform.Dto;
using compete_platform.Dto.Common;
using compete_poco.Dto;
using compete_poco.Models;

namespace compete_poco.Infrastructure.Services
{
    public interface IUserService
    {
        public Task<GetUserDto> GetUserAsync(GetUserRequest req);
        public Task<List<ISteamUserBasedDto<GetUserDto>>> LinkUsersToSteam(List<ISteamUserBasedDto<GetUserDto>> users, bool includeFriends = false);
        public Task<ISteamUserBasedDto<GetUserDto>> LinkUserToSteam(ISteamUserBasedDto<GetUserDto> user, bool includeFriends = false);
        public Task SetUserAvailability(long userId,bool isOnline);
        public Task<User> CreateUser(UserRole role, string steamId);
        public Task<bool> UserIsExists(long userId);
        public Task TopUpUserBalance(
            decimal amount, long userId, 
            string? payId = null, string? payCorrelation = null,
            string? reason = null, 
            bool shouldUseTransaction = true);
        public Task UpdateUsersRaiting();
        public Task<string> WithdrawalFunds(long userId, decimal amount);
        public Task<UserStatus> GetUserStatusForLobby(long userId);
        public Task<bool> HandleUnproccesedUserAward();
        public Task SetUserBanStatus(bool isBanned, long userId, long sourceId);
    }
}

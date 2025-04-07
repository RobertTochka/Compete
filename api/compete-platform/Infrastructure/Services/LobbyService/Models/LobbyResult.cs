using compete_poco.Models;

namespace compete_platform.Infrastructure.Services;

public class LobbyResult
{
    public LobbyResult(
        long teamWinner, long teamLooser, long[] teamWinnerUserIds, 
        long[] teamLooserUserIds, UserBid[] userWinnerBids, 
        decimal teamWinnerFund, decimal lobbyFund, Dictionary<long, decimal> userWinnerIncomePercent) 
    { 
        TeamWinner = teamWinner;
        TeamLoser = teamLooser;
        TeamWinnerUserIds = teamWinnerUserIds;
        TeamLoserUserIds = teamLooserUserIds;
        UserWinnerBids = userWinnerBids;
        TeamWinnerFund = teamWinnerFund;
        LobbyFund = lobbyFund;
        UserWinnerIncomePercent = userWinnerIncomePercent;
    }
    public long TeamWinner { get; private set; }
    public long TeamLoser { get; private set; }
    public long[] TeamWinnerUserIds { get; private set; } = new long[0];
    public long[] TeamLoserUserIds { get; private set; } = new long[0];
    public UserBid[] UserWinnerBids { get; private set; } = new UserBid[0];
    public decimal TeamWinnerFund {  get; private set; }
    public decimal LobbyFund {  get; private set; }
    public Dictionary<long, decimal> UserWinnerIncomePercent { get; private set; } = new();
}

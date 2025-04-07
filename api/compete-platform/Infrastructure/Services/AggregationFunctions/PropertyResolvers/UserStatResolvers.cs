using CompeteGameServerHandler.Dto;
using System.Linq.Expressions;

namespace CompeteGameServerHandler.Infrastructure.PropertyResolvers
{
    public static class UserStatResolvers
    {
        public static Expression<Func<PlayerLogInformation, int>> CountHeadshots => playerInfo =>
        (int)Math.Round(playerInfo.Hsp * playerInfo.Kills / 100);
        public static double CalculatePlayerRating(double winRate, double kdRatio, decimal income, double headshots)
        {
            
            double winRateWeight = 0.4;
            double kdRatioWeight = 0.3;
            double incomeWeight = 0.2;
            double headshotPercentageWeight = 0.1;

            double normalizedWinRate = winRate / 100;
            double normalizedKdRatio = kdRatio;   
            double normalizedIncome = ((double)income / 100000.0);
            double normalizedHeadshotPercentage = headshots;

            double playerRating = (normalizedWinRate * winRateWeight) +
                                  (normalizedKdRatio * kdRatioWeight) +
                                  (normalizedIncome * incomeWeight) +
                                  (normalizedHeadshotPercentage * headshotPercentageWeight);
            playerRating *= 100;

            return playerRating;
        }

    }
}

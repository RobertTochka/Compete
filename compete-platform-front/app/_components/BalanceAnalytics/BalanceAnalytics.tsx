import { useGetInfographicUserStatsQuery } from "@/app/_fetures/lib/api/publicLobbiesApi";
import { handleError } from "@/app/_utils/functions";
import React from "react";
import { Doughnut } from "react-chartjs-2";
import { Information } from "../Loading/Loading";
import { IUserBasedStats } from "@/app/(pages)/(guard-user)/balance/page";

interface BalanceAnalyticsProps extends IUserBasedStats {}
export const BalanceAnalytics = ({ userId }: BalanceAnalyticsProps) => {
  const { data, isLoading, error } = useGetInfographicUserStatsQuery(userId);
  const dataPercents = data
    ? [
        data.earnedPercent,
        data.outcomeFromBalancePercent,
        data.replenishedSummPercent,
      ]
    : [0, 0, 0];
  const analyticsErrorText = handleError(error);
  const notEnoughData =
    data?.earnedPercent == 0 &&
    data.outcomeFromBalancePercent == 0 &&
    data.replenishedSummPercent == 0
      ? "Недостаточно информации"
      : undefined;
  return (
    <div className="bg-[#29377E]/10 p-[30px] rounded-[10px] shadow-[0_2px_10px_0_rgba(0,0,0,0.10)] w-[355px] xl:w-full xl:flex-middle xl:flex-col">
      <h6 className="text-[20px] font-medium">Аналитика</h6>
      <p className="text-[#BDBDBD] text-[14px] font-medium py-2">
        График показывает соотношение пополненных, выведенных и заработанных
        средств
      </p>
      <div className="bg-[#4F4F4F] h-0.5 relative -ml-[30px] -mr-[30px] mb-8" />
      <div>
        {isLoading || analyticsErrorText || notEnoughData ? (
          <Information
            size={50}
            loading={isLoading}
            errorMessage={analyticsErrorText || notEnoughData}
          ></Information>
        ) : (
          <Doughnut
            data={{
              labels: ["Заработанные", "Вывод с баланса", "Пополнено"],
              datasets: [
                {
                  data: dataPercents,
                  backgroundColor: ["#2563EB", "#E9E9E9", "#f94144"],
                  borderWidth: 0,
                },
              ],
            }}
          />
        )}
      </div>
    </div>
  );
};

import { useGetUserStatsQuery } from "@/app/_fetures/lib/api/publicLobbiesApi";
import { handleError } from "@/app/_utils/functions";
import { Line } from "react-chartjs-2";
import { Information } from "../Loading/Loading";
import { IUserBasedStats } from "@/app/(pages)/(guard-user)/balance/page";

interface IBalancePlotProps extends IUserBasedStats {}
export type Stats = { [key: number]: number };
export const BalancePlot = ({ userId }: IBalancePlotProps) => {
  const {
    data: userStats = {},
    isLoading: userStatsLoading,
    error,
  } = useGetUserStatsQuery(userId);
  const labels = Object.keys(userStats).map(Number);
  const values = Object.values(userStats);
  const userStatsTextError = handleError(error);
  return (
    <div className="bg-[#29377E1A] rounded-[10px] py-9 px-8 xl:w-full w-full">
      <h6 className="text-[20px] font-medium mb-2">Денежный поток</h6>
      <p className="max-w-[425px] text-[14px] font-inter font-medium text-[#BDBDBD]">
        График, отображающий изменение вашего баланса за месяц, но только
        заработанные деньги
      </p>
      <div className="bg-[#4F4F4F] mt-[18px] mb-[14px] -ml-8 -mr-8 h-0.5" />
      {userStatsLoading || error ? (
        <Information
          size={50}
          errorMessage={userStatsTextError}
          loading={userStatsLoading}
        ></Information>
      ) : (
        <>
          <div
            style={{
              height: "200px",
              width: "100%",
              position: "relative",
            }}
          >
            <Line
              style={{
                maxWidth: "100%",
              }}
              options={{
                maintainAspectRatio: false,
                responsive: true,
                color: "#ffffff",
                scales: {
                  x: {
                    grid: {
                      color: "rgba(0, 0, 0, 0)",
                    },
                    border: {
                      display: true,
                    },
                    ticks: {
                      color: "#ffffff",
                      font: {
                        size: 14,
                      },
                    },
                  },
                  y: {
                    grid: {
                      color: "#ffffff",
                      tickColor: "rgba(0,0,0,0)",
                    },
                    border: {
                      display: false,
                      dash: [7, 5],
                    },
                    min: 0,
                    ticks: {
                      stepSize: 5,
                      color: "#ffffff",
                      font: {
                        size: 14,
                      },
                      callback: function (val: string | number, index) {
                        return index % 2 === 0
                          ? `${this.getLabelForValue(Number(val) || 0)}k`
                          : "";
                      },
                    },
                  },
                },
              }}
              data={{
                labels: labels,
                datasets: [
                  {
                    data: values,
                    backgroundColor: "#2563EB",
                    borderColor: "#2563EB",
                    borderWidth: 2,
                    pointBorderWidth: 1,
                    pointBorderColor: "#393A52",
                    pointRadius: 3,
                    pointHoverBackgroundColor: "#2563EB",
                    pointHoverRadius: 5,
                    pointHoverBorderWidth: 2,
                    pointHitRadius: 5,
                    tension: 0.4,
                  },
                ],
              }}
            />
          </div>
          <p className="inline-flex items-center gap-2 font-inter text-[#BDBDBD] font-medium text-[14px] mt-[18px]">
            <span className="w-[9px] h-[9px] bg-[#3780E5] rounded-full" />
            Изменение баланса
          </p>
        </>
      )}
    </div>
  );
};

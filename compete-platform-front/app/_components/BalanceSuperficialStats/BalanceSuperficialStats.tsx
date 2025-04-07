import React from "react";
import InfoBlock from "../Platform/InfoBlock";
import { IUserBasedStats } from "@/app/(pages)/(guard-user)/balance/page";
import { useGetSuperficialStatsQuery } from "@/app/_fetures/lib/api/publicLobbiesApi";
import { formatNumber, handleError } from "@/app/_utils/functions";
import { Information } from "../Loading/Loading";

interface IBalanceSuperficialProps extends IUserBasedStats {
  outcomeMoneyShowing: boolean;
}

export const BalanceSuperficialStats = ({
  userId,
  outcomeMoneyShowing = true,
}: IBalanceSuperficialProps) => {
  const { data, isLoading, error } = useGetSuperficialStatsQuery(userId);
  const superficialErrorText = handleError(error);
  return (
    <>
      {isLoading || superficialErrorText || !data ? (
        <Information
          size={50}
          loading={isLoading}
          errorMessage={superficialErrorText}
        ></Information>
      ) : (
        <>
          <InfoBlock
            label="Баланс"
            value={`₽ ${formatNumber(data.balance)}`}
            line_length={13}
          />
          <InfoBlock
            label="Прибыль за месяц"
            value={`₽ ${formatNumber(data.incomeByMonth)}`}
            line_length={17}
          />
          {outcomeMoneyShowing && (
            <InfoBlock
              label="Выведенные средства"
              value={`₽ ${formatNumber(data.outcomeFromBalance)}`}
              line_length={40}
            />
          )}
        </>
      )}
    </>
  );
};

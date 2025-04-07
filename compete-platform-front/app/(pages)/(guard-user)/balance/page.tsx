"use client";
import {
  ArcElement,
  CategoryScale,
  Chart,
  LineElement,
  LinearScale,
  PointElement,
  Tooltip,
} from "chart.js";
import { BalanceAnalytics } from "@/app/_components/BalanceAnalytics/BalanceAnalytics";
import { BalanceOperations } from "@/app/_components/BalanceOperations/BalanceOperations";
import { BalancePlot } from "@/app/_components/BalancePlot/BalancePlot";
import {
  useDoPayoutMutation,
  useGetUserProfileQuery,
} from "@/app/_fetures/lib/api/publicLobbiesApi";
import { Information } from "@/app/_components/Loading/Loading";
import { handleError } from "@/app/_utils/functions";
import { useEffect, useState } from "react";
import { PayModal } from "@/app/_components/PayModal/PayModal";
import { BalanceSuperficialStats } from "@/app/_components/BalanceSuperficialStats/BalanceSuperficialStats";
import { PayoutModal } from "@/app/_components/PayoutModal/PayoutModal";
import { ResultModal } from "@/app/_components/ResultModal/ResultModal";
import { useSearchParams } from "next/navigation";
import { BalanceWallet } from "@/app/_components/BalanceWallet/BalanceWallet";

Chart.register(
  ArcElement,
  CategoryScale,
  LinearScale,
  PointElement,
  LineElement,
  Tooltip
);

export interface IUserBasedStats {
  userId: number;
}
export default function BalancePage() {
  const {
    data: userInfo,
    isLoading: userInfoLoading,
    error,
  } = useGetUserProfileQuery({ includeFriends: false });

  const userInfoError = handleError(error);
  if ((userInfoLoading && !userInfo) || error) {
    return (
      <Information
        size={90}
        loading={userInfoLoading && !userInfo}
        errorMessage={userInfoError}
      ></Information>
    );
  }

  return (
    <section className="pl-[55px] pt-[14px] flex items-start gap-10 justify-between xxl:pl-5 xxl:gap-x-5">
      <div className="grow">
        <div className="flex items-start gap-10 xxl:gap-5 xl:flex-col">
          <div className="flex flex-col w-[282px] gap-3 xl:w-full xl:gap-3 xl:grid xl:grid-cols-3">
            <BalanceSuperficialStats
              outcomeMoneyShowing
              userId={userInfo?.id ?? 0}
            ></BalanceSuperficialStats>
          </div>
          <BalancePlot userId={userInfo?.id ?? 0}></BalancePlot>
        </div>

        <div className="mt-12 flex items-start justify-between gap-[70px]  xxl:gap-5 xxl:mt-6 xl:flex-col-reverse">
          <BalanceOperations userId={userInfo?.id ?? 0}></BalanceOperations>
          <BalanceAnalytics userId={userInfo?.id ?? 0}></BalanceAnalytics>
        </div>
      </div>
      <BalanceWallet></BalanceWallet>
    </section>
  );
}

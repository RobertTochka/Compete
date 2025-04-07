import { GetUserView } from "@/app/_utils/types";
import clsx from "clsx";
import React, { useState } from "react";
import Avatar from "../Platform/Avatar";
import { LastResult } from "../LastResult/LastResult";
import Icon from "../Icon";
import { useRouter } from "next/navigation";
import { useSetUserBanMutation } from "@/app/_fetures/lib/api/publicLobbiesApi";
import { Information } from "../Loading/Loading";
import { formatNumber } from "@/app/_utils/functions";
import toast from "react-hot-toast";

interface GetTableUserViewProps extends GetUserView {
  shouldBeSelected: boolean;
}

const getRevenue = (revenue: number) => {
  const formatedRevenue = new Intl.NumberFormat("ru").format(Math.abs(revenue));
  if (revenue < 0) {
    return `- ${formatedRevenue}`;
  } else {
    return `+ ${formatedRevenue}`;
  }
};

export const GetTableUserView = ({
  name,
  isOnline,
  id,
  profit,
  winrate,
  avatarUrl,
  lastResults,
  ratePlace,
  balance,
  matches,
  currentLobby,
  isBanned,
  shouldBeSelected,
}: GetTableUserViewProps) => {
  const router = useRouter();
  const [setUserBan, { isLoading: settingBanStatus }] = useSetUserBanMutation();
  const onBan = (userId: number, isBanned: boolean) => {
    setUserBan({ userId, isBanned })
      .unwrap()
      .then(() => setIsBanned(isBanned))
      .catch((e) => toast.error("Ошибка"));
  };
  const [isBannedProp, setIsBanned] = useState<boolean>(isBanned);
  const onPlay = () => {
    router.push(`/lobby/${currentLobby}`);
  };
  return (
    <tr key={id} className={clsx(shouldBeSelected && "bg-black bg-opacity-15")}>
      <td className="text-xs text-gray font-bold whitespace-nowrap">{id}</td>
      <td className="flex items-center px-6 py-4 whitespace-nowrap gap-3 lg:text-[14px]">
        <Avatar
          width={36}
          height={36}
          status_online={isOnline}
          image_url={avatarUrl}
        />
        <span className="font-medium">{name}</span>
      </td>
      <td className="px-6 py-4 whitespace-nowrap lg:text-[14px]">
        {ratePlace}
      </td>
      <td className="px-6 py-4 whitespace-nowrap lg:text-[14px]">
        {formatNumber(winrate, false) || "-"}%
      </td>
      <td className="px-6 py-4 whitespace-nowrap lg:text-[14px]">{matches}</td>
      <td className="px-6 py-4 whitespace-nowrap lg:text-[14px]">
        <div className="flex-middle gap-0.5">
          {lastResults?.map((result, index) => (
            <LastResult result={result} key={index}></LastResult>
          ))}
        </div>
        {!lastResults || lastResults.length === 0 ? "-" : ""}
      </td>
      <td
        className={`lg:text-[14px] px-6 py-4 whitespace-nowrap font-medium ${
          profit < 0 ? "text-negative" : "text-positive"
        }`}
      >
        {getRevenue(profit)} ₽
      </td>
      <td className="px-6 py-4 whitespace-nowrap font-medium lg:text-[14px]">
        {balance} ₽
      </td>
      <td className="px-6 py-4 whitespace-nowrap text-right lg:text-[14px]">
        <div className="flex items-center justify-end gap-2">
          <button
            onClick={onPlay}
            className={clsx(
              "w-6 h-6 flex-middle rounded-full bg-white shrink-0",
              currentLobby !== null && currentLobby != 0
                ? "hover:opacity-80 transition-opacity"
                : "opacity-30 cursor-default"
            )}
          >
            <span className="scale-75">
              <Icon icon="play" defaultColor="#000000" />
            </span>
          </button>
          <a href={`/profile/${id}`} target="_blank" className="shrink-0">
            <Icon icon="eye" defaultColor="#545454" hoverColor="#ffffff" />
          </a>
          {settingBanStatus ? (
            <Information
              loading={settingBanStatus}
              size={15}
              marginTop="0"
            ></Information>
          ) : (
            <button onClick={() => onBan(id, !isBannedProp)}>
              <Icon
                icon={isBannedProp ? "pick" : "delete"}
                defaultColor="#545454"
                hoverColor="#ff0000"
              />
            </button>
          )}
        </div>
      </td>
    </tr>
  );
};

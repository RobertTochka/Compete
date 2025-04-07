import React, { useEffect, useState } from "react";
import Team from "../Platform/Lobby/Team";
import Chat from "../Platform/Lobby/Chat";
import { GetLobbyDto } from "@/app/_utils/types";
import {
  getMyAward,
  getMyTeamChatId,
  getMyTeamId,
  getTeam,
  getUserAwards,
  getUserBids,
} from "@/app/_utils/functions";
import { MatchEndCover } from "../MatchEndCover/MatchEndCover";
import { BalanceSuperficialStats } from "../BalanceSuperficialStats/BalanceSuperficialStats";
import { ColumnKeyPair } from "../Text/Text";
import { useRouter } from "next/navigation";

interface MatchEndPageProps extends GetLobbyDto {
  userId: number;
}

export const MatchEndPage = ({ userId, ...rest }: MatchEndPageProps) => {
  const rightTeam = getTeam(rest);
  const leftTeam = rest.teams[0];
  const userBids = getUserBids(rest);
  const userAwards = getUserAwards(rest);
  const myAward = getMyAward(rest, userId);
  const myTeamId = getMyTeamId(rest, userId);
  const myTeamChatId = getMyTeamChatId(rest, userId);
  const [timer, setTimer] = useState(120);
  const router = useRouter();

  useEffect(() => {
    const interval = setInterval(() => {
      setTimer((pr) => pr - 1);
    }, 1000);

    return () => clearInterval(interval);
  }, []);

  useEffect(() => {
    if (!timer) router.push("/lobbies");
  }, [router, timer]);

  const renderTimerValue = (time: number) => {
    let minutes = Math.floor(time / 60);
    let seconds = String(time - minutes * 60);
    if (seconds.length < 2) {
      seconds = "0" + seconds;
    }
    return `${minutes}:${seconds}`;
  };

  return (
    <section className="px-[15px] pt-[40px] xl:px-8">
      <div className="flex items-start justify-between gap-5">
        <Team
          userAwards={userAwards}
          isWinner={leftTeam.id === rest.teamWinner}
          currentUserId={userId}
          {...leftTeam}
          teamLength={rest.playersAmount}
          userBids={userBids}
        />
        <div className="">
          <ColumnKeyPair
            keyValue="Разыгранный банк"
            value={`₽ ${rest.bids
              .map((b) => b.bid)
              .reduce((a, v) => a + v, 0)}`}
            valuesClassnames="text-[40px]"
          />
          <div className="mt-[99px] flex flex-col gap-[32px]">
            <div className="flex gap-[17px]">
              <div className="bg-[#00000040] max-w-[345px] w-full flex items-center rounded-[10px] font-bold px-[20px] py-[20px]">
                Побеждает команда:{" "}
                {rest.teams.find(({ id }) => rest.teamWinner === id)?.name}
              </div>
              <div className="bg-[#00000040] flex-1 text-[40px] rounded-[10px] flex justify-center items-center font-bold">
                3:1
              </div>
            </div>
            <div className="flex gap-[17px]">
              <div className="bg-[#00000040] flex items-center max-w-[345px] font-bold w-full rounded-[10px] py-[10px] pl-[20px] pr-[5px]">
                При отсутствии жалоб выигрыш зачислится по истечению таймера:
              </div>
              <div className="bg-[#00000040] flex-1 text-[40px] rounded-[10px] flex justify-center items-center font-bold">
                {renderTimerValue(timer)}
              </div>
            </div>
            <div className="bg-[#00000040] rounded-[10px] px-[20px] py-[14px] text-[16px] text-gray max-w-[489px] w-full">
              Время таймера может увеличится при подаче жалобы на проведение
              матча или игрока до выяснения обстоятельств администрацией. Обычно
              это не занимает больше 30 минут.
            </div>
          </div>
        </div>
        <Team
          userAwards={userAwards}
          isWinner={rightTeam.id === rest.teamWinner}
          {...rightTeam}
          userBids={userBids}
          currentUserId={userId}
          teamLength={rest.playersAmount}
          isSecondTeam
        />
      </div>
      <div className="w-[650px] mx-auto mt-[38px]">
        <Chat lobbyChatId={rest.chatId} teamChatId={myTeamChatId} showReport />
      </div>
    </section>
  );
};

import {
  useGetLobbyStatusQuery,
  useGetServerPingQuery,
  useGetUsersInLobbyQuery,
} from "@/app/_fetures/lib/api/publicLobbiesApi";
import { lobbyStatusToTitle, mapToTitle } from "@/app/_utils/functions";
import { GetLobbyViewDto, LobbyStatus } from "@/app/_utils/types";
import Image from "next/image";
import { useEffect, useState } from "react";

interface MatchItemProps extends GetLobbyViewDto {}

export default function MatchItem({
  id,
  bankSumm,
  creator,
  currentMap,
  matchFormat,
  playersAmount,
  server,
}: MatchItemProps) {
  const { data: serverPing } = useGetServerPingQuery({ ip: server.path });
  const { data: status } = useGetLobbyStatusQuery({ id });
  const [pingValue, setPingValue] = useState(0);
  const [statusValue, setStatusValue] = useState(LobbyStatus.Configuring);
  const { data: usersInLobby } = useGetUsersInLobbyQuery(id);

  useEffect(() => {
    if (serverPing) {
      setPingValue(serverPing.pingTime);
    }
  }, [serverPing]);

  useEffect(() => {
    if (status) {
      setStatusValue(status.status);
    }
  }, [status]);

  return (
    <div className="bg-[#00000040] rounded-[10px] flex items-center gap-[10px] cursor-pointer min-h-[80px] hover:bg-[#1E202F]">
      {currentMap ? (
        <div className="rounded-[10px] overflow-hidden w-[136px] relative min-h-[80px] h-full">
          <Image
            fill
            src={`/img/maps/${mapToTitle(currentMap)}.png`}
            alt={mapToTitle(currentMap)}
          />
        </div>
      ) : (
        <div className="w-[136px] relative justify-center align-middle flex">
          {usersInLobby?.length} из {playersAmount * 2}
        </div>
      )}

      <div className="grid grid-cols-7 flex-1">
        <div className="flex flex-col items-center gap-[13px] text-white font-medium">
          <span>Организатор</span>
          <span>{creator.name}</span>
        </div>
        <div className="flex flex-col items-center gap-[13px] text-white font-medium">
          <span>Режим</span>
          <span>
            {playersAmount}v{playersAmount}
          </span>
        </div>
        <div className="flex flex-col items-center gap-[13px] text-white font-medium">
          <span>Счет</span>
          <span>1:10</span>
        </div>
        <div className="flex flex-col items-center gap-[13px] text-white font-medium">
          <span>Тип</span>
          <span>bo{matchFormat}</span>
        </div>
        <div className="flex flex-col items-center gap-[13px] text-white font-medium">
          <span>Ping, ms</span>
          <span>{pingValue}</span>
        </div>
        <div className="flex flex-col items-center gap-[13px] text-white font-medium">
          <span>Ставка, ₽</span>
          <span>{bankSumm}</span>
        </div>
        <div className="flex flex-col items-center gap-[13px] text-white font-medium">
          <span>Статус</span>
          <span>{lobbyStatusToTitle(statusValue)}</span>
        </div>
      </div>
    </div>
  );
}

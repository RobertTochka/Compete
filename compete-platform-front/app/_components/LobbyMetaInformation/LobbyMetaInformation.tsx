import React from "react";
import Chat from "../Platform/Lobby/Chat";
import { KeyValuePair } from "../KeyValuePair/KeyValuePair";
import { GetLobbyDto } from "@/app/_utils/types";
import { getMyTeamChatId, getTextMatchFormat } from "@/app/_utils/functions";

interface ILobbyMetaInformationProps extends GetLobbyDto {
  userId: number;
}

export const LobbyMetaInformation = (props: ILobbyMetaInformationProps) => {
  const {
    id,
    matchFormat,
    chatId,
    userId,
    playersAmount,
    server,
    port,
    status,
  } = props;
  const teamChatId = getMyTeamChatId(props, userId);
  const matchFormatView = getTextMatchFormat(matchFormat);
  return (
    <>
      <ul className="my-10 flex gap-[40px] justify-center items-start">
        {[
          {
            label: "ID матча",
            value: `#${id}`,
          },
          {
            label: "Режим",
            value: `${playersAmount}v${playersAmount}`,
          },
          {
            label: "Тип",
            value: `bo${matchFormat === 1 ? 1 : matchFormat === 2 ? 3 : 5}`,
          },
          {
            label: "Пинг",
            value: "28ms",
          },
          {
            label: "Статус",
            value: status.toString(),
          },
        ].map((pair, i) => (
          <KeyValuePair {...pair} key={i}></KeyValuePair>
        ))}
      </ul>
      <div className="mx-auto flex gap-[30px] relative">
        <Chat lobbyChatId={chatId} teamChatId={teamChatId} showReport />
      </div>
    </>
  );
};

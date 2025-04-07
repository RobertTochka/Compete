import React, { useEffect, useState } from "react";

import { VetoTeamView } from "../VetoTeamView/VetoTeamView";
import { ColumnKeyPair } from "../Text/Text";

import {
  getTeam,
  getMyTeamChatId,
  getTeamBalance,
  handleMapsActionInfo,
  titleToMap,
  useSelectMap,
  configureConnectionForPickMapSecondsNotifications,
  disposeConnectionForPickMapSecondsNotifications,
  getTextMatchFormat,
  truncateString,
} from "@/app/_utils/functions";
import { ActionInfo } from "@/app/_utils/types";
import { VetoTeamLgView } from "../VetoTeamLgView/VetoTeamLgView";
import { VetoMapView } from "../VetoMapView/VetoMapView";
import { LobbyMetaInformation } from "../LobbyMetaInformation/LobbyMetaInformation";
import { useServerTimer } from "@/app/_utils/hooks/useServerTimer";

interface IVetoProps extends ActionInfo {
  userId: number;
}

export const Veto = (props: IVetoProps) => {
  const leftTeamBalance = getTeamBalance(props.newLobby, 0);
  const rightTeamBalance = getTeamBalance(props.newLobby, 1);
  const maps = handleMapsActionInfo(props.newLobby);

  const [selectMap, { condition }] = useSelectMap();
  const rightTeam = getTeam(props.newLobby);
  const leftTeam = props.newLobby.teams[0];

  return (
    <section className="px-[20px] xl:px-8 w-full max-w-[1380px] mx-auto">
      <div className="flex justify-between mt-[44px]">
        <div className={`flex items-center gap-[40px]`}>
          <h3 className={`text-[24px] font-medium relative z-0`}>
            <div className={`absolute right-[-20px]  top-[50%] w-[1px] h-[20px] translate-y-[-50%] bg-white`} />
            {truncateString(leftTeam.name, 16)}
          </h3>
          <p className="text-[24px] font-medium">{leftTeamBalance}₽</p>
        </div>
        <div className={`flex items-center gap-[40px] flex-row-reverse`}>
          <h3 className={`text-[24px] font-medium relative z-0`}>
            <div className={`absolute left-[-20px] top-[50%] w-[1px] h-[20px] translate-y-[-50%] bg-white`} />
            {truncateString(rightTeam.name, 16)}
          </h3>
          <p className="text-[24px] font-medium">{rightTeamBalance}₽</p>
        </div>
      </div>
      <div className="mb-9 pt-[40px] pb-2.5 flex justify-between items-center gap-5 lg:flex-col lg:items-center lg:flex-wrap">
        <VetoTeamView {...leftTeam}></VetoTeamView>
        <div className="flex flex-col gap-3 items-center lg:-order-1">
          <p className="font-medium text-[24px]">
            {props.isPickNow ? "Идет выбор карт" : "Идет бан карт"}
          </p>
        </div>
        <VetoTeamView {...rightTeam} />
        <div className="hidden lg:flex justify-around w-full">
          <VetoTeamLgView {...leftTeam}></VetoTeamLgView>
          <VetoTeamLgView {...rightTeam}></VetoTeamLgView>
        </div>
      </div>
      <div className="flex gap-10 justify-center xl:gap-3">
        {maps.map((map) => (
          <VetoMapView
            key={map.title}
            {...map}
            onClick={() =>
              selectMap({
                map: titleToMap(map.title),
                lobbyId: props.newLobby.id,
              })
            }
          ></VetoMapView>
        ))}
      </div>
      <LobbyMetaInformation
        {...props.newLobby}
        userId={props.userId}
      />
    </section>
  );
};

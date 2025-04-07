import React from "react";
import { VetoTeamView } from "../VetoTeamView/VetoTeamView";
import { GetLobbyDto } from "@/app/_utils/types";
import { ColumnKeyPair } from "../Text/Text";
import { VetoTeamLgView } from "../VetoTeamLgView/VetoTeamLgView";
import { PrepareMapView } from "../PrepareMapView/PrepareMapView";
import { LobbyMetaInformation } from "../LobbyMetaInformation/LobbyMetaInformation";
import { ServerAccessButtons } from "../ServerAccessButtons/ServerAccessButtons";
import { aggregateMapsWithScore, getMyTeamId, getTeamBalance, getTeamScore, truncateString } from "@/app/_utils/functions";
import { useServerTimer } from "@/app/_utils/hooks/useServerTimer";
import Icon from "../Icon";

interface MatchPrepareProps extends GetLobbyDto {
  userId: number;
}

export const MatchPrepare = (props: MatchPrepareProps) => {
  const { teams, bids, port } = props;
  const leftTeam = teams[0];
  const rightTeam = teams[1];
  const leftTeamBalance = getTeamBalance(props, 0);
  const rightTeamBalance = getTeamBalance(props, 1);
  const teamId = getMyTeamId(props, props.userId);
  const pickMaps = aggregateMapsWithScore(props, teamId);
  const seconds = useServerTimer();
  const teamScore = getTeamScore(props, teamId)
  return (
    <section className="px-[15px] pr-20 xl:px-8 w-full max-w-[1380px] mx-auto">
      <div className="flex justify-between mt-[44px]">
        <div className={`flex items-center gap-[40px]`}>
          <h3 className={`text-[24px] font-medium relative z-0`}>
            <div className={`absolute right-[-20px]  top-[50%] w-[1px] h-[20px] translate-y-[-50%] bg-white`} />
            {truncateString(leftTeam.name, 16)}
          </h3>
          <p className="text-[24px] font-medium">{leftTeamBalance}₽</p>
        </div>
        <ServerAccessButtons
          ipAddress={`${props?.server?.path}:${port}`}
        ></ServerAccessButtons>
        <div className={`flex items-center gap-[40px] flex-row-reverse`}>
          <h3 className={`text-[24px] font-medium relative z-0`}>
            <div className={`absolute left-[-20px] top-[50%] w-[1px] h-[20px] translate-y-[-50%] bg-white`} />
            {truncateString(rightTeam.name, 16)}
          </h3>
          <p className="text-[24px] font-medium">{rightTeamBalance}₽</p>
        </div>
      </div>
      <div className="mb-9 mt-[40px] pb-2.5 border-b-[2px] border-gray flex justify-between items-center gap-5">
        <VetoTeamView {...leftTeam}></VetoTeamView>
        <div className="flex flex-col items-center">
          <div className="flex flex-col gap-[7px] items-center">
            <p className="text-[48px] font-bold">{teamScore}</p>
            <p className="font-medium">1-я карта из 3</p>
            <div className="flex gap-[5px] items-center">
              <Icon icon="clock" defaultColor="#fff" />
              <p className="text-[#fff] font-medium">18:25</p>
            </div>
          </div>
        </div>
        <VetoTeamView {...rightTeam}></VetoTeamView>
      </div>
      <div className="flex gap-[60px] justify-center xl:gap-10">
        {pickMaps.map((map) => (
          <PrepareMapView {...map} key={map.title}></PrepareMapView>
        ))}
      </div>

      <LobbyMetaInformation {...props}></LobbyMetaInformation>
    </section>
  );
};

import React from "react";
import { VetoTeamPlayerView } from "../VetoTeamPlayerView/VetoTeamPlayerView";
import { GetTeamDto } from "@/app/_utils/types";
import { truncateString } from "@/app/_utils/functions";

export interface IVetoTeamViewProps extends Omit<GetTeamDto, "lobby"> { }
export const VetoTeamView = ({ users, }: IVetoTeamViewProps) => {
  return (
    <div className="flex items-start justify-between gap-[40px] min-w-[480px]">
      {users?.map((item) => (
        <VetoTeamPlayerView {...item} key={item.id}></VetoTeamPlayerView>
      ))}
    </div>
  );
};

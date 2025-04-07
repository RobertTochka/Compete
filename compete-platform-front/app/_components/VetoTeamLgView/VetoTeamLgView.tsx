import React from "react";
import { VetoTeamPlayerView } from "../VetoTeamPlayerView/VetoTeamPlayerView";
import { IVetoTeamViewProps } from "../VetoTeamView/VetoTeamView";

export const VetoTeamLgView = ({ name, users, id }: IVetoTeamViewProps) => {
  return (
    <div key={id}>
      <h6 className="text-[20px] font-semibold mb-5 text-center">{name}</h6>
      <div className="flex items-start gap-5">
        {users?.map((item) => (
          <VetoTeamPlayerView {...item} key={item.id}></VetoTeamPlayerView>
        ))}
      </div>
    </div>
  );
};

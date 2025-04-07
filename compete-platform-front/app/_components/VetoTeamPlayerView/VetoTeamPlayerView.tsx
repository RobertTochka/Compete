import React from "react";
import Avatar from "../Platform/Avatar";
import { GetUserDto } from "@/app/_utils/types";

interface IVetoTeamPlayerViewProps extends GetUserDto {}

export const VetoTeamPlayerView = ({
  name,
  avatarUrl,
  isOnline,
  id,
}: IVetoTeamPlayerViewProps) => {
  return (
    <div className="flex flex-col items-center gap-2" key={id}>
      <div className="xxl:hidden">
        <Avatar
          width={64}
          height={64}
          status_online={isOnline}
          image_url={avatarUrl}
        />
      </div>
      <div className="hidden xxl:block">
        <Avatar
          width={42}
          height={42}
          status_online={isOnline}
          image_url={avatarUrl}
        />
      </div>
      <span className="font-medium xxl:text-[14px]">{name}</span>
    </div>
  );
};

import { GetUserDto } from "@/app/_utils/types";
import React from "react";
import Avatar from "../Avatar";
import { truncateString } from "@/app/_utils/functions";

interface ITeamPlayerViewProps extends GetUserDto {
  isLeader: boolean;
  bid: number;
  award?: number;
  win?: boolean;
}

export const TeamPlayerView = ({
  id,
  isOnline,
  bid,
  avatarUrl,
  isLeader,
  name,
  win,
}: ITeamPlayerViewProps) => {
  const getBidConfig = (win?: boolean) => {
    if (typeof win !== "boolean") return { color: "white", addText: "" };
    if (win) return { color: "#09FA21", addText: "+" }
    return { color: "#FF3C00", addText: "-" }
  }
  return (
    <div key={id} className="flex items-center gap-[15px]">
      <div className="flex items-center gap-[10px]">
        <Avatar
          width={64}
          height={64}
          image_url={avatarUrl}
          status_online={isOnline}
          creator={isLeader}
        />
        <span className="text-[20px] font-medium w-[138px] break-words">
          {truncateString(name, 21)}
        </span>
      </div>
      <span
        className={`text-[14px] font-medium text-gray-new text-center w-[75px]`}
      >
        id {id}
      </span>
      <span className={`text-[20px] font-medium text-center text-[${getBidConfig(win).color}]`}>
        {getBidConfig(win).addText} ₽&nbsp;{bid}
      </span>
    </div>
  );
};

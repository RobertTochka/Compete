import React from "react";
import Avatar from "../Platform/Avatar";
import { GetReportDto } from "@/app/_utils/types";
import { truncateString } from "@/app/_utils/functions";

interface IReportProps extends GetReportDto {
  onClick: () => void;
  onGoToLobby: () => void;
}

export const Report = ({
  subject,
  content,
  id,
  user,
  onClick,
  onGoToLobby,
}: IReportProps) => {
  const onProfileClick = (e: React.MouseEvent<HTMLDivElement>) => {
    const target = e.target as HTMLElement;

    if (target instanceof HTMLButtonElement) {
    } else {
      onClick();
    }
  };
  return (
    <div
      className="flex gap-x-10 p-5 w-full rounded-[15px] mt-5 align-middle items-center bg-[#181e3a]"
      key={id}
      onClick={onProfileClick}
    >
      <Avatar
        width={60}
        height={60}
        image_url={user.avatarUrl}
        status_online={user.isOnline}
      ></Avatar>
      <div className="flex flex-col gap-y-3 w-full">
        <h5 className="text-white text-[15px] font-bold leading-[130%]">
          {`${truncateString(subject, 30)} от игрока [${truncateString(
            user.name,
            15
          )}]`}
        </h5>
        <p className="text-[12px] text-gray font-medium leading-[130%] my-">
          {truncateString(content, 150)}
        </p>
      </div>
      <button
        onClick={onGoToLobby}
        className="text-[20px] px-5 font-medium rounded-[10px] bg-saturateBlue  hover:bg-secondaryBlue whitespace-nowrapm-5 m-5 py-3"
      >
        Лобби
      </button>
    </div>
  );
};

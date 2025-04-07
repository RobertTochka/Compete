import { GetAppealChatDto } from "@/app/_utils/types";
import Avatar from "../Platform/Avatar";
import { useGetUserProfileQuery } from "@/app/_fetures/lib/api/publicLobbiesApi";
import { useEffect, useMemo } from "react";
import { truncateString } from "@/app/_utils/functions";

interface IReportProps {
  appealChat: GetAppealChatDto;
  onClick: () => void;
}

export const Appeal = ({ appealChat, onClick }: IReportProps) => {
  const userReq = useMemo(
    () => ({ userId: appealChat.userIds[0], includeFriends: false }),
    []
  );
  const { data: user } = useGetUserProfileQuery(userReq);

  const onProfileClick = (e: React.MouseEvent<HTMLDivElement>) => {
    const target = e.target as HTMLElement;

    if (target instanceof HTMLButtonElement) {
    } else {
      onClick();
    }
  };

  return (
    <div
      className="flex gap-x-10 px-5 py-2 w-full rounded-[15px] mt-2 align-middle items-center bg-[#181e3a]"
      onClick={onProfileClick}
    >
      <div className="text-base text-gray font-bold whitespace-nowrap ml-2">
        {user?.id}
      </div>
      <Avatar
        width={60}
        height={60}
        image_url={user?.avatarUrl}
        status_online={user?.isOnline}
      ></Avatar>
      <div className="flex flex-col gap-y-3 w-full">
        <h5 className="text-white text-[20px] font-bold leading-[130%]">
          {user && `${truncateString(user.name, 15)}`}
        </h5>
      </div>
      {!appealChat.isRead && (
        <div
          className={`rounded-full bg-[#ffffff] w-[17px] h-[14px] animate-pulse`}
        />
      )}
    </div>
  );
};

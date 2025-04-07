"use client";

import Avatar from "@/app/_components/Platform/Avatar";
import { GetUserDto } from "@/app/_utils/types";
import { HTMLAttributes } from "react";
import { truncateString } from "@/app/_utils/functions";

type IUserViewProps = Omit<HTMLAttributes<HTMLDivElement>, "id"> &
  GetUserDto & { isSelected?: boolean };

export default function User({
  name,
  id: userId,
  winrate,
  isOnline,
  canInvite,
  avatarUrl,
  isSelected,
  income,
  ...rest
}: IUserViewProps) {
  return (
    <div
      {...rest}
      className={` py-2 pl-3 pr-6 rounded-[10px] flex shadow-user-card justify-between gap-4 hover:bg-[#194066]/50 cursor-pointer
        ${isSelected ? "bg-[#194066]/50" : "bg-transparent-black"}`}
    >
      <div className="flex items-center gap-[18px]">
        <Avatar
          width={64}
          height={64}
          status_online={isOnline}
          canInvite={canInvite}
          image_url={avatarUrl}
        />
        <div className="flex flex-col items-start">
          <span className="text-[20px] font-medium">
            {truncateString(name, 8)}
          </span>
          {status && (
            <span className="text-gray-new text-[14px] font-semibold">
              {status}
            </span>
          )}
        </div>
      </div>
      <div className="flex items-start gap-10 mt-0.5">
        <div className="flex flex-col items-center gap-[11px]">
          <span className="text-gray-new text-[10px] font-semibold">ID игрока</span>
          <span className="text-gray-new font-medium text-[14px]">{userId}</span>
        </div>
        <div className="flex flex-col items-center gap-[11px]">
          <span className="text-gray-new text-[10px] font-semibold">Выиграно</span>
          <span className="text-white font-medium text-[14px]">{income} р</span>
        </div>
      </div>
    </div>
  );
}

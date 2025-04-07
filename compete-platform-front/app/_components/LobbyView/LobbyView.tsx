import { GetLobbyViewDto } from "@/app/_utils/types";
import React from "react";
import Icon from "../Icon";
import clsx from "clsx";

interface ILobbyViewProps extends GetLobbyViewDto {
  onLobbyAdd: () => void;
}

export const LobbyView = ({
  id,
  onLobbyAdd,
  bankSumm,
  playersAmount,
  capacity,
  matchFormat,
  creator,
}: ILobbyViewProps) => {
  return (
    <div
      className={clsx(
        "flex items-center border-[#373b50] text-[14px] font-semibold xl:text-[12px]",
        "border-t"
      )}
      key={id}
    >
      <div className="text-center w-[8%] h-[34px] flex-middle">#{id}</div>
      <div className="border-l h-[34px] flex items-center border-[#373b50] justify-start px-[7px] text-[#9B9B9B] w-[42%]">
        {creator.name}
      </div>
      <div className="border-l h-[34px] flex-middle border-[#373b50] text-center w-[8%] text-positive">
        {id}
      </div>
      <div className="border-l h-[34px] flex-middle border-[#373b50] text-center w-[12%] text-[#9B9B9B]">
        {matchFormat}
      </div>
      <div className="border-l h-[34px] flex-middle border-[#373b50] text-center w-[9%] text-[#9B9B9B]">
        {capacity}/{playersAmount}
      </div>
      <div className="border-l h-[34px] flex-middle border-[#373b50] text-center w-[13%]">
        ₽ {bankSumm}
      </div>
      <div
        className="border-l h-[34px] flex-middle border-[#373b50] text-center w-[8%]"
        onClick={onLobbyAdd}
      >
        <div className="rounded-[3px] bg-saturateBlue h-[18px] w-[60%] flex-middle cursor-pointer hover:bg-secondaryBlue transition-colors">
          <Icon icon="plus" defaultColor="#ffffff" />
        </div>
      </div>
    </div>
  );
};

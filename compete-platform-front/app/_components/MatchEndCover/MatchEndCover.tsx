"use client";
import { formatNumber } from "@/app/_utils/functions";
import { useRouter } from "next/navigation";

import React from "react";

interface IMatchEndCoverProps {
  userAward: number;
  isLost: boolean;
}

export const MatchEndCover = ({ userAward, isLost }: IMatchEndCoverProps) => {
  const router = useRouter();
  const onCreate = () => router.push("/create-lobby");
  return (
    <div className="w-[310px] text-center">
      <p
        className={`${
          !isLost ? "text-positive" : "text-negative"
        } text-[60px] font-medium`}
      >
        {!isLost ? "+" : "-"} {formatNumber(userAward)} ₽
      </p>
      <div className="flex flex-col gap-5">
        <button
          onClick={onCreate}
          className="rounded-[10px] w-full text-[20px] 
        font-medium leading-normal py-1.5 px-6 bg-saturateBlue mt-4 hover:bg-secondaryBlue"
        >
          Создать новое лобби
        </button>

        {/* <button className="rounded-[10px] w-full text-[20px] font-medium leading-normal py-1.5 px-6 bg-[#17C11B]  hover:bg-[#51df69]">
          Оставить фидбек
        </button> */}
      </div>
    </div>
  );
};

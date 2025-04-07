"use client";

import {
  useCreateReportMutation,
  useGetUserProfileQuery,
} from "@/app/_fetures/lib/api/publicLobbiesApi";
import { getUserId } from "@/app/_utils/functions";
import { useId, useState } from "react";

export default function FaqPage() {
  const { data: user } = useGetUserProfileQuery({
    userId: undefined,
    includeFriends: false,
  });
  const [matchId, setMatchId] = useState("");
  const [content, setContent] = useState("");
  const [theme, setTheme] = useState("");
  const [createReport, { isLoading, error }] = useCreateReportMutation();

  const onClick = () => {
    createReport({
      subject: theme,
      content,
      lobbyId: parseInt(matchId),
      userId: parseInt(getUserId()),
      status: "Open",
      response: "-",
      createdAt: new Date().toISOString(),
    });
  };

  return (
    <div className="bg-[#191B21] rounded-[20px] w-full pt-[40px] h-full mt-[30px] flex-1 overflow-auto flex flex-col">
      <div className="gap-[10px] flex-1 overflow-auto flex flex-col">
        <form className="ml-20">
          <div className="mb-6">
            <p>ID матча:</p>
            <input
              type="number"
              className="w-[20%] h-10 mt-4 p-2 text-[16px] placeholder-gray border-[#23262e] border-[2px] rounded-[8px]"
              placeholder="Введите ID Матча..."
              value={matchId}
              onChange={(e) => setMatchId(e.target.value)}
            />
          </div>
          <div className="mb-6">
            <p>Тема жалобы:</p>
            <textarea
              className="bg-transparent w-[40%] h-10 mt-4 p-2 text-[16px] placeholder-gray border-[#23262e] border-[2px] rounded-[8px] overflow-hidden"
              placeholder="Введите тему жалобы..."
              value={theme}
              onChange={(e) => setTheme(e.target.value)}
            />
          </div>
          <div>
            <p>Текст жалобы:</p>
            <textarea
              className="bg-transparent w-[60%] h-72 mt-4 p-2 text-[16px] placeholder-gray border-[#23262e] border-[2px] rounded-[8px] custom-scrollbar"
              placeholder="Введите текст жалобы..."
              value={content}
              onChange={(e) => setContent(e.target.value)}
            />
          </div>
          <button
            type="button"
            className="py-1.5 px-6 bg-[#AD0000]  hover:bg-[#bb5454] w-[300px] h-[50px] border-none rounded-[15px]  mt-8 text-[18px] font-medium"
            onClick={() => onClick()}
          >
            Пожаловаться
          </button>
        </form>
      </div>
    </div>
  );
}

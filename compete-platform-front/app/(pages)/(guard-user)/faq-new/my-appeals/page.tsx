"use client";
import { Information } from "@/app/_components/Loading/Loading";
import { NotAvailable } from "@/app/_components/NotAvailable/NotAvailable";
import {
  reportsAdapter,
  useGetUserReportsQuery,
} from "@/app/_fetures/lib/api/publicLobbiesApi";
import {
  formatDateWithTime,
  reportStatusToTitle,
} from "@/app/_utils/functions";
import { useScrollPagination } from "@/app/_utils/hooks/useScrollPagination";
import { useState } from "react";

const appeals = [
  {
    id: 1,
    status: "В обработке",
    userId: "23784",
    matchId: "23784",
    date: "01.09.24",
    response: "",
    text: "",
  },
  {
    id: 2,
    status: "В обработке",
    userId: "23784",
    matchId: "23784",
    date: "01.09.24",
    response: "",
    text: "",
  },
  {
    id: 3,
    status: "В обработке",
    userId: "23784",
    matchId: "23784",
    date: "01.09.24",
    response: "",
    text: "",
  },
  {
    id: 4,
    status: "В обработке",
    userId: "23784",
    matchId: "23784",
    date: "01.09.24",
    response: "",
    text: "",
  },
  {
    id: 5,
    status: "В обработке",
    userId: "23784",
    matchId: "23784",
    date: "01.09.24",
    response: "",
    text: "",
  },
  {
    id: 6,
    status: "В обработке",
    userId: "23784",
    matchId: "23784",
    date: "01.09.24",
    response: "",
    text: "",
  },
  {
    id: 7,
    status: "В обработке",
    userId: "23784",
    matchId: "23784",
    date: "01.09.24",
    response: "",
    text: "",
  },
  {
    id: 8,
    status: "В обработке",
    userId: "23784",
    matchId: "23784",
    date: "01.09.24",
    response: "",
    text: "",
  },
];

const cols = [
  "Статус жалобы",
  "id пользователя",
  "id матча",
  "Дата и время",
  "Вердикт",
];

export default function FaqPage() {
  const [choosenAppeal, setChoosenAppeal] = useState<number>(null);
  const { page, pageSize, onScroll } = useScrollPagination(true, true);
  const {
    data: reports,
    isLoading: isReportsLoading,
    error: reportsError,
  } = useGetUserReportsQuery(
    {
      page,
      pageSize,
      userId: 1,
    },
    {
      selectFromResult: ({ data, ...res }) => ({
        data: reportsAdapter
          .getSelectors()
          .selectAll(data ?? reportsAdapter.getInitialState()),
        ...res,
      }),
    }
  );

  return (
    <div className="flex-1 mt-[25px] flex flex-col overflow-auto">
      {reports.length === 0 ? (
        <NotAvailable text="У вас нет обращений"></NotAvailable>
      ) : (
        <>
          <div className="grid grid-cols-5 px-[58px]">
            {cols.map((col, index) => (
              <p
                key={col}
                className={`text-[#888891] ${index ? "text-center" : ""}`}
              >
                {col}
              </p>
            ))}
          </div>
          <div className="flex-1 overflow-auto flex flex-col gap-[20px] mt-[11px] custom-scrollbar">
            {reports.map(
              (
                { createdAt, id, lobbyId, response, status, content, userId },
                index
              ) => (
                <div
                  key={id}
                  className={`rounded-[20px] cursor-pointer bg-[#393A52] px-[58px] ${
                    choosenAppeal === index
                      ? "pb-[20px]"
                      : "max-h-[62px] min-h-[62px] overflow-hidden"
                  }`}
                  onClick={() =>
                    setChoosenAppeal(choosenAppeal === index ? null : index)
                  }
                >
                  <div className="grid grid-cols-5 py-[20px] leading-none">
                    <p className="text-[20px]">{reportStatusToTitle(status)}</p>
                    <p className="text-[20px] text-center">{userId}</p>
                    <p className="text-[20px] text-center">{lobbyId}</p>
                    <p className="text-[20px] text-center">
                      {formatDateWithTime(createdAt)}
                    </p>
                    <p className="text-[20px] text-center">{response || "-"}</p>
                  </div>
                  <p className="text-[#888891]">Текст жалобы: {content}</p>
                </div>
              )
            )}
          </div>
        </>
      )}
    </div>
  );
}

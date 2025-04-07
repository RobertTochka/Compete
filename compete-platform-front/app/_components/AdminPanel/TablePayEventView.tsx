"use client";
import { formatDate } from "@/app/_utils/functions";
import { GetPayEvent } from "@/app/_utils/types";
import clsx from "clsx";
import React from "react";
interface TablePayEventViewProps extends GetPayEvent {
  isShouldBeSelected: boolean;
}

export const TablePayEventView = ({
  id,
  amount,
  createdUtc,
  status,
  eventType,
  isShouldBeSelected,
}: TablePayEventViewProps) => {
  return (
    <tr
      key={id}
      className={clsx("text-xs", isShouldBeSelected && "bg-black/10")}
    >
      <td className="px-6 py-5 font-medium">{id}</td>
      <td className="px-6 py-5 font-medium">
        {formatDate(new Date(createdUtc))}
      </td>
      <td className="px-6 py-5 font-medium ">{amount.toLocaleString()} ₽</td>
      <td className="px-6 py-5 font-semibold">{eventType}</td>
      <td className="px-6 py-5 ">
        {status === "Успешно" && (
          <span className="font-semibold bg-positive/30 rounded-[4px] px-2.5 py-0.5">
            Успешно
          </span>
        )}
        {status === "В процессе" && (
          <span className="font-semibold bg-yellow-300/30 rounded-[4px] px-2.5 py-0.5">
            В процессе
          </span>
        )}
        {status === "Ошибка" && (
          <span className="font-semibold bg-negative/30 rounded-[4px] px-2.5 py-0.5">
            Ошибка
          </span>
        )}
      </td>
    </tr>
  );
};

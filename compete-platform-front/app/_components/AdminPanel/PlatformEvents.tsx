"use client";
import React from "react";
import { PlatformEventView } from "./PlatformEventView";
import {
  platformEventsAdapter,
  useGetPlatformEventsQuery,
} from "@/app/_fetures/lib/api/publicLobbiesApi";
import { useHandleError } from "@/app/_utils/hooks/useTemplatedError";
import { Information } from "../Loading/Loading";
import { NotAvailable } from "../NotAvailable/NotAvailable";
import { useScrollPagination } from "@/app/_utils/hooks/useScrollPagination";

export const PlatformEvents = () => {
  const { page, pageSize, onScroll } = useScrollPagination(true, true);
  const {
    data: platformEvents,
    isLoading: platformEventsLoading,
    error: platformEventsError,
  } = useGetPlatformEventsQuery(
    {
      page,
      pageSize,
      order: "desc",
      orderProperty: "id",
    },
    {
      selectFromResult: ({ data, ...res }) => ({
        data: platformEventsAdapter
          .getSelectors()
          .selectAll(data ?? platformEventsAdapter.getInitialState()),
        ...res,
      }),
    }
  );
  const commonErrorText = useHandleError(platformEventsError);
  if ((!platformEvents && platformEventsLoading) || commonErrorText)
    return (
      <Information
        loading={!platformEvents && platformEventsLoading}
        errorMessage={commonErrorText}
      ></Information>
    );
  return (
    <div
      className="p-6 rounded-lg shadow bg-saturateBlue/10 overflow-y-auto max-h-[705px] custom-scrollbar"
      onScroll={platformEventsLoading ? () => {} : onScroll}
    >
      <h2 className="mb-6 text-xl font-semibold">
        {" "}
        Последние действия на платформе
      </h2>
      {!platformEvents || platformEvents.length === 0 ? (
        <NotAvailable text="Нет доступных событий"></NotAvailable>
      ) : (
        platformEvents?.map((s) => (
          <PlatformEventView {...s} key={s.id}></PlatformEventView>
        ))
      )}
    </div>
  );
};

"use client";
import React, { useEffect } from "react";
import { TablePayEventView } from "./TablePayEventView";

import {
  payEventsAdapter,
  useGetPayEventsQuery,
} from "@/app/_fetures/lib/api/publicLobbiesApi";
import { useHandleError } from "@/app/_utils/hooks/useTemplatedError";
import { Information } from "../Loading/Loading";
import { NotAvailable } from "../NotAvailable/NotAvailable";
import { UserTableHeaderColumn } from "../UserTableHeaderColumn/UserTableHeaderColumn";
import { useScrollPagination } from "@/app/_utils/hooks/useScrollPagination";
import { useOrderProps } from "@/app/_utils/hooks/useOrderProps";
const HEADS = ["ID", "Дата", "Сумма", "Событие", "Статус"];
const headerToPropsMappings = {
  ID: "id",
  Дата: "date",
  Статус: "state",
};
const shouldBeWithSort = (h: string) =>
  Object.keys(headerToPropsMappings).includes(h);
export const PayEvents = () => {
  const { order, orderProperty, onHeaderClick } = useOrderProps();
  const { page, pageSize, onScroll, setPageSize, setPage } =
    useScrollPagination(true, true);
  const {
    data: events,
    isLoading: payEventsLoading,
    error: payEventsError,
  } = useGetPayEventsQuery(
    { page, pageSize, order, orderProperty, searchParam: undefined },
    {
      selectFromResult: ({ data, ...res }) => ({
        data: payEventsAdapter
          .getSelectors()
          .selectAll(data ?? payEventsAdapter.getInitialState()),
        ...res,
      }),
    }
  );

  useEffect(() => {
    setPage(1);
  }, [order, orderProperty]);
  const commonErrorText = useHandleError(payEventsError);
  if ((payEventsLoading && !events) || commonErrorText)
    return (
      <Information
        loading={payEventsLoading && !events}
        errorMessage={commonErrorText}
      ></Information>
    );
  return (
    <div className="pt-4 bg-saturateBlue/10 rounded-lg shadow bg-saturatЁeBlue/10">
      <div className="flex px-6 pb-4 border-b">
        <h2 className="text-xl font-bold">Финансовые операции</h2>
      </div>
      <div
        className="p-4 overflow-x-auto overflow-y-auto max-h-[705px] custom-scrollbar"
        onScroll={payEventsLoading ? () => {} : onScroll}
      >
        {!events || events.length === 0 ? (
          <NotAvailable text="Нет доступных финансовых операций"></NotAvailable>
        ) : (
          <table className="w-full table-auto h-full">
            <thead>
              <tr className="text-xs text-left text-white/40">
                {HEADS.map((h, index) => (
                  <UserTableHeaderColumn
                    key={h}
                    onClick={() => {
                      shouldBeWithSort(h)
                        ? onHeaderClick(headerToPropsMappings[h])
                        : null;
                    }}
                    isLeftText={index === 1}
                    isRightText={false}
                    text={h}
                    shouldSort={shouldBeWithSort(h)}
                  ></UserTableHeaderColumn>
                ))}
              </tr>
            </thead>
            <tbody>
              {events?.map((operation, index) => (
                <TablePayEventView
                  key={operation.id}
                  isShouldBeSelected={index % 2 == 0}
                  {...operation}
                ></TablePayEventView>
              ))}
            </tbody>
          </table>
        )}
      </div>
    </div>
  );
};

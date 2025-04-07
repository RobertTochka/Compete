"use client";

import { GetTableUserView } from "@/app/_components/AdminPanel/GetTableUserView";
import {
  useGetUsersForAdminQuery,
  usersForAdminAdapter,
} from "@/app/_fetures/lib/api/publicLobbiesApi";
import { useScrollPagination } from "@/app/_utils/hooks/useScrollPagination";

import { useHandleError } from "@/app/_utils/hooks/useTemplatedError";
import { Information } from "@/app/_components/Loading/Loading";
import { UserTableHeaderColumn } from "@/app/_components/UserTableHeaderColumn/UserTableHeaderColumn";
import { useOrderProps } from "@/app/_utils/hooks/useOrderProps";
import { NotAvailable } from "@/app/_components/NotAvailable/NotAvailable";
import { useDeferredValue, useEffect, useState } from "react";
const headerToPropsDictionary = {
  ID: "id",

  Пользователь: "name",
  "Место в рейтинге": "rate",
  Винрейт: "winrate",
  "Матчей сыграно": "matches",
  "Последние матчи": "lastResults",
  Заработано: "profit",
  Баланс: "balance",
};
const mapHeaderToProperty = (header: string) => {
  return headerToPropsDictionary[header];
};

const HEADS = [
  "ID",
  "Пользователь",
  "Место в рейтинге",
  "Винрейт",
  "Матчей сыграно",
  "Последние матчи",
  "Заработано",
  "Баланс",
  "Действия",
];

export default function AdminPanelUsersPage() {
  const { page, pageSize, onScroll, setPage } = useScrollPagination(true, true);
  const [searchParamValue, setSearchParamValue] = useState<string>(undefined);
  const searchParam = useDeferredValue(searchParamValue);
  const { order, orderProperty, onHeaderClick } = useOrderProps();
  const {
    data: users,
    isLoading: usersLoading,
    error: usersError,
  } = useGetUsersForAdminQuery(
    { page, pageSize, order, orderProperty, searchParam },
    {
      selectFromResult: ({ data, ...res }) => ({
        data: usersForAdminAdapter
          .getSelectors()
          .selectAll(data ?? usersForAdminAdapter.getInitialState()),
        ...res,
      }),
    }
  );

  const commonErrorText = useHandleError(usersError);
  useEffect(() => {
    setPage(1);
  }, [order, orderProperty, searchParam]);

  if (commonErrorText || (usersLoading && users.length == 0))
    return (
      <Information
        loading={usersLoading && users.length == 0}
        errorMessage={commonErrorText}
      ></Information>
    );
  return (
    <section className="p-10 pt-0 xl:p-6 h-full overflow-y-auto custom-scrollbar">
      <input
        type="text"
        name="searchParam"
        value={searchParamValue}
        onChange={(e) => setSearchParamValue(e.currentTarget.value)}
        placeholder="Введите ник или id"
        className="rounded-md py-3 flex items-center gap-4 bg-deepBlue/30 px-4 placeholder:text-white/20"
      />
      <table
        className="min-w-full rounded-[8px] overflow-hidden text-center mt-5"
        onScroll={usersLoading ? () => {} : onScroll}
      >
        <thead className="bg-[#181e3a] text-[10px] text-gray uppercase font-inter lg:text-[9px]">
          <tr>
            {HEADS.map((head, index) => (
              <UserTableHeaderColumn
                key={head}
                onClick={() => {
                  onHeaderClick(mapHeaderToProperty(head));
                }}
                isLeftText={index === 1}
                isRightText={index === HEADS.length - 1}
                text={head}
                shouldSort={head !== "Действия"}
              ></UserTableHeaderColumn>
            ))}
          </tr>
        </thead>
        <tbody className="bg-[#181e3a]">
          {users.map((user, index) => (
            <GetTableUserView
              {...user}
              shouldBeSelected={index % 2 == 0}
              key={user.id}
            ></GetTableUserView>
          ))}
        </tbody>
      </table>
      {users.length === 0 && (
        <NotAvailable text="Список пользователей пуст"></NotAvailable>
      )}
    </section>
  );
}

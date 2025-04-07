"use client";
import { AdminMatchView } from "@/app/_components/AdminMatchView/AdminMatchView";
import {
  matchesAdapter,
  useGetMatchesForAdminQuery,
} from "@/app/_fetures/lib/api/publicLobbiesApi";
import { useScrollPagination } from "@/app/_utils/hooks/useScrollPagination";
import { UserTableHeaderColumn } from "@/app/_components/UserTableHeaderColumn/UserTableHeaderColumn";
import { useOrderProps } from "@/app/_utils/hooks/useOrderProps";
import { useHandleError } from "@/app/_utils/hooks/useTemplatedError";
import { Information } from "@/app/_components/Loading/Loading";
import { NotAvailable } from "@/app/_components/NotAvailable/NotAvailable";
import { useDeferredValue, useEffect, useState } from "react";
import { useSearchParams } from "next/navigation";

const notSortedHEADS = ["Действия", "Демо", "Карты"];
const headerToPropMaps = {
  ID: "id",
  Сервер: "server",
  "Адрес матча": "path",
  Игроки: "players",
  Статус: "status",
};
const mapHeaderToProp = (header: string) => {
  return headerToPropMaps[header];
};
const HEADS = [
  "ID",
  "Сервер",
  "Адрес матча",
  "Карты",
  "Игроки",
  "Статус",
  "Действия",
];

export default function AdminPanelMatchesPage() {
  const { order, orderProperty, onHeaderClick } = useOrderProps();
  const [searchParamValue, setSearchParamValue] = useState<string | undefined>(
    undefined
  );
  const [findBy, setFindBy] = useState("");
  const { page, pageSize, onScroll, setPage } = useScrollPagination(true, true);
  const searchParam = useDeferredValue(searchParamValue);
  const search = useSearchParams();
  const {
    data: matches,
    isLoading: isMatchesLoading,
    error: matchesError,
  } = useGetMatchesForAdminQuery(
    {
      page,
      pageSize,
      order,
      orderProperty,
      searchParam,
      findBy,
    },
    {
      selectFromResult: ({ data, ...res }) => ({
        data: matchesAdapter
          .getSelectors()
          .selectAll(data ?? matchesAdapter.getInitialState()),
        ...res,
      }),
    }
  );
  useEffect(() => {
    setPage(1);
  }, [orderProperty, order, searchParam]);
  useEffect(() => {
    const searchUrl = search.get("search");
    if (searchUrl) setSearchParamValue(searchUrl);
    const findByUrl = search.get("findBy");
    if (findByUrl) setFindBy(findByUrl);
  }, [search]);
  const commonErrorText = useHandleError(matchesError);
  if ((isMatchesLoading && matches.length === 0) || commonErrorText)
    return (
      <Information
        loading={isMatchesLoading && matches.length === 0}
        errorMessage={commonErrorText}
      ></Information>
    );

  return (
    <section
      className="p-10 pt-0 xl:p-6 h-full overflow-y-auto custom-scrollbar"
      onScroll={isMatchesLoading ? () => {} : onScroll}
    >
      <input
        type="text"
        name="searchParam"
        value={searchParamValue}
        onChange={(e) => setSearchParamValue(e.currentTarget.value)}
        placeholder="Введите ник игрока или его id или id матча"
        className="rounded-md py-3 flex items-center gap-4 bg-deepBlue/30 px-4 placeholder:text-white/20"
      />
      <table className="min-w-full rounded-[8px] overflow-hidden text-center mt-5">
        <thead className="bg-[#181e3a] text-[10px] text-gray uppercase font-inter lg:text-[9px]">
          <tr>
            {HEADS.map((head, index) => (
              <UserTableHeaderColumn
                key={head}
                onClick={() => onHeaderClick(mapHeaderToProp(head))}
                isLeftText={index === 1}
                isRightText={index === HEADS.length - 1}
                text={head}
                shouldSort={!notSortedHEADS.includes(head)}
              ></UserTableHeaderColumn>
            ))}
          </tr>
        </thead>
        <tbody className="bg-[#181e3a]">
          {matches.map((match, index) => (
            <AdminMatchView
              {...match}
              key={match.id}
              shouldBeSelected={index % 2 === 0}
            ></AdminMatchView>
          ))}
        </tbody>
      </table>
      {matches.length === 0 && (
        <NotAvailable text="Список матчей пуст"></NotAvailable>
      )}
    </section>
  );
}

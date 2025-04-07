"use client";
import { Appeal } from "@/app/_components/AdminPanel/Appeal";
import AppealModal from "@/app/_components/AppealModal/AppealModal";
import { NotAvailable } from "@/app/_components/NotAvailable/NotAvailable";
import {
  appealChatsAdapter,
  useGetAppealChatsQuery,
} from "@/app/_fetures/lib/api/publicLobbiesApi";
import { useSetAppealChatRead } from "@/app/_utils/functions";
import { useScrollPagination } from "@/app/_utils/hooks/useScrollPagination";
import { GetAppealChatDto } from "@/app/_utils/types";
import { useDeferredValue, useEffect, useState } from "react";

const AppealChats = () => {
  const { page, setPage, onScroll } = useScrollPagination(true, true);
  const [appealForRead, setAppealForRead] = useState<null | GetAppealChatDto>(
    null
  );
  const [searchParamValue, setSearchParamValue] = useState<undefined | string>(
    undefined
  );
  const searchParam = useDeferredValue(searchParamValue);
  const { data: appealChats, isLoading: isAppealLoading } =
    useGetAppealChatsQuery(
      { page, pageSize: 20, searchParam },
      {
        pollingInterval: 2000,
        selectFromResult: ({ data, ...res }) => ({
          data: appealChatsAdapter
            .getSelectors()
            .selectAll(data ?? appealChatsAdapter.getInitialState()),
          ...res,
        }),
      }
    );
  const [setAppealChatRead, { condition }] = useSetAppealChatRead();

  useEffect(() => {
    setPage(1);
  }, []);

  const onClick = (appealChat) => {
    setAppealForRead(appealChat);
    setAppealChatRead(appealChat.id);
    const interval = setInterval(() => {
      if (appealChat !== null) {
        setAppealChatRead(appealChat.id);
      } else {
        clearInterval(interval);
      }
    }, 2000);
  };

  return (
    <section
      className="p-10 pt-0 xl:p-6 h-full overflow-y-auto custom-scrollbar "
      onScroll={isAppealLoading ? () => {} : onScroll}
    >
      <input
        type="text"
        name="searchParam"
        value={searchParamValue}
        onChange={(e) => setSearchParamValue(e.currentTarget.value)}
        placeholder="Введите id или ник"
        className="rounded-md py-3 flex items-center gap-4 bg-deepBlue/30 px-4 placeholder:text-white/20"
      />
      {appealForRead != null && (
        <AppealModal
          onClose={() => setAppealForRead(null)}
          {...appealForRead}
        ></AppealModal>
      )}

      <div className="flex flex-col">
        {appealChats?.length == 0 && (
          <NotAvailable text="Нет открытых чатов"></NotAvailable>
        )}
        {appealChats?.map((appealChat) => (
          <Appeal
            key={appealChat.id}
            appealChat={appealChat}
            onClick={() => onClick(appealChat)}
          />
        ))}
      </div>
    </section>
  );
};

export default AppealChats;

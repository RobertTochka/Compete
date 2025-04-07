"use client";

import { useEffect, useId, useMemo, useState } from "react";
import Icon from "../Icon";
import Messager from "./Messager";
import {
  messagesAdapter,
  selectAll,
  useGetAppealChatMessagesQuery,
} from "@/app/_fetures/lib/api/publicLobbiesApi";
import { useScrollPagination } from "@/app/_utils/hooks/useScrollPagination";
import { useChat } from "@/app/_utils/hooks/useChat";

interface IAppealChat {
  chatId: number;
}

const AppealChat = ({ chatId }: IAppealChat) => {
  const inputImgId = useId();
  const { onMessageSend, condition, message, date, setMessage } = useChat({
    chatId: chatId,
    isAppeal: true,
  });
  const [hasMessagesRefetched, setHasMessagesRefetched] = useState(false);
  const { page, setPage, onScroll } = useScrollPagination<HTMLDivElement>(
    true,
    false
  );

  const onKey = (e: React.KeyboardEvent<HTMLInputElement>) => {
    if (e.code === "Enter") onMessageSend();
  };

  const sendRequest = useMemo(() => {
    if (!chatId) return null;
    return { page, appealChatId: chatId };
  }, [chatId]);

  const {
    data: messages,
    isLoading: messagesLoading,
    error,
    refetch: refetchMessages,
  } = useGetAppealChatMessagesQuery(sendRequest, {
    skip: !sendRequest,
    selectFromResult: ({ data, ...result }) => ({
      data: selectAll(data ?? messagesAdapter.getInitialState()).sort(
        (a, b) =>
          new Date(a.sendTime).getTime() - new Date(b.sendTime).getTime()
      ),
      ...result,
    }),
  });

  useEffect(() => {
    if (chatId && !hasMessagesRefetched) {
      refetchMessages();
      setHasMessagesRefetched(true);
    }
  }, [chatId, hasMessagesRefetched, refetchMessages]);

  return (
    <div className="gap-[10px] flex-1 overflow-auto flex flex-col">
      <Messager
        date={date}
        messages={messages}
        messagesLoading={messagesLoading}
        error={error}
        setPage={setPage}
        onScroll={onScroll}
      />
      <div className="border-t-[1px] border-t-[#808080] py-[22px] flex gap-[20px] max-w-[766px] mx-auto w-full">
        <label htmlFor={inputImgId}>
          <Icon defaultColor="#808080" icon="addImg" hoverColor="#fff" />
          <input className="hidden" type="file" id={inputImgId} />
        </label>
        <input
          type="text"
          className="flex-1"
          placeholder="Message..."
          onKeyDown={onKey}
          onInput={(e) => setMessage(e.currentTarget.value)}
          value={message}
        />
        <button onClick={onMessageSend}>
          <Icon defaultColor="#808080" icon="sentMessage" hoverColor="#fff" />
        </button>
      </div>
    </div>
  );
};

export default AppealChat;

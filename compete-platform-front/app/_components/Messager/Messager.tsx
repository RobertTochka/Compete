import {
  selectAll,
  messagesAdapter,
  useGetChatMessagesQuery,
} from "@/app/_fetures/lib/api/publicLobbiesApi";
import React, { useEffect, useLayoutEffect, useMemo, useRef } from "react";
import { ChatMessage } from "../ChatMessage/ChatMessage";
import { Information } from "../Loading/Loading";
import { useScrollPagination } from "@/app/_utils/hooks/useScrollPagination";
import { handleError } from "@/app/_utils/functions";
import Link from "next/link";
import Image from "next/image";

export interface IMessangerProps {
  chatId: number;
  date: string;
  useBorder?: boolean;
  height?: number;
  isAppeal?: boolean;
}

export const Messager = ({
  chatId,
  date,
  height = 167,
  useBorder = true,
  isAppeal,
}: IMessangerProps) => {
  const container = useRef<HTMLDivElement | null>(null);
  const { page, setPage, onScroll } = useScrollPagination<HTMLDivElement>(
    container.current != null,
    false
  );
  useEffect(() => {
    setPage(1);
  }, [chatId]);
  const sendRequest = useMemo(
    () => ({
      chatId,
      page,
    }),
    [page, chatId]
  );
  const {
    data: messages,
    isLoading: messagesLoading,
    error,
  } = useGetChatMessagesQuery(sendRequest, {
    selectFromResult: ({ data, ...result }) => ({
      data: selectAll(data ?? messagesAdapter.getInitialState())
        .filter((cm) => cm.chatId === chatId)
        .sort(
          (a, b) =>
            new Date(a.sendTime).getTime() - new Date(b.sendTime).getTime()
        ),
      ...result,
    }),
  });
  useLayoutEffect(() => {
    const timerId = setInterval(() => {
      if (container.current) {
        const start = container.current.scrollTop;
        container.current.scrollTop = container.current.scrollHeight;
        if (start < container.current.scrollHeight) clearInterval(timerId);
      }
    }, 300);
  }, [date]);
  const chatError = handleError(error);
  if (chatError)
    return <Information errorMessage={chatError} loading={false}></Information>;

  return (
    <div
      className={`pl-5 pr-6 ${
        useBorder ? "border-y border-gray" : ""
      } py-2.5 custom-scrollbar overflow-y-auto`}
      onScroll={messagesLoading ? () => {} : onScroll}
      ref={container}
      style={{
        scrollBehavior: "smooth",
        overscrollBehavior: "contain",
        maxHeight: `${height}px`,
        minHeight: `${height}px`,
      }}
    >
      {messages.length === 0 && messagesLoading && (
        <div className="flex flex-col items-center justify-center mt-8">
          <Information
            loading={messages.length === 0 && messagesLoading}
            size={70}
          />
        </div>
      )}
      <div className="flex flex-col gap-1.5">
        {isAppeal ? (
          <p className="text-gray font-bold">
            Для жалобы на игрока(ов) отправьте в этот чат: <br />
            - Id нарушителя;
            <br />
            - основание жалобы;
            <br />
            <br />
            Во время расследования общий банк матча будет заморожен до выяснения
            обстоятельств (до 30 мин). <br />
            При положительной обработке жалобы поставленные средства вернутся на
            ваш баланс.
          </p>
        ) : (
          <div className="flex gap-[39px] w-max mx-auto items-center">
            <p className="text-gray font-bold max-w-[280px]">
              Если хотите оставить фидбек, предложить сотрудничество - свяжитесь
              с администрацией в{" "}
              <Link href="/" className="text-saturateBlue">
                Telegram
              </Link>
            </p>
            <Image src="/img/qr.png" width={136} height={136} alt="qr-код" />
          </div>
        )}
        {messages.length !== 0 && messagesLoading && (
          <Information loading={messagesLoading} size={20}></Information>
        )}
        {messages.map((m) => (
          <ChatMessage {...m} key={m.id}></ChatMessage>
        ))}
      </div>
    </div>
  );
};

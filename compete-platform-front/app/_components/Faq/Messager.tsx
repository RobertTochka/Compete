import ChatMessage from "./ChatMessage";
import {
  Dispatch,
  SetStateAction,
  UIEvent,
  useEffect,
  useLayoutEffect,
  useRef,
} from "react";
import { Information } from "../Loading/Loading";
import { GetChatMessageDto } from "@/app/_utils/types";
import { FetchBaseQueryError } from "@reduxjs/toolkit/query";
import { SerializedError } from "@reduxjs/toolkit";
import { handleError, isNeedAvatar } from "@/app/_utils/functions";

interface IMessager {
  date: string;
  messagesLoading: boolean;
  messages: GetChatMessageDto[];
  error: FetchBaseQueryError | SerializedError | undefined;
  setPage: Dispatch<SetStateAction<number>>;
  onScroll: (e: UIEvent<HTMLDivElement>) => void;
}

const Messager = ({
  date,
  messagesLoading,
  messages,
  error,
  setPage,
  onScroll,
}: IMessager) => {
  const container = useRef<HTMLDivElement | null>(null);

  useEffect(() => {
    setPage(1);
  }, []);

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
      className="flex justify-center overflow-auto flex-1 /* custom-scrollbar */"
      onScroll={messagesLoading ? () => {} : onScroll}
      ref={container}
      style={{
        scrollBehavior: "smooth",
        overscrollBehavior: "contain",
      }}
    >
      <div className="pb-[22px] flex flex-col flex-1 max-w-[766px]">
        {messagesLoading ? (
          <div className="flex flex-col items-center justify-center mt-8">
            <Information loading={messagesLoading} size={70} />
          </div>
        ) : messages.length === 0 ? (
          <p className="text-gray font-bold text-[18px] ml-16 mt-6">
            Для жалобы на игрока(ов) отправьте в этот чат: <br />
            <br />
            - Id нарушителя;
            <br />
            <br />- основание жалобы;
          </p>
        ) : (
          messages.map(({ id, content, user }, index) => {
            return (
              <ChatMessage
                key={id}
                user={user}
                content={content}
                isNeedAvatar={isNeedAvatar({ messages, index })}
              />
            );
          })
        )}
      </div>
    </div>
  );
};

export default Messager;

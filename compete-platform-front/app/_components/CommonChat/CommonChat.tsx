import React from "react";
import Icon from "../Icon";
import { Messager } from "../Messager/Messager";
import { useChat } from "@/app/_utils/hooks/useChat";
import { Information } from "../Loading/Loading";
import { useGetCommonChatIdQuery } from "@/app/_fetures/lib/api/publicLobbiesApi";

const CommonChat = () => {
  const { data: chatId } = useGetCommonChatIdQuery();

  const { onMessageSend, setMessage, date, condition, message } = useChat({
    chatId: parseInt(chatId || "0"),
  });
  const onKey = (e: React.KeyboardEvent<HTMLInputElement>) => {
    if (e.code === "Enter") onMessageSend();
  };
  if (!chatId) return <Information loading={!chatId}></Information>;
  return (
    <div className="min-h-[790px] max-h-[790px] w-[275px] bg-GRADIK-1 rounded-[10px] border border-[#373b50] mt-[30px] relative">
      <Messager
        height={760}
        chatId={parseInt(chatId)}
        date={date}
        useBorder={false}
      ></Messager>
      <div className="absolute h-[26px] -left-0.5 -right-0.5 border border-saturateBlue bg-[#24242E] rounded-[10px] -bottom-0.5 flex items-center px-2 justify-between gap-2">
        <div className="flex items-center gap-1.5">
          <input
            onKeyDown={onKey}
            value={message}
            onInput={(e) => setMessage(e.currentTarget.value)}
            className="text-[14px] font-medium placeholder:text-[#626364]"
            type="text"
            placeholder="Message..."
          />
        </div>
        <div className="flex gap-x-4">
          <Information
            loading={condition === "loading"}
            size={30}
          ></Information>
          <button onClick={onMessageSend}>
            <Icon
              icon="sendInChat"
              defaultColor="#545454"
              hoverColor="#ffffff"
            />
          </button>
        </div>
      </div>
    </div>
  );
};
export default CommonChat;

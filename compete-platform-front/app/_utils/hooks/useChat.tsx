import { useState } from "react";
import { useSendAppealMessage, useSendMessage } from "../functions";

interface useChatProps {
  chatId: number;
  isAppeal?: boolean;
}
export const useChat = ({ chatId, isAppeal }: useChatProps) => {
  const [message, setMessage] = useState("");
  const [sendMessage, { condition }] = useSendMessage();
  const [sendAppealMessage, { condition: appealCondition }] =
    useSendAppealMessage();
  const [date, setDate] = useState("");
  const onMessageSend = () => {
    if (isAppeal) {
      return sendAppealMessage({
        message,
        chatId,
      }).then(() => {
        setMessage("");
        setDate(new Date().getMilliseconds().toString());
      });
    }
    return sendMessage({
      message,
      chatId,
    }).then(() => {
      setMessage("");
      setDate(new Date().getMilliseconds().toString());
    });
  };
  if (isAppeal)
    return { onMessageSend, date, message, appealCondition, setMessage };
  return { onMessageSend, date, message, condition, setMessage };
};

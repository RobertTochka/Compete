import { GetChatMessageDto, GetUserDto } from "@/app/_utils/types";
import Avatar from "../Platform/Avatar";

type ChatMessageProps = GetChatMessageDto;

export const ChatMessage = ({
  content,
  user,
  id,
  sendTime,
}: ChatMessageProps) => {
  return (
    <div className="flex items-start gap-2.5" key={id}>
      <Avatar width={24} height={24} image_url={user.avatarUrl} />
      <div>
        <span className="text-gray text-[12px]">{user.name}</span>
        <p className="mt-0.5 text-[12px]">{content}</p>
      </div>
    </div>
  );
};

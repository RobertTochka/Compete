import { GetUserDto } from "@/app/_utils/types";
import Avatar from "../Platform/Avatar";

interface IChatMessageProps {
  user: GetUserDto;
  content: string;
  isNeedAvatar: boolean;
}

const ChatMessage = ({ user, content, isNeedAvatar }: IChatMessageProps) => {
  return (
    <>
      {!user?.isAdmin && (
        <div className="self-end flex flex-col gap-[10px]">
          <div className="self-end">
            {isNeedAvatar && (
              <Avatar
                width={40}
                height={40}
                image_url={user?.avatarUrl ?? ""}
              />
            )}
          </div>
          <div className="p-[20px] flex justify-center bg-saturateBlue rounded-[40px] min-w-[100px] rounded-tr-none">
            {content}
          </div>
        </div>
      )}
      {user?.isAdmin && (
        <div className="self-start flex flex-col gap-[10px]">
          <div className="self-start">
            {isNeedAvatar && (
              <Avatar width={40} height={40} image_url="/img/admin-photo.png" />
            )}
          </div>
          <div className="p-[20px] justify-center bg-[#393A52] rounded-[40px] min-w-[100px] rounded-tl-none">
            {content}
          </div>
        </div>
      )}
    </>
  );
};

export default ChatMessage;

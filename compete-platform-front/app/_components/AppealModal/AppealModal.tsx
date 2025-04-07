import { useGetUserProfileQuery } from "@/app/_fetures/lib/api/publicLobbiesApi";
import AppealChat from "../Faq/AppealChat";
import ModalWindow from "../ModalWindow";

interface IAppealModal {
  onClose: () => void;
  id: number;
  userIds: number[];
}

const AppealModal = ({ onClose, id, userIds }: IAppealModal) => {
  const { data: user } = useGetUserProfileQuery({
    userId: userIds[0],
    includeFriends: false,
  });

  return (
    <ModalWindow onClose={onClose} className="w-[45%]">
      <div className="text-base text-gray font-medium whitespace-nowrap ml-2">
        Чат с пользователем:{" "}
        <span className="font-bold">
          {user?.id} | {user?.name}
        </span>
      </div>
      <div className="bg-[#191B21] rounded-[20px] w-full pt-[30px] px-[20px] h-[70vh] mt-[20px] flex-1 overflow-auto flex flex-col">
        <AppealChat chatId={id} />
      </div>
    </ModalWindow>
  );
};

export default AppealModal;

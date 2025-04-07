import toast from "react-hot-toast";
import Icon from "../Icon";
import { Triangle } from "../Triangle/Triangle";

interface IServerAccessButtonsProps {
  ipAddress: string;
}
const copyToClipboard = async (ipAddress: string) => {
  try {
    // Проверка поддержки Clipboard API
    if (!navigator.clipboard) {
      toast.error("Clipboard API не поддерживается в этом браузере.");
      return;
    }
    await navigator.clipboard.writeText(ipAddress);
    toast.success("Скопировано");
  } catch (error) {
    toast.error("Ошибка при копировании текста в буфер обмена.");
  }
};

export const ServerAccessButtons = ({
  ipAddress,
}: IServerAccessButtonsProps) => {
  return (
    <div className="flex items-center gap-2.5 mt-1.5">
      <Triangle></Triangle>
      <p>
        connect <span className="blur-[7px]">{ipAddress}</span>
      </p>
      <button onClick={() => copyToClipboard(`connect ${ipAddress}`)}>
        <Icon icon="document" defaultColor="#707070" hoverColor="#ffffff" />
      </button>
    </div>
  );
};

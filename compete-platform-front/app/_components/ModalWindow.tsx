import { Portal } from "react-portal";
import clsx from "clsx";

export interface IModalWindow {
  children?: React.ReactNode;
  onClose: () => void;
  className?: string;
}

export default function ModalWindow({
  onClose,
  children,
  className,
}: IModalWindow) {
  const onCloseClick = (e: React.MouseEvent<HTMLDivElement>) => {
    if (e.target === e.currentTarget) onClose();
  };
  return (
    <Portal node={document && document.getElementsByTagName("body")[0]}>
      <div
        onClick={onCloseClick}
        className={clsx(
          "fixed top-0 left-0 right-0 bottom-0 w-screen h-screen bg-black/40 z-[999] p-5 flex-middle"
        )}
      >
        <div
          className={clsx(
            "custom-scrollbar p-5 rounded-[18px] bg-[#1e2741] fixed overflow-auto mx-auto w-[40%] max-h-[90vh]",
            className
          )}
        >
          {children}
        </div>
      </div>
    </Portal>
  );
}

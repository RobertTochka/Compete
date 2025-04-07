import { useStartVeto } from "@/app/_utils/functions";
import { HashLoader } from "react-spinners";

interface IStartMatchButtonProps {
  lobbyId: number;
}

export const StartMatchButton = ({ lobbyId }: IStartMatchButtonProps) => {
  const [startVeto, { condition: vetoCondition }] = useStartVeto();

  return (
    <button
      className="text-[20px] py-[18px] w-[180px] font-medium rounded-[10px] bg-saturateBlue hover:bg-secondaryBlue whitespace-nowrap"
      onClick={() => startVeto(lobbyId)}
      disabled={vetoCondition === "loading"}
    >
      {vetoCondition === "loading" ? <HashLoader color="white" size={25} /> : 'Запустить матч'}
    </button>
  );
};

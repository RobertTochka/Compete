import { LastResult } from "../LastResult/LastResult";

export type MatchResult = "W" | "L";
export interface IStatProps {
  children?: React.ReactNode;
  label: string;
  lastResults?: MatchResult[];
}

const Stat = ({ children, label, lastResults }: IStatProps) => {
  return (
    <div className="flex flex-col items-center justify-center gap-1.5 bg-transparent-black rounded-[20px] h-[120px]">
      {lastResults ? (
        lastResults.length > 0 ? (
          <div className="flex text-[20px] font-medium gap-3">
            {lastResults?.map((result, index) => (
              <LastResult key={index} result={result}></LastResult>
            ))}
          </div>
        ) : (
          "-"
        )
      ) : (
        <span className="text-[20px] font-medium">{children}</span>
      )}
      <span className="text-gray-new text-[14px] font-medium text-center">{label}</span>
    </div>
  );
};
export default Stat;

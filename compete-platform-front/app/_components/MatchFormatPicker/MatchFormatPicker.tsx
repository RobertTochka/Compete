import { Format } from "@/app/_utils/types";

interface IMatchFormatPickerProps {
  matchFormat: Format;
  onUpdate: (newValue: number) => void;
}

export const MatchFormatPicker = ({
  matchFormat,
  onUpdate,
}: IMatchFormatPickerProps) => {
  return (
    <div className="flex items-center justify-between">
      <span className="font-inter">Формат матча:</span>
      <div className="flex items-center gap-4">
        {["bo1", "bo3", "bo5"].map((value, i) => (
          <button
            key={value}
            className={`font-medium ${
              i + 1 === (matchFormat as number)
                ? "text-white cursor-default"
                : "text-gray hover:text-white"
            }`}
            onClick={() => onUpdate(i + 1)}
          >
            {value}
          </button>
        ))}
      </div>
    </div>
  );
};

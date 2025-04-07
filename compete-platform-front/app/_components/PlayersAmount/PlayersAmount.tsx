import { Type } from "@/app/_utils/types";
import React from "react";

interface IPlayersAmountPickerProps {
  playersAmount: Type;
  onUpdate: (newFormat: number) => void;
}

export const PlayersAmountPicker = ({
  playersAmount,
  onUpdate,
}: IPlayersAmountPickerProps) => {
  return (
    <div className="flex items-center justify-between">
      <span className="font-inter">Режим:</span>
      <div className="flex items-center gap-4">
        {[1, 2, 3, 4, 5].map((value) => (
          <button
            key={value}
            className={`font-medium ${
              (playersAmount as number) === value
                ? "text-white cursor-default"
                : "text-gray hover:text-white"
            }`}
            onClick={() => onUpdate(value)}
          >
            {value}×{value}
          </button>
        ))}
      </div>
    </div>
  );
};

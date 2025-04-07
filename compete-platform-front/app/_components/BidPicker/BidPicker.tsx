import { isNumeric } from "@/app/_utils/functions";
import clsx from "clsx";
import { useCallback, useDeferredValue, useEffect, useState } from "react";

interface IBidPickerProps {
  bid: number;
  onUpdate: (value: number) => void;
}

export const BidPicker = ({ bid, onUpdate }: IBidPickerProps) => {
  const [customBid, setCustomBid] = useState(String(bid ?? 0));
  const [isInputVisible, setIsInputVisible] = useState(false);

  const onSumOk = () => {
    if (isNumeric(customBid)) onUpdate(parseInt(customBid));
    setIsInputVisible(pr => !pr)
  }

  return (
    <div className="flex items-center w-full gap-[16px]">
      <span className="font-inter">Ставка:</span>
      {
        !isInputVisible
          ? (
            <>
              <p className="ml-auto">{bid} ₽</p>
            </>
          )
          : <input
            onChange={(e) => setCustomBid(e.target.value)}
            placeholder="Сумма"
            className="border-b-[1px] max-w-[90px] ml-auto border-b-gray-new flex items-center placeholder:text-white/20"
            type="number"
            value={customBid}
            min={1}
          />
      }
      <button className="font-medium text-[#545454] hover:text-white" onClick={onSumOk}>
        {isInputVisible ? "ок" : "изм."}
      </button>
    </div>
  );
};

import React from "react";
import Icon from "../Icon";
import { GetPayDto } from "@/app/_utils/types";
import { formatNumber } from "@/app/_utils/functions";

interface IBalanceOperationProps extends GetPayDto {}

export const BalanceOperation = ({
  description,
  amount,
}: IBalanceOperationProps) => {
  return (
    <>
      <div className="flex items-center pl-6 py-2.5">
        <div className="flex items-center gap-4 basis-[55%]">
          <Icon
            icon={description === "Вывод средств" ? "moneyBack" : "load"}
            defaultColor="#9B9B9B"
          />
          <span className="text-[19px] leading-[110%]">{description}</span>
        </div>
        <p className="font-medium">₽ {formatNumber(amount)}</p>
      </div>
    </>
  );
};

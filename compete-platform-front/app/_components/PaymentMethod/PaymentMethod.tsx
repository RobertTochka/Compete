import React from "react";

interface IPaymentMethosProps {
  icon: string;
  text: string;
  onClick: () => void;
  isSelected: boolean;
}

export const PaymentMethod = ({
  icon,
  text,
  onClick,
  isSelected,
}: IPaymentMethosProps) => {
  return (
    <button
      onClick={onClick}
      key={icon}
      className={`${
        isSelected ? "bg-secondaryBlue" : ""
      } h-[65px] flex items-center gap-[30px] px-6 rounded-[10px] bg-[#1D2141] hover:bg-secondaryBlue xxl:gap-4 xxl:text-left`}
    >
      <img className="w-[50px]" src={`/img/payments/${icon}.svg`} alt={text} />
      <span className="font-medium">{text}</span>
    </button>
  );
};

import React from "react";

interface NotAvailableProps {
  text?: string;
}

export const NotAvailable = ({ text }: NotAvailableProps) => {
  return (
    <div className="text-center d-flex mx-auto text-[16px] font-medium mt-5">
      {text || "Нет контента"}
    </div>
  );
};

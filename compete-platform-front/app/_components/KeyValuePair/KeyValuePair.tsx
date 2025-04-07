import React from "react";
import Icon from "../Icon";

interface IKeyValuePairProps {
  label: string;
  value: string;
}

export const KeyValuePair = ({ label, value }: IKeyValuePairProps) => {
  return (
    <li key={label} className="flex flex-col gap-y-3 items-center">
      <span className="text-white text-[13px] font-medium">{label}</span>
      <span className="font-medium inline-flex items-center gap-1.5">
        {value}
      </span>
    </li>
  );
};

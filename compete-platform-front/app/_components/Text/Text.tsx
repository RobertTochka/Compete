import React from "react";

interface ColumnKeyPairProps {
  keyValue: string;
  value: string;
  keyClassnames?: string;
  valuesClassnames?: string;
}

export const ColumnKeyPair = ({
  keyValue: key,
  value,
  keyClassnames,
  valuesClassnames,
}: ColumnKeyPairProps) => {
  return (
    <div className="flex-middle flex-col text-center">
      <span
        className={`font-medium ${
          valuesClassnames !== undefined ? valuesClassnames : ""
        }`}
      >
        {value}
      </span>
      <span
        className={`text-gray text-[14px] font-medium ${
          keyClassnames !== undefined ? keyClassnames : ""
        }`}
      >
        {key}
      </span>
    </div>
  );
};

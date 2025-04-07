import clsx from "clsx";
import React, { HTMLAttributes } from "react";

interface IDetailsTextProps extends HTMLAttributes<HTMLDivElement> {}

export const DetailsText = ({ className, ...rest }: IDetailsTextProps) => {
  return (
    <div
      {...rest}
      className={`
      text-[14px] text-gray font-medium text-center ${
        className !== undefined ? className : ""
      }`}
    ></div>
  );
};

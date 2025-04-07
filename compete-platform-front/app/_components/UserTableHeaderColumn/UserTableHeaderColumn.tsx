import clsx from "clsx";
import React from "react";
type UserTableHeaderColumnProps = JSX.IntrinsicElements["th"] & {
  text: string;
  isRightText: boolean;
  isLeftText: boolean;
  shouldSort: boolean;
};
export const UserTableHeaderColumn = ({
  text,
  isRightText,
  isLeftText,
  shouldSort,
  ...rest
}: UserTableHeaderColumnProps) => {
  return (
    <th
      {...rest}
      className={clsx("px-6 py-5 tracking-wider lg:px-5 lg:py-3", {
        "text-right": isRightText,
        "text-left": isLeftText,
      })}
    >
      <span
        className={clsx(
          "inline-flex items-center gap-0.5 transition-all",
          text !== "Действия" && "cursor-pointer hover:brightness-[500%] group"
        )}
      >
        {text}
        {shouldSort && (
          <img
            src="/icons/sort.svg"
            className="opacity-20 group-hover:opacity-100 transition-opacity"
          />
        )}
      </span>
    </th>
  );
};

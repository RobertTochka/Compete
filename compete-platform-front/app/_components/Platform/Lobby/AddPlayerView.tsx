import React, { HTMLAttributes } from "react";
import Avatar from "../Avatar";

interface AddPlayerView extends HTMLAttributes<HTMLDivElement> {
  isUnknown?: boolean;
  showLastColumn?: boolean;
}

export const AddPlayerView = ({
  className,
  isUnknown,
  showLastColumn = true,
  ...rest
}: AddPlayerView) => {
  return (
    <div className="flex items-center">
      <div {...rest} className={`flex items-center gap-3 ${"basis-[60%]"}`}>
        <Avatar width={64} height={64} add_friend={isUnknown !== true} />
        <span className="text-[20px] font-medium">No name</span>
      </div>
      <span className={`text-[20px] text-gray text-center ${"basis-[20%]"}`}>
        -
      </span>
      {showLastColumn && (
        <span className={`text-[20px] text-center ${"basis-[20%]"}`}>-</span>
      )}
    </div>
  );
};

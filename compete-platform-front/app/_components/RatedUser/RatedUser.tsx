import { GetUserRateDto } from "@/app/_utils/types";
import React, { HTMLAttributes } from "react";
import Avatar from "../Platform/Avatar";
import { formatNumber } from "@/app/_utils/functions";
import { ratingLabels } from "@/app/_utils/constants/constants";

type IRatedUser = { user: GetUserRateDto } &
  Omit<HTMLAttributes<HTMLTableRowElement>, "id"> & { index: number };

export const RatedUser = ({
  user,
  contentEditable,
  index,
  ...rest
}: IRatedUser) => {
  return (
    <div key={user.id} className={`grid grid-cols-7 items-center ${index ? "border-t border-[#707070]" : ""}`} {...rest}>
      {Object.values(ratingLabels).map((item) => {
        if (item.dataName === "name") {
          return (
            <div className="py-3.5 text-[18px] font-inter font-medium text-left inline-flex items-center gap-2" key={item.colIndex}>
              <Avatar
                status_online={user.isOnline}
                width={36}
                height={36}
                image_url={user.avatarUrl}
              />
              <span className="whitespace-nowrap">{user.name}</span>
            </div>
          )
        }
        if (item.dataName === "ratePlace") return (
          <div className="py-3.5 text-[20px] font-semibold text-center" key={item.colIndex}>
            {user.ratePlace || "-"}
          </div>
        )
        return (
          <div className="py-3.5 text-[20px] font-semibold text-center" key={item.colIndex}>
            {formatNumber(user[item.dataName] ?? 0, false) || "-"}
          </div>
        )
      })}
    </div>
  );
};

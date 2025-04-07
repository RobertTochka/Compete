import React from "react";
import Icon from "../Icon";

interface AdminBannerProps {
  img: string;
  isNotSaved: boolean;
  onClick: () => void;
}

export const AdminBanner = ({ img, isNotSaved, onClick }: AdminBannerProps) => {
  return (
    <div className="relative">
      {isNotSaved && (
        <div className="absolute uppercase bg-red-600 rounded-full text-white px-2 top-2 left-2 py-1 text-[10px] font-semibold">
          Not saved
        </div>
      )}
      <img
        className="block rounded-lg h-[220px] w-full object-cover"
        src={img}
        alt="Рекламный баннер"
      />
      <div className="absolute bottom-3 right-3 bg-[#264379] rounded-full flex-middle gap-2 px-3 py-1.5 shadow-lg">
        <button
          onClick={onClick}
          className="hover:opacity-70   transition-opacity"
        >
          <Icon icon="delete" defaultColor="#ffffff" />
        </button>
      </div>
    </div>
  );
};

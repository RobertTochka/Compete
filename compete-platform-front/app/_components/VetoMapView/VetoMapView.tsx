import React from "react";
import Icon from "../Icon";

interface IVetoMapViewProps {
  title: string;
  state: string;
  preview: string;
  onClick: () => void;
}

export const VetoMapView = ({
  title,
  state,
  preview,
  onClick,
}: IVetoMapViewProps) => {
  return (
    <div
      onClick={onClick}
      key={title}
      className={`flex flex-col gap-[7px] max-w-[160px] w-full items-center ${
        state === "waiting" ? "hover:scale-105" : ""
      }`}
    >
      <div
        className={`overflow-hidden rounded-[10px] h-[200px]  relative xl:h-40 ${
          state === "waiting" && "group cursor-pointer"
        }`}
      >
        <img
          className="object-cover w-full h-full group-hover:scale-110 transition"
          src={preview}
          alt={title}
        />
        {state !== "waiting" && (
          <div
            className={`absolute top-0 left-0 right-0 bottom-0 rounded-[10px] backdrop-blur-[2px] flex-middle ${
              state === "ban" ? "bg-negative/10" : "bg-positive/10"
            }`}
          >
            <Icon
              icon={state}
              defaultColor={state === "pick" ? "#00FF47" : "#F01414"}
            />
          </div>
        )}
      </div>
      <h6 className="font-medium leading-normal">{title}</h6>
    </div>
  );
};

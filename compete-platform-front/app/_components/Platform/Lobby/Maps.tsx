"use client";

import { Map } from "@/app/_utils/types";
import Image from "next/image";
const maps = [
  "Mirage",
  "Inferno",
  "Nuke",
  "Anubis",
  "Overpass",
  "Vertigo",
  "Ancient",
  "Dust2",
  "Office",
  "Italy",
];
const mapToEnum = (map: string) => maps.findIndex((m) => m === map);
interface IMapsProps {
  mapsOnLobby: Map[];
  onMapChange: (map: Map) => void;
}

export default function Maps({ mapsOnLobby, onMapChange }: IMapsProps) {
  return (
    <div className="flex flex-col gap-[11px] overflow-auto max-h-[486px] w-[50%] pr-[10px] custom-scrollbar">
      {maps.map((map) => (
        <div
          onClick={() => onMapChange(mapToEnum(map))}
          key={map}
          className={`cursor-pointer flex ${mapsOnLobby.includes(mapToEnum(map)) ? "bg-[#1E202F]" : "bg-[#00000040]"} rounded-[10px] hover:bg-[#1E202F]`}
        >
          <div className="rounded-[10px] relative w-[100px] overflow-hidden">
            <Image
              src={`/img/maps/${map.toLowerCase()}.png`}
              alt={map}
              fill
              objectFit="cover"
            />
          </div>
          <div className="p-[5px] grid grid-cols-2 flex-1">
            <div className="flex flex-col gap-[10px] items-center">
              <p className="text-gray font-medium">Карта</p>
              <p>{map}</p>
            </div>
            <div className="flex flex-col gap-[10px] items-center">
              <p className="text-gray font-medium">Винрейт</p>
              <p>0%</p>
            </div>
          </div>
        </div>
      ))}
    </div>
  );
}

import { GetServerDto } from "@/app/_utils/types";
import { useEffect, useState } from "react";
import Icon from "../Icon";
import { useGetServerPingQuery } from "@/app/_fetures/lib/api/publicLobbiesApi";

interface ISingleServerProps extends GetServerDto {
  isActive: boolean;
  onClick: () => void;
}
export const SingleServer = ({
  location,
  onClick,
  isActive,
  path,
}: ISingleServerProps) => {
  const { data: serverPing } = useGetServerPingQuery({ ip: path });
  const [pingValue, setPingValue] = useState(0);

  useEffect(() => {
    if (serverPing) {
      setPingValue(serverPing.pingTime);
    }
  }, [serverPing]);

  return (
    <button
      key={location}
      className={`flex items-center gap-1 ${
        isActive ? "text-white cursor-default" : "text-gray hover:text-white"
      }`}
      onClick={onClick}
    >
      <Icon
        icon="ping"
        defaultColor={
          pingValue <= 30 ? "#00FF47" : pingValue >= 90 ? "#BD0000" : "#FFF500"
        }
      />
      {pingValue}
    </button>
  );
};

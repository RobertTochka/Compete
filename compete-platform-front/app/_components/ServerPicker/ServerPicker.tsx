import { useGetServersQuery } from "@/app/_fetures/lib/api/publicLobbiesApi";
import { SingleServer } from "../SingleServer/SingleServer";

interface IServerPickerProps {
  serverId: number;
  onUpdate: (newServerId: number) => void;
}

export const ServerPicker = ({ serverId, onUpdate }: IServerPickerProps) => {
  const { data: availableServers } = useGetServersQuery();
  return (
    <div className="flex items-center gap-2.5">
      <div className="flex items-center gap-4">
        {availableServers?.map((item) => (
          <SingleServer
            key={item.id}
            {...item}
            onClick={() => onUpdate(item.id)}
            isActive={serverId === item.id}
          />
        ))}
      </div>
    </div>
  );
};

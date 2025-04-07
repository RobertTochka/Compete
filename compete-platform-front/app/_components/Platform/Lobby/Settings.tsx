"use client";

import { GetLobbyDto } from "@/app/_utils/types";
import { useUpdateBid } from "@/app/_utils/functions";
import { PlayersAmountPicker } from "../../PlayersAmount/PlayersAmount";
import { MatchFormatPicker } from "../../MatchFormatPicker/MatchFormatPicker";
import { ServerPicker } from "../../ServerPicker/ServerPicker";
import { FreezeTimePicker } from "../../FreezeTimePicker/FreezeTimePicker";
import { FriendlyFireSwitch } from "../../FriendlyFireSwitch/FriendlyFireSwitch";
import { LobbyModeSwitch } from "../../LobbyModeSwitch/LobbyModeSwitch";
import { BidPicker } from "../../BidPicker/BidPicker";
import { useCopyToClipboard } from "@/app/_utils/hooks/useCopyToClipboard";
import Icon from "../../Icon";
import { StartMatchButton } from "../../StartMatchButton/StartMatchButton";

interface ISettingsProps extends GetLobbyDto {
  bid: number;
  onPropSetup: (prop: string) => (value: any) => void;
  onServerConfigSetup: (prop: string) => (value: any) => void;
  onLobbyLeave: () => void;
}

export default function Settings({
  config,
  playersAmount,
  bid,
  onPropSetup,
  serverId,
  onServerConfigSetup,
  public: isPublic,
  matchFormat,
  codeToConnect,
  id,
  onLobbyLeave,
}: ISettingsProps) {
  const [setupBid] = useUpdateBid();
  const [, copy] = useCopyToClipboard();

  return (
    <div className="w-[calc(50% + 10px)] pl-[10px] flex-col gap-[11px] flex ml-auto">
      <div className="flex gap-[11px] w-full">
        <div className="bg-[#00000040] flex flex-1 gap-[11px] p-[20px] pr-[9px] rounded-[10px]">
          <LobbyModeSwitch
            isPublic={isPublic}
            onUpdate={onPropSetup("public")}
          />
          <button className="ml-auto" title="Ссылка для подключения к лобби" onClick={() => copy(`https://compete.wtf/create-lobby/${id}?code=${codeToConnect}`)}>
            <Icon defaultColor="#545454" icon="copyPerson" />
          </button>
        </div>
        <div className="text-[#545454] w-[55px] text-[14px] font-medium bg-[#00000040] rounded-[10px] flex flex-col p-[8px] text-center justify-center">
          <p>id</p>
          <p>{id}</p>
        </div>
      </div>
      <div className="flex gap-[11px] w-full">
        <div className="bg-[#00000040] flex-1 flex gap-[11px] p-[20px] pr-[9px] rounded-[10px]">
          <BidPicker bid={bid} onUpdate={setupBid} />
        </div>
        <div className="text-[#545454] w-[55px] flex items-center font-medium bg-[#00000040] rounded-[10px] p-[8px] text-center">
          <ServerPicker
            serverId={serverId}
            onUpdate={onPropSetup("serverId")}
          />
        </div>
      </div>
      <div className="bg-[#00000040] rounded-[10px] p-[20px] pr-[15px]">
        <PlayersAmountPicker
          playersAmount={playersAmount}
          onUpdate={onPropSetup("playersAmount")}
        />
      </div>
      <div className="bg-[#00000040] rounded-[10px] p-[20px] pr-[15px]">
        <MatchFormatPicker
          matchFormat={matchFormat}
          onUpdate={onPropSetup("matchFormat")}
        />
      </div>
      <div className="bg-[#00000040] rounded-[10px] p-[20px] pr-[15px]">
        <FreezeTimePicker
          freezeTime={config.freezeTime}
          onUpdate={onServerConfigSetup("freezeTime")}
        />
      </div>
      <div className="bg-[#00000040] rounded-[10px] p-[20px] pr-[15px]">
        <FriendlyFireSwitch
          friendlyFire={config.friendlyFire}
          onUpdate={onServerConfigSetup("friendlyFire")}
        />
      </div>
      <div className="flex gap-[11px]">
        <StartMatchButton
          lobbyId={id}
        />
        <button
          className="text-[20px] flex-1 flex items-center bg-[#00000040] rounded-[10px] justify-center"
          onClick={onLobbyLeave}
        >
          Выйти
        </button>
      </div>
    </div>
  );
}

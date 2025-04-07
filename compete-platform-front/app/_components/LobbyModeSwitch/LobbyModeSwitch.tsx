import ReactSwitch from "react-switch";

interface ILobbyModeSwitchProps {
  isPublic: boolean;
  onUpdate: (value: boolean) => void;
}

export const LobbyModeSwitch = ({
  isPublic,
  onUpdate,
}: ILobbyModeSwitchProps) => {
  return (
    <div className="flex items-center gap-[11px]">
      <span className="font-inter">Закрытое лобби:</span>
      <ReactSwitch
        checked={!isPublic}
        onChange={(value) => onUpdate(!value)}
        onColor="#3297F5"
        offColor="#404040"
        handleDiameter={16}
        checkedIcon={false}
        uncheckedIcon={false}
        activeBoxShadow=""
        height={16}
        width={30}
      />
    </div>
  );
};

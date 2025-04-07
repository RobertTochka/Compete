import ReactSwitch from "react-switch";
interface IFriendlyFireSwitchProps {
  friendlyFire: boolean;
  onUpdate: (newValue: boolean) => void;
}

export const FriendlyFireSwitch = ({
  friendlyFire,
  onUpdate,
}: IFriendlyFireSwitchProps) => {
  return (
    <div className="flex items-center justify-between">
      <span className="font-inter">Friendly fire:</span>
      <ReactSwitch
        checked={friendlyFire}
        onChange={(state) => onUpdate(state)}
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

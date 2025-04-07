interface IFreezeTimePickerProps {
  freezeTime: number;
  onUpdate: (newValue: number) => void;
}

export const FreezeTimePicker = ({
  freezeTime,
  onUpdate,
}: IFreezeTimePickerProps) => {
  return (
    <div className="flex items-center justify-between">
      <span className="font-inter">Freeze time, sec:</span>
      <div className="flex items-center gap-4">
        {[1, 4, 6, 8].map((value) => (
          <button
            key={value}
            className={`font-medium ${
              value === freezeTime
                ? "text-white cursor-default"
                : "text-gray hover:text-white"
            }`}
            onClick={() => onUpdate(value)}
          >
            {value}
          </button>
        ))}
      </div>
    </div>
  );
};

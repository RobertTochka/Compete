import { PropsWithChildren, useId } from "react";
import clsx from "classnames";

interface CheckboxProps {
  isChecked: boolean;
  name: string;
  value: string;
  onChange: (value: string, name: string, checked: boolean) => void;
  squared?: boolean;
}

export default function Checkbox({
  children,
  isChecked,
  name,
  onChange,
  value,
  squared,
}: PropsWithChildren<CheckboxProps>) {
  const id = useId();
  return (
    <label
      htmlFor={id}
      className="flex items-center gap-[11px] font-medium cursor-pointer"
    >
      <div
        className={clsx(
          "w-[14px] h-[14px] border-[1px] border-saturateBlue flex justify-center items-center",
          !squared && "rounded-full"
        )}
      >
        {isChecked && (
          <div
            className={clsx(
              "w-[10px] h-[10px] bg-saturateBlue",
              !squared && "rounded-full"
            )}
          />
        )}
      </div>
      {children}
      <input
        type="checkbox"
        name={name}
        value={value}
        id={id}
        className="hidden"
        checked={isChecked}
        onChange={(e) =>
          onChange(e.target.value, e.target.name, e.target.checked)
        }
      />
    </label>
  );
}

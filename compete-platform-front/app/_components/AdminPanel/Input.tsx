"use client";

import {
  ChangeEvent,
  ClassAttributes,
  InputHTMLAttributes,
  useState,
} from "react";

import Icon from "../Icon";

export type TInput = {
  label?: string;
  icon?: string;
  value?: string;
  onChange?: (e: ChangeEvent<HTMLInputElement>) => void;
  type?: string;
  placeholder?: string;
} & InputHTMLAttributes<HTMLInputElement> &
  ClassAttributes<HTMLInputElement>;

export default function Input(props: TInput) {
  const [focused, setFocused] = useState(false);
  const {
    label,
    icon,
    value,
    onChange,
    placeholder,
    type = "text",
    ...rest
  } = props;
  return (
    <div>
      {label && (
        <label className="block mb-1 ml-2 text-[12px] text-secondGray">
          {label}
        </label>
      )}
      <div className="rounded-full flex items-center gap-4 bg-deepBlue/30 h-[42px] px-4">
        {icon && (
          <Icon icon={icon} defaultColor={focused ? "#ffffff" : "#ffffff40"} />
        )}
        <input
          onBlur={() => setFocused(false)}
          onFocus={() => setFocused(true)}
          className="placeholder:text-white/20"
          type={type}
          placeholder={placeholder}
          value={value}
          onChange={onChange}
          {...rest}
        />
      </div>
    </div>
  );
}

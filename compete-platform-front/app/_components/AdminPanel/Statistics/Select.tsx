"use client";

import { useRef, useState } from "react";
import { useOnClickOutside } from "usehooks-ts";

interface ISelectProps {
  values: string[];
  value: string;
  setValue: (v: string) => void;
}

export default function Select({ values, value, setValue }: ISelectProps) {
  const [open, setOpen] = useState(false);

  const ref = useRef(null);

  useOnClickOutside(ref, () => setOpen(false));

  return (
    <div ref={ref} className={`relative ${open ? "z-10" : "z-0"}`}>
      <div
        className="bg-white text-black/90 font-semibold px-2 py-0.5 rounded-full cursor-pointer hover:opacity-80 transition-opacity flex-middle gap-1 text-[13px]"
        onClick={() => setOpen(!open)}
      >
        <span>{value}</span>
        <div
          className={`${
            open ? "-rotate-180 top-0.5" : "rotate-0"
          } transition-transform relative`}
        >
          <svg
            width="13"
            height="13"
            viewBox="0 0 13 13"
            fill="none"
            xmlns="http://www.w3.org/2000/svg"
          >
            <path
              d="M3 6L6.5 9.5L10 6"
              stroke="black"
              stroke-opacity="0.9"
              stroke-width="1.2"
              stroke-linecap="round"
              strokeLinejoin="round"
            />
          </svg>
        </div>
      </div>
      {open && (
        <ul className="bg-white text-black/70 font-medium rounded-md mt-1 text-[13px] shadow-xl">
          {values.map(
            (val) =>
              val !== value && (
                <li
                  className="px-2.5 cursor-pointer py-[3px] hover:bg-black/10"
                  key={val}
                  onClick={() => {
                    setValue(val);
                    setOpen(false);
                  }}
                >
                  {val}
                </li>
              )
          )}
        </ul>
      )}
    </div>
  );
}

import { ChangeEvent, ClassAttributes, HTMLAttributes } from "react";

export type TTextarea = {
  label?: string;
  value?: string;
  onChange?: (e: ChangeEvent<HTMLTextAreaElement>) => any;
  placeholder?: string;
} & HTMLAttributes<HTMLTextAreaElement> &
  ClassAttributes<HTMLTextAreaElement>;

export default function Textarea(props: TTextarea) {
  const { label, value, onChange, placeholder, ...rest } = props;
  return (
    <div>
      {label && (
        <label className="block mb-1 ml-2 text-[12px] text-secondGray">
          {label}
        </label>
      )}
      <textarea
        value={value}
        onChange={onChange}
        className="rounded-xl flex items-center gap-4 bg-deepBlue/30 h-52 py-4 px-5 placeholder:text-white/20 w-full custom-scrollbar whitespace-pre-wrap"
        placeholder={placeholder}
        {...rest}
      ></textarea>
    </div>
  );
}

import { FieldHookConfig, useField } from "formik";
import React, { ClassAttributes, InputHTMLAttributes } from "react";

export const Input = (
  props: InputHTMLAttributes<HTMLInputElement> &
    ClassAttributes<HTMLInputElement> &
    FieldHookConfig<string>
) => {
  const [field] = useField(props);
  return (
    <input
      {...props}
      {...field}
      className="rounded-md py-3 flex items-center gap-4 bg-deepBlue/30 px-4 placeholder:text-white/20"
    />
  );
};

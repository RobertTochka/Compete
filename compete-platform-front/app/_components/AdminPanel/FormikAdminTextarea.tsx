import React from "react";
import Textarea, { TTextarea } from "./Textarea";
import { FieldHookConfig, useField } from "formik";

type FormikAdminTextareaProps = TTextarea & FieldHookConfig<string>;
export const FormikAdminTextarea = (props: FormikAdminTextareaProps) => {
  const [field] = useField(props);
  return <Textarea {...props} {...field}></Textarea>;
};

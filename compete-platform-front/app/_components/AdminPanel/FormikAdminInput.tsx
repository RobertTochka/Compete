import React from "react";
import Input, { TInput } from "./Input";
import { FieldHookConfig, useField } from "formik";

type FormikAdminInputProps = TInput & FieldHookConfig<string>;

export const FormikAdminInput = (props: FormikAdminInputProps) => {
  const [field] = useField(props);
  return <Input {...props} {...field}></Input>;
};

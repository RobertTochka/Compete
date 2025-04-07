import React, { ReactNode } from "react";
import ModalWindow from "../ModalWindow";
import { IDefaultModalProps } from "../ChangeTeamNameModal/ChangeTeamNameModal";
import { Formik } from "formik";
import { Information } from "../Loading/Loading";

interface IPayModalProps extends IDefaultModalProps {
  buttonText?: string;
  initialValues: { [key: string]: any };
  validationScheme: any;
  placeholderTexts: { [key: string]: string };
}
export const PayModal = ({
  onSubmit,
  buttonText,
  initialValues,
  validationScheme,
  placeholderTexts,
  isLoading,
  ...rest
}: IPayModalProps) => {
  return (
    <ModalWindow {...rest}>
      <Formik
        validationSchema={validationScheme}
        initialValues={initialValues}
        onSubmit={(values, {}) => {
          onSubmit(values);
        }}
      >
        {({ handleSubmit, handleChange, errors, values, touched }) => (
          <form
            onSubmit={handleSubmit}
            className="w-full flex flex-col gap-y-4"
          >
            {Object.keys(initialValues).map((key) => (
              <>
                <input
                  type="text"
                  name={key}
                  value={values[key]}
                  onChange={handleChange}
                  data-type="number"
                  placeholder={placeholderTexts[key]}
                  className="rounded-md py-3 flex items-center gap-4 bg-deepBlue/30 px-4 placeholder:text-white/20"
                />
                {errors[key] &&
                  touched[key] &&
                  typeof errors[key] === "string" && (
                    <div className="text-left text-[16px] font-medium text-negative mt-[7px]">
                      {errors[key] as ReactNode}
                    </div>
                  )}
              </>
            ))}
            {isLoading && (
              <Information size={30} loading={isLoading}></Information>
            )}
            <button
              type="submit"
              className="text-[20px] px-5 font-medium rounded-[10px] bg-saturateBlue  hover:bg-secondaryBlue whitespace-nowrap m-5 mx-auto py-3"
            >
              {buttonText}
            </button>
          </form>
        )}
      </Formik>
    </ModalWindow>
  );
};

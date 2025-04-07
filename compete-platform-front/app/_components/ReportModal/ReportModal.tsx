"use client";
import React from "react";
import ModalWindow from "../ModalWindow";
import { Form, Formik } from "formik";
import Input from "../AdminPanel/Input";
import Textarea from "../AdminPanel/Textarea";
import { IDefaultModalProps } from "../ChangeTeamNameModal/ChangeTeamNameModal";
import { useRouter } from "next/navigation";
import { GetReportDto } from "@/app/_utils/types";
import { Information } from "../Loading/Loading";
import * as yup from "yup";
import { FormikAdminTextarea } from "../AdminPanel/FormikAdminTextarea";
import { FormikAdminInput } from "../AdminPanel/FormikAdminInput";
interface IReportModalProps extends IDefaultModalProps, Partial<GetReportDto> {
  forRead?: boolean;
}

const schema = yup.object().shape({
  subject: yup
    .string()
    .min(10, "Не менее 10 знаков для темы жалобы")
    .required("Поле обязательное"),

  content: yup
    .string()
    .min(50, "Не менее 50 знаков для содержания жалобы")
    .required("Поле обязательное"),
});

export const ReportModal = ({
  onClose,
  forRead,
  subject,
  content,
  isLoading,
  errorMessage,
  onSubmit,
  lobbyId,
}: IReportModalProps) => {
  const router = useRouter();
  const onGoToLobby = () => {
    router.push(`/admin-panel/matches?search=${lobbyId}&findBy=${"FindById"}`);
  };
  return (
    <ModalWindow onClose={onClose}>
      <div className="flex flex-col gap-y-6">
        {forRead && (
          <button
            className="p-2 rounded-[15px] bg-saturateBlue font-medium text-[15px] self-start"
            onClick={onGoToLobby}
          >
            Перейти к лобби
          </button>
        )}

        <Formik
          initialValues={{
            subject: subject || "",
            content: content || "",
          }}
          validationSchema={schema}
          onSubmit={(values) => {
            onSubmit(values);
          }}
        >
          {({ errors, touched }) => (
            <Form>
              <FormikAdminInput
                name="subject"
                readOnly={forRead}
                label="Тема"
              ></FormikAdminInput>
              {errors.subject && touched.subject && (
                <div className="text-left text-[16px] font-medium text-negative mt-[7px]">
                  {errors.subject}
                </div>
              )}
              <div className="mt-5">
                <FormikAdminTextarea
                  name="content"
                  readOnly={forRead}
                  label="Содержание"
                ></FormikAdminTextarea>
              </div>
              {errors.content && touched.content && (
                <div className="text-left text-[16px] font-medium text-negative mt-[7px]">
                  {errors.content}
                </div>
              )}
              {!forRead && (
                <button className="mt-5 rounded-[10px] w-full text-[20px] font-medium leading-normal py-1.5 px-6 bg-[#AD0000]  hover:bg-[#bb5454]">
                  Отправить
                </button>
              )}
            </Form>
          )}
        </Formik>
        {(errorMessage || isLoading) && (
          <Information
            loading={isLoading}
            errorMessage={errorMessage}
            size={40}
          ></Information>
        )}
      </div>
    </ModalWindow>
  );
};

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
  onCloseReport: (id: number, reportResponse: string) => void;
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

export const ReportAdminModal = ({
  onClose,
  subject,
  content,
  isLoading,
  errorMessage,
  onCloseReport,
  id,
}: IReportModalProps) => {
  return (
    <ModalWindow onClose={onClose}>
      <div className="flex flex-col gap-y-6">
        <div>
          {subject && (
            <div className="text-left text-[16px] font-medium mt-[7px]">
              <strong>Тема:</strong>
              <p className="mt-2">{subject}</p>
            </div>
          )}
          {content && (
            <div className="text-left text-[16px] font-medium mt-[27px]">
              <strong>Содержание:</strong>
              <p className="mt-2 flex">{content}</p>
            </div>
          )}
        </div>

        <button
          onClick={() => {
            onCloseReport(id, "Одобрено");
          }}
          className="text-[20px] mt-5 w-full px-5 font-medium rounded-[10px] bg-saturateBlue  hover:bg-secondaryBlue whitespace-nowrapm-5 py-3"
        >
          Одобрить
        </button>
        <button
          onClick={() => onCloseReport(id, "Отклонено")}
          className="rounded-[10px] w-full text-[20px] font-medium px-5 bg-[#AD0000] whitespace-nowrapm-5 py-3 hover:bg-[#bb5454]"
        >
          Отклонить
        </button>

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

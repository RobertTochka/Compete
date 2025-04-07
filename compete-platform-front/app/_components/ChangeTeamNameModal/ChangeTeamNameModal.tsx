"use client";
import { Form, Formik } from "formik";
import ModalWindow, { IModalWindow } from "../ModalWindow";
import { Input } from "../Input/Input";
import { Information } from "../Loading/Loading";

export interface IDefaultModalProps {
  onClose: () => void;
  errorMessage?: string;
  isLoading?: boolean;
  onSubmit?: (newValue: any) => void;
}

interface IChangeTeamModalProps extends IDefaultModalProps {
  initialName: string;
}
export const ChangeTeamNameModal = ({
  onClose,
  isLoading = false,
  errorMessage,
  onSubmit = () => {},
  initialName,
}: IChangeTeamModalProps) => {
  return (
    <ModalWindow onClose={onClose}>
      <Formik
        initialValues={{ name: initialName }}
        onSubmit={(values) => onSubmit(values.name)}
      >
        {({}) => (
          <Form className="w-full flex flex-col gap-y-4">
            <Input
              name="name"
              placeholder="Введите название"
              type="text"
            ></Input>
            {
              <Information
                loading={isLoading}
                errorMessage={errorMessage}
              ></Information>
            }
            <button className="text-[20px] p-5 font-medium rounded-[10px] bg-saturateBlue  hover:bg-secondaryBlue whitespace-nowrap">
              Изменить название
            </button>
          </Form>
        )}
      </Formik>
    </ModalWindow>
  );
};

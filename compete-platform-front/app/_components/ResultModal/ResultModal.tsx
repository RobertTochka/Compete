import React from "react";
import ModalWindow from "../ModalWindow";
import Image from "next/image";
import { IDefaultModalProps } from "../ChangeTeamNameModal/ChangeTeamNameModal";

export interface ResultModalProps extends IDefaultModalProps {
  successMessage?: string;
  img?: string;
}

export const ResultModal = ({
  successMessage,
  errorMessage,
  onClose,
  img,
  ...rest
}: ResultModalProps) => {
  const src =
    img !== undefined
      ? img
      : successMessage
      ? "/img/success.png"
      : errorMessage
      ? "/img/error.png"
      : null;
  const text = successMessage || errorMessage || null;
  if (!src || !text) {
    return <p>ОШИБКА!</p>;
  }

  return (
    <ModalWindow onClose={() => {}}>
      <div className="flex flex-col">
        <img
          alt="Изображение результата"
          src={src}
          className="h-[150px] w-full object-contain"
        />
        <span className="text-center d-flex mx-auto text-[16px] font-medium whitespace-pre-line mt-5">
          {text}
        </span>
        <button
          onClick={onClose}
          className="text-[20px] px-5 font-medium rounded-[10px]
         bg-saturateBlue  hover:bg-secondaryBlue whitespace-nowrap 
         m-5 mx-auto 
         py-3"
        >
          ОК
        </button>
      </div>
    </ModalWindow>
  );
};

"use client";
import ModalWindow from "../ModalWindow";
import { GetUserDto } from "@/app/_utils/types";
import User from "../Platform/User";
import { IDefaultModalProps } from "../ChangeTeamNameModal/ChangeTeamNameModal";
import { Information } from "../Loading/Loading";

interface AddPlayerModalProps extends IDefaultModalProps {
  friends: GetUserDto[];
}

export const AddPlayerModal = ({
  friends,
  onSubmit,
  isLoading,
  errorMessage,
  ...props
}: AddPlayerModalProps) => {
  return (
    <ModalWindow {...props}>
      <div className="flex flex-col items-center">
        <Information
          label="Список друзей"
          loading={isLoading}
          errorMessage={errorMessage}
        ></Information>
        <div className="grid grid-cols-2 gap-9">
          {friends.map((_) => (
            <User {..._} key={_.id} onClick={() => onSubmit(_.id)} />
          ))}
        </div>
      </div>
    </ModalWindow>
  );
};

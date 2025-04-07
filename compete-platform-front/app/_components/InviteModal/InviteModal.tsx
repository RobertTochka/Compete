import React, { useMemo } from "react";
import { IDefaultModalProps } from "../ChangeTeamNameModal/ChangeTeamNameModal";
import ModalWindow from "../ModalWindow";
import { JoinToLobbyInfo } from "@/app/_utils/types";
import Avatar from "../Platform/Avatar";
import { Information } from "../Loading/Loading";
import { handleError } from "@/app/_utils/functions";
import { useGetUserProfileQuery } from "@/app/_fetures/lib/api/publicLobbiesApi";

interface InviteModalProps extends IDefaultModalProps {
  joinInfo: JoinToLobbyInfo;
}

export const InviteModal = ({
  joinInfo,
  onSubmit = () => {},
  isLoading = false,
  errorMessage,
  onClose,
}: InviteModalProps) => {
  const userRequest = useMemo(
    () => ({
      userId: joinInfo.inviterId!,
      includeFriends: false,
    }),
    [joinInfo]
  );
  const {
    data: inviter,
    isLoading: inviterLoading,
    error,
  } = useGetUserProfileQuery(userRequest);
  const inviterErrorMessage = handleError(error);
  return (
    <ModalWindow onClose={() => {}}>
      <div className="flex flex-col items-center gap-y-6">
        <Avatar width={90} height={90} image_url={inviter?.avatarUrl}></Avatar>

        <Information
          label={` Пользователь ${inviter?.name} приглашает вас в свою команду`}
          loading={isLoading || inviterLoading}
          errorMessage={errorMessage || inviterErrorMessage}
        ></Information>
        <div className="flex gap-x-5">
          <button
            className="text-[20px] px-5 font-medium rounded-[10px] bg-saturateBlue  hover:bg-secondaryBlue whitespace-nowrap m-5 mx-auto py-3"
            onClick={() => onSubmit(joinInfo)}
          >
            Принять
          </button>
          <button
            className="text-[20px] px-5 font-medium rounded-[10px]  whitespace-nowrap m-5 mx-auto py-3 bg-red-600  hover:bg-red-400"
            onClick={() => onClose()}
          >
            Отклонить
          </button>
        </div>
      </div>
    </ModalWindow>
  );
};

import React, { useState } from "react";
import ModalWindow from "../ModalWindow";
import { IDefaultModalProps } from "../ChangeTeamNameModal/ChangeTeamNameModal";
import { Information } from "../Loading/Loading";
import {
  useCancelLobbyMutation,
  useGetUsersInLobbyQuery,
} from "@/app/_fetures/lib/api/publicLobbiesApi";
import { useHandleError } from "@/app/_utils/hooks/useTemplatedError";
import User from "../Platform/User";

interface ICancelMatchModalProps extends IDefaultModalProps {
  lobbyId: number;
}

export const CancelMatchModal = ({ ...rest }: ICancelMatchModalProps) => {
  const {
    data: users,
    isLoading: usersLoading,
    error: usersError,
  } = useGetUsersInLobbyQuery(rest.lobbyId);

  const [cancelLobby, { isLoading: cancelingLobby, error: cancelationError }] =
    useCancelLobbyMutation();
  const [offenders, setOffenders] = useState<number[]>([]);
  const onOffender = (userId: number) => {
    if (offenders.includes(userId))
      setOffenders((prev) => prev.filter((u) => u != userId));
    else setOffenders((prev) => [...prev, userId]);
  };
  const onCancel = () => {
    cancelLobby({ id: rest.lobbyId, offenders });
  };
  const commonErrorText = useHandleError(usersError || cancelationError);
  return (
    <ModalWindow {...rest}>
      <div className="flex flex-col items-center">
        <Information
          label="Список участников"
          loading={(usersLoading && !users) || cancelingLobby}
          errorMessage={commonErrorText}
        ></Information>
        <div className="grid grid-cols-2 gap-9 mt-2">
          {users?.map((_) => (
            <User
              {..._}
              key={_.id}
              onClick={() => onOffender(_.id)}
              isSelected={offenders.includes(_.id)}
            />
          ))}
        </div>
        <button
          onClick={onCancel}
          className="mt-4 rounded-[10px] w-full text-[20px] font-medium leading-normal py-1.5 px-6 bg-[#AD0000]  hover:bg-[#bb5454]"
        >
          Отменить лобби
        </button>
      </div>
    </ModalWindow>
  );
};

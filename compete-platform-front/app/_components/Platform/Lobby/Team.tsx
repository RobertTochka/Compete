"use client";
import clsx from "clsx";
import Icon from "../../Icon";
import { GetTeamDto, GetUserDto } from "@/app/_utils/types";
import { TeamPlayerView } from "./TeamPlayerView";
import { AddPlayerView } from "./AddPlayerView";
import { useMemo, useState } from "react";
import { DetailsText } from "../../DetailsText/DetailsText";
import { ChangeTeamNameModal } from "../../ChangeTeamNameModal/ChangeTeamNameModal";
import {
  truncateString,
  useInviteUser,
  useUpdateTeamName,
} from "@/app/_utils/functions";
import { AddPlayerModal } from "../../AddPlayerModal/AddPlayerModal";
import { useHandleError } from "@/app/_utils/hooks/useTemplatedError";
import { useGetUserProfileQuery } from "@/app/_fetures/lib/api/publicLobbiesApi";
export interface ITeamProps extends Omit<GetTeamDto, "lobby"> {
  teamLength: number;
  userBids: { [key: string]: number };
  userAwards?: { [key: string]: number };
  isWinner?: boolean;
  currentUserId: number;
  isSecondTeam?: boolean;
}
export type TeamPlayer = GetUserDto | null;

export default function Team({
  name,
  users,
  creatorId,
  currentUserId,
  userBids,
  userAwards,
  teamLength,
  isWinner,
  isSecondTeam,
}: ITeamProps) {
  const players: TeamPlayer[] = [
    ...users,
    ...new Array(
      teamLength - users?.length < 0 ? 0 : teamLength - users?.length
    ).map((u) => undefined),
  ];
  const userReq = useMemo(
    () => ({ userId: undefined, includeFriends: true }),
    []
  );

  const {
    data: user,
    isLoading: friendsLoading,
    error: friendsError,
  } = useGetUserProfileQuery(userReq);
  const handledFriendsError = useHandleError(friendsError);

  const [updateTeamName, { condition: updateCondition }] = useUpdateTeamName();
  const [onUserInvite, { condition: inviteCondition }] = useInviteUser();

  const [showModal, setShowModal] = useState(false);
  const [showUserInvite, setShowUserInvite] = useState<boolean>(false);

  const leader = useMemo(() => {
    const creator = users.find((u) => u.id === creatorId);
    if (!creator) return creator;
    return {
      win: creator.matches * creator.winrate,
      lose: creator.matches * (1 - creator.winrate),
      ...creator,
    };
  }, [users, creatorId]);

  return (
    <>
      {showUserInvite && (
        <AddPlayerModal
          isLoading={inviteCondition === "loading" || friendsLoading}
          errorMessage={
            inviteCondition !== "idle" && inviteCondition !== "loading"
              ? inviteCondition
              : handledFriendsError
          }
          onSubmit={onUserInvite}
          friends={user?.friends ?? []}
          onClose={() => setShowUserInvite(false)}
        ></AddPlayerModal>
      )}
      {showModal && (
        <ChangeTeamNameModal
          initialName={name}
          onClose={() => setShowModal(false)}
          isLoading={updateCondition === "loading"}
          onSubmit={updateTeamName}
          errorMessage={
            updateCondition !== "idle" && updateCondition !== "loading"
              ? updateCondition
              : undefined
          }
        ></ChangeTeamNameModal>
      )}
      <div className="w-full max-w-[440px]">
        <div className={`flex items-center gap-[40px] mb-[40px] ${isSecondTeam ? "flex-row-reverse" : ""}`}>
          <h3 className={`text-[24px] font-medium relative z-0`
          }>
            {currentUserId === creatorId && !userAwards &&
              <div className={`absolute ${isSecondTeam ? "left-[-20px]" : "right-[-20px]"} top-[50%] w-[1px] h-[20px] translate-y-[-50%] bg-white`} />
            }
            {truncateString(name, 16)}
          </h3>
          {currentUserId === creatorId && !userAwards && (
            <Icon
              icon="pencil"
              defaultColor="#fff"
              onClick={() => setShowModal(true)}
            />
          )}
        </div>
        <div className="flex flex-col gap-y-[40px]">
          {players?.map((player, i) =>
            player ? (
              <TeamPlayerView
                key={player.id}
                win={isWinner}
                award={userAwards ? userAwards[`${player.id}`] : undefined}
                bid={userBids[`${player.id}`]}
                {...player}
                isLeader={player.id === creatorId}
              ></TeamPlayerView>
            ) : (
              <AddPlayerView
                key={i}
                showLastColumn={userAwards !== undefined}
                isUnknown={creatorId !== user?.id ?? false}
                onClick={() =>
                  creatorId !== user?.id ? {} : setShowUserInvite(true)
                }
              ></AddPlayerView>
            )
          )}
        </div>
      </div>
    </>
  );
}

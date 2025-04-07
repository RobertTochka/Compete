"use client";

import { useEffect, useMemo } from "react";
import {
  useGetLobbyQuery,
  useGetUserProfileQuery,
} from "@/app/_fetures/lib/api/publicLobbiesApi";

import { Veto } from "@/app/_components/Veto/Veto";
import { GetUserRequest, LobbyStatus } from "@/app/_utils/types";
import { Information } from "@/app/_components/Loading/Loading";
import { handleError } from "@/app/_utils/functions";
import { useParams, useRouter } from "next/navigation";
import { MatchPrepare } from "@/app/_components/MatchPrepare/MatchPrepare";
import { MatchEndPage } from "@/app/_components/MatchEndPage/MatchEndPage";

export default function Lobby() {
  const router = useRouter();
  const { id } = useParams();
  let lobbyId = id ? parseInt(id.toString()) : undefined;
  const {
    data: actionInfo,
    isLoading,
    error,
  } = useGetLobbyQuery({ id: lobbyId });
  const userReq: GetUserRequest = useMemo(
    () => ({ userId: undefined, includeFriends: false }),
    []
  );
  const { data: user, isLoading: userLoading } =
    useGetUserProfileQuery(userReq);
  const lobby = actionInfo?.newLobby;
   useEffect(() => {
     if (
       actionInfo &&
       actionInfo.newLobby &&
       actionInfo.newLobby.status === LobbyStatus.Configuring
     )
       router.push(`/create-lobby/${id}`);
   }, [actionInfo]);
  const getActionInfoError = handleError(error);
  if ((isLoading && !actionInfo) || (userLoading && !user) || !lobby || !user)
    return (
      <Information
        size={90}
        loading={(isLoading && !actionInfo) || (userLoading && !user)}
        errorMessage={getActionInfoError}
      />
    );
  if (
    lobby.status === LobbyStatus.Playing ||
    lobby.status === LobbyStatus.Warmup
  )
    return <MatchPrepare {...lobby} userId={user.id} />;
  if (
    lobby.status === LobbyStatus.Canceled ||
    lobby.status === LobbyStatus.Over
  )
    return (
      <MatchEndPage
        {...actionInfo.newLobby}
        userId={user?.id ?? 0}
      ></MatchEndPage>
    );
  return <Veto {...actionInfo} userId={user?.id ?? 0}></Veto>;
}

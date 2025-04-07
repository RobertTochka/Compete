"use client";

import Settings from "@/app/_components/Platform/Lobby/Settings";
import Team from "@/app/_components/Platform/Lobby/Team";
import Chat from "@/app/_components/Platform/Lobby/Chat";
import Maps from "@/app/_components/Platform/Lobby/Maps";
import { ColumnKeyPair } from "@/app/_components/Text/Text";
import { Information } from "@/app/_components/Loading/Loading";
import { useEffect, useMemo } from "react";
import { JoinToLobbyInfo, LobbyStatus, Map } from "@/app/_utils/types";
import { useParams, useSearchParams } from "next/navigation";
import {
  getTeam,
  getMyBid,
  getMyTeamChatId,
  getTeamBalance,
  getUserBids,
  useUpdateConfig,
} from "@/app/_utils/functions";
import { LobbyAdminConfiguration } from "@/app/_utils/types";
import {
  useCreateLobbyMutation,
  useGetLobbyQuery,
  useGetUserProfileQuery,
  useJoinToLobbyMutation,
  useLeaveFromLobbyMutation,
} from "@/app/_fetures/lib/api/publicLobbiesApi";
import { useHandleError } from "@/app/_utils/hooks/useTemplatedError";
import { useRouter } from "next/navigation";

const getParam = (wrappedParam: string | string[] | undefined) => {
  if (typeof wrappedParam === "undefined") return undefined;
  if (typeof wrappedParam === "string") {
    try {
      return parseInt(wrappedParam);
    } catch {
      return undefined;
    }
  }
  if (wrappedParam.length > 0) {
    try {
      return parseInt(wrappedParam[0]);
    } catch {
      return undefined;
    }
  }
  return undefined;
};

export default function CreateLobby() {
  const router = useRouter();
  const searchParams = useSearchParams();
  const { id } = useParams();
  const lobbyId = getParam(id);

  const code = searchParams.get("code");
  const {
    data: actionInfo,
    isLoading,
    error: getLobbyError,
    refetch,
    isFetching,
  } = useGetLobbyQuery({ id: lobbyId }, { refetchOnMountOrArgChange: true });

  const [leaveLobby, { isLoading: lobbyLeaveLoading }] =
    useLeaveFromLobbyMutation();
  const [updateConfig] = useUpdateConfig();
  const [createLobby, { error: createErrorMutation, isLoading: isCreation }] =
    useCreateLobbyMutation();
  const [joinToLobby, { error: joinError, isLoading: isJoining }] =
    useJoinToLobbyMutation();

  const userReq = useMemo(
    () => ({ userId: undefined, includeFriends: false }),
    []
  );
  const {
    data: user,
    isLoading: userLoading,
    isUninitialized,
  } = useGetUserProfileQuery(userReq);

  const onMapChanging = (map: Map) => {
    if (actionInfo === undefined) return;
    const needDelete = actionInfo.newLobby?.pickMaps.includes(map);
    let newMaps = needDelete
      ? actionInfo.newLobby?.pickMaps.filter((m) => m !== map)
      : [...actionInfo.newLobby?.pickMaps, map];
    let newCfg: LobbyAdminConfiguration = {
      ...actionInfo.newLobby,
      pickMaps: newMaps,
    };
    updateConfig(newCfg);
  };

  const onServerPropSetup = (prop: string) => (value: any) => {
    if (actionInfo === undefined) return;
    const newCfg: LobbyAdminConfiguration = {
      ...actionInfo.newLobby,
      config: { ...actionInfo.newLobby.config, [prop]: value },
    };
    updateConfig(newCfg);
  };

  const onPropSetup = (property: string) => (value: any) => {
    if (actionInfo === undefined) return;
    console.log("settuping", property, value);
    const newCfg: LobbyAdminConfiguration = {
      ...actionInfo.newLobby,
      [property]: value,
    };
    updateConfig(newCfg);
  };

  const onLobbyLeave = () => {
    if (actionInfo && user)
      leaveLobby({ userId: user?.id, lobbyId: actionInfo.newLobby?.id }).then(
        () => {
          router.push("/lobbies");
        }
      );
  };

  useEffect(() => {
    if (!id && !isUninitialized)
      createLobby()
        .unwrap()
        .then((e) => {
          refetch();
        });
  }, [id, isUninitialized]);

  useEffect(() => {
    if (
      actionInfo &&
      actionInfo.newLobby.status !== LobbyStatus.Configuring &&
      !isLoading &&
      !isCreation &&
      !isFetching &&
      id !== actionInfo.newLobby.id.toString() &&
      !createErrorMutation &&
      !isUninitialized
    ) {
      router.push(`/lobby/${actionInfo.newLobby.id}`);
    }
  }, [actionInfo, id, isUninitialized, isCreation, isLoading, isFetching]);

  useEffect(() => {
    if (code && id && user) {
      const joinToLobbyInfo: JoinToLobbyInfo = {
        lobbyId,
        userId: user?.id,
        code,
      };
      joinToLobby(joinToLobbyInfo)
        .unwrap()
        .then((t) => {
          refetch();
        });
    }
  }, [code, user, id]);

  const lobbyError = useHandleError(
    createErrorMutation || joinError || getLobbyError
  );
  if (
    (isLoading && !actionInfo) ||
    (!user && userLoading) ||
    isCreation ||
    lobbyError ||
    isJoining
  ) {
    return (
      <Information
        size={90}
        loading={
          (isLoading && !actionInfo) ||
          (!user && userLoading) ||
          isCreation ||
          isJoining
        }
        errorMessage={
          lobbyError == "Такого лобби не существует" ? undefined : lobbyError
        }
      />
    );
  }
  const rightTeam = getTeam(actionInfo?.newLobby, 1);
  const leftTeam = getTeam(actionInfo?.newLobby, 0);
  const leftTeamBalance = getTeamBalance(actionInfo?.newLobby, 0);
  const rightTeamBalance = getTeamBalance(actionInfo?.newLobby, 1);
  const bid = getMyBid(actionInfo?.newLobby, user?.id);
  const userBids = getUserBids(actionInfo?.newLobby);
  const myTeamChatId = getMyTeamChatId(actionInfo?.newLobby, user?.id);
  return (
    <section className="px-[25px] flex-1 flex flex-col overflow-auto">
      <div className="flex justify-between max-w-[1617px] mx-auto w-full flex-1 overflow-auto custom-scrollbar pr-[10px]">
        {actionInfo && (
          <Team
            currentUserId={user?.id ?? -1}
            {...leftTeam}
            teamLength={actionInfo.newLobby.playersAmount}
            userBids={userBids}
          />
        )}
        <div className="flex-1 flex flex-col">
          <div className="mb-[40px] flex gap-[65px] justify-center items-center max-h-max">
            <ColumnKeyPair
              keyValue="Банк команды"
              value={`₽ ${leftTeamBalance}`}
              valuesClassnames="text-[20px]"
            />
            {actionInfo?.newLobby && (
              <ColumnKeyPair
                keyValue="Общий банк"
                value={`₽ ${actionInfo.newLobby.bids
                  .map((b) => b.bid)
                  .reduce((a, v) => a + v, 0)}`}
                valuesClassnames="text-[40px]"
              />
            )}
            <ColumnKeyPair
              keyValue="Банк команды"
              value={`₽ ${rightTeamBalance}`}
              valuesClassnames="text-[20px]"
            />
          </div>
          <div className="flex w-[624px] mx-auto">
            {actionInfo?.newLobby && (
              <Maps
                mapsOnLobby={actionInfo.newLobby.pickMaps}
                onMapChange={onMapChanging}
              />
            )}
            {actionInfo?.newLobby && (
              <Settings
                bid={bid}
                {...actionInfo.newLobby}
                onPropSetup={onPropSetup}
                onServerConfigSetup={onServerPropSetup}
                onLobbyLeave={onLobbyLeave}
              />
            )}
          </div>
          <Chat
            userId={user?.id ?? 0}
            teamChatId={myTeamChatId}
            lobbyChatId={actionInfo?.newLobby?.chatId ?? 0}
          />
        </div>
        {actionInfo?.newLobby && (
          <Team
            currentUserId={user?.id ?? -1}
            {...rightTeam}
            teamLength={actionInfo.newLobby.playersAmount}
            userBids={userBids}
            isSecondTeam
          />
        )}
      </div>
    </section>
  );
}

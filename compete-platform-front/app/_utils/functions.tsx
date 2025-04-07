import * as signalR from "@microsoft/signalr";
import { jwtDecode } from "jwt-decode";

import {
  ActionInfo,
  Format,
  GetChatMessageDto,
  GetLobbyDto,
  GetPayDto,
  GetUserDto,
  JoinToLobbyInfo,
  LobbyAdminConfiguration,
  LobbyStatus,
  MapPickRequest,
  ProblemDetails,
  SendMessageRequest,
  UserStatus,
} from "./types";
import { Notification } from "./types";
import { Map } from "./types";
import { useEffect, useState } from "react";
import { FetchBaseQueryError } from "@reduxjs/toolkit/query";
import { SerializedError } from "@reduxjs/toolkit";
import { isProblemDetails } from "./hooks/useTemplatedError";
import toast from "react-hot-toast";
import { IPrepareMapViewProps } from "../_components/PrepareMapView/PrepareMapView";
import { IMenuLink } from "../_components/Platform/Menu";
import { useGetUserStatusQuery } from "../_fetures/lib/api/publicLobbiesApi";

const defaultTeam = {
  id: 0,
  creatorId: 0,
  name: "Default",
  users: [],
  chatId: 0,
};
type Selector<T, K> = (element: T) => K;

export function distinctBy<T, K>(array: T[], selector: Selector<T, K>): T[] {
  const seen = new Set<K>();
  return array?.filter((element) => {
    //
    const key = selector(element);
    if (seen.has(key)) {
      return false;
    } else {
      seen.add(key);
      return true;
    }
  });
}
export const getTeam = (lobby: GetLobbyDto | undefined, order: number = 1) => {
  if (lobby === undefined) return { ...defaultTeam };
  if (lobby.teams.length === 1 && order === 1) return { ...defaultTeam };
  const selector = (user: GetUserDto) => user.id;
  let team = { ...lobby.teams[order] };
  team.users = distinctBy(team.users, selector);
  return team;
};

export const getQueryParameter = (url: string, name: string) => {
  const queryString = url.split("?")[1];
  if (!queryString) return null;

  const params = new URLSearchParams(queryString);
  return params.get(name);
};
export const handleMapsActionInfo = (lobby: GetLobbyDto) => {
  const actions = lobby.mapActions;
  const alreadyPicker = actions.map((a) => a.map);
  const allMaps = lobby.pickMaps;
  return allMaps.map((m) => {
    let state = "waiting";
    if (alreadyPicker.includes(m))
      state = actions.find((w) => w.map === m)?.isPicked ? "pick" : "ban";
    return {
      title: mapToTitle(m),
      state,
      preview: `/img/maps/previews/${mapToTitle(m).toLowerCase()}.jpg`,
    };
  });
};

const MAPS = [
  "mirage",
  "inferno",
  "nuke",
  "anubis",
  "overpass",
  "vertigo",
  "ancient",
  "dust2",
  "office",
  "italy",
];

export const mapToTitle = (map: Map) => MAPS[map as number];

export const titleToMap = (title: string) => MAPS.findIndex((m) => m === title);

const STATUSES = [
  "Набор игроков",
  "Выбор карт",
  "Идет матч",
  "Матч отменен",
  "Матч окончен",
  "Разминка",
];

export const reportStatusToTitle = (status: string) => {
  if (status === "Open") return "Рассматривается";
  return "Закрыт";
};

export const lobbyStatusToTitle = (status: LobbyStatus) => {
  if (status === 0) return STATUSES[0];
};

export function formatDateWithTime(dateString) {
  const date = new Date(dateString);
  return date
    .toLocaleString("ru-RU", {
      day: "2-digit",
      month: "2-digit",
      year: "numeric",
      hour: "2-digit",
      minute: "2-digit",
      timeZone: "Europe/Moscow",
    })
    .replace(",", "");
}

export const getMyTeamChatId = (
  lobby: GetLobbyDto | undefined,
  userId: number | undefined
) => {
  if (!lobby || !userId) return 0;
  const team = lobby.teams.filter((t) =>
    t.users.map((u) => u.id).includes(userId)
  );
  if (team.length > 0) return team[0].chatId;
  return 0;
};

export function formatCsMapName(mapName: string): string {
  const mapPrefixes: { [key: string]: string } = {
    mirage: "de_",
    dust2: "de_",
    inferno: "de_",
    nuke: "de_",
    train: "de_",
    overpass: "de_",
    italy: "cs_",
    office: "cs_",
    assault: "cs_",
    militia: "cs_",
  };

  const mapNameLower = mapName.toLowerCase();

  const prefix = mapPrefixes[mapNameLower] || "de_";

  return `${prefix}${mapNameLower}`;
}

export function downloadBlob(blob: Blob, filename: string) {
  const url = URL.createObjectURL(blob);

  const a = document.createElement("a");
  a.href = url;
  a.download = filename;

  document.body.appendChild(a);
  a.click();

  document.body.removeChild(a);
  URL.revokeObjectURL(url);
}

export const getMyTeamId = (
  lobby: GetLobbyDto | undefined,
  userId: number | undefined
) => {
  if (!lobby || !userId) return 0;
  const team = lobby.teams.filter((t) =>
    t.users.map((u) => u.id).includes(userId)
  );
  if (team.length > 0) return team[0].id;
  return 0;
};

export function keyValueToString(obj: { [key: string]: any }): string {
  let result = "";
  for (let key in obj) {
    if (obj.hasOwnProperty(key)) {
      result += `${key}:${obj[key]}`;
    }
  }
  return result;
}
export const getTemplatedError = (error: ProblemDetails) => {
  let message = "";
  if (error.title != undefined) message = message.concat(`${error.title}\n`);
  if (error.detail != undefined) message = message.concat(`${error.detail}\n`);
  if (error.errors != undefined) {
    for (let i = 0; i < error.errors.length; i++) {
      message = `${message.concat(keyValueToString(error.errors[i]))}\n`;
    }
  }
  return message;
};

export function isNumeric(value: string) {
  return /^-?\d+$/.test(value);
}

export function groupByCreationTime(objects: GetPayDto[]) {
  return objects.reduce((acc: any, obj) => {
    const key = obj.creationTime;
    if (!acc[key]) {
      acc[key] = [];
    }
    acc[key].push(obj);
    return acc;
  }, {});
}

export const getUserBids = (lobby: GetLobbyDto | undefined) => {
  let result: { [key: string]: number } = {};
  if (lobby === undefined) return {};
  for (let i = 0; i < lobby.bids.length; i++) {
    result[`${lobby.bids[i].userId}`] = lobby.bids[i].bid;
  }
  return result;
};
export const getUserAwards = (lobby: GetLobbyDto | undefined) => {
  let result: { [key: string]: number } = {};
  if (lobby === undefined) return {};
  for (let i = 0; i < lobby.awards.length; i++) {
    result[`${lobby.awards[i].userId}`] = lobby.awards[i].award;
  }
  return result;
};
export const getTeamBalance = (
  lobby: GetLobbyDto | undefined,
  team: number
) => {
  if (lobby === undefined) return 0;
  if (team !== 0 && lobby.teams.length === 1) return 0;
  return lobby.bids
    .filter((b) => lobby.teams[team].users.map((u) => u.id).includes(b.userId))
    .reduce((a, v) => a + v.bid, 0);
};

export const getTextMatchFormat = (matchformat: Format) => {
  if (matchformat === 1) return "Ban/Ban/Ban/Ban/Ban/Ban/Decider";
  else if (matchformat === 2) return "Ban/Ban/Pick/Pick/Ban/Ban/Decider";
  else return "Ban/Ban/Pick/Pick/Pick/Pick/Decider";
};

export const getTeamScore = (lobby: GetLobbyDto, teamId: number): string => {
  let score = "0:0";
  let pickedMaps = lobby.mapActions.filter((c) => c.isPicked);
  let match = lobby.matches.find((m) => m.playedMap === pickedMaps.at(-1)?.map);
  if (match) {
    if (match.teamId === teamId)
      score = `${match.firstTeamScore}:${match.secondTeamScore}`;
    else score = `${match.secondTeamScore}:${match.firstTeamScore}`;
  }
  return score;
};

export const aggregateMapsWithScore = (
  lobby: GetLobbyDto,
  teamId: number
): IPrepareMapViewProps[] => {
  let pickedMaps = lobby.mapActions.filter((c) => c.isPicked);

  return pickedMaps.map((a) => {
    let score = "0:0";
    let result: IPrepareMapViewProps = {
      title: mapToTitle(a.map),
      score,
      isPickedByUs: a.teamId == teamId,
      preview: `/img/maps/previews/${mapToTitle(a.map).toLowerCase()}.jpg`,
    };
    let match = lobby.matches.find((m) => m.playedMap === a.map);

    if (match) {
      if (match.teamId === teamId)
        score = `${match.firstTeamScore}:${match.secondTeamScore}`;
      else score = `${match.secondTeamScore}:${match.firstTeamScore}`;
      result.score = score;
    }
    return result;
  });
};

export let connection: signalR.HubConnection | null = null;
export async function start() {
  if (connection == null) {
    connection = new signalR.HubConnectionBuilder()
      .withUrl("/api/eventHub")
      .configureLogging(signalR.LogLevel.Error)
      .build();
  }
  try {
    if (connection.state === signalR.HubConnectionState.Connected) return;
    await connection.start();
    toast.success("Подключение установлено");
  } catch (err) {
    toast.error("Проблемы с подключением");

    setTimeout(() => start(), 5000);
  }
  connection.onclose(() => {
    if (connection?.state === signalR.HubConnectionState.Disconnected) start();
  });
}

export const configureConnectionForLobbyEvents = (
  onNewConfig: (lobby: ActionInfo) => void
) => {
  intervalTrying(() =>
    connection!.on("LobbyChanged", (lobby: ActionInfo) => {
      onNewConfig(lobby);
    })
  );
};

const intervalTrying = (func: () => void) => {
  const timerId = setInterval(() => {
    try {
      func();
      clearInterval(timerId);
    } catch (e) {
      console.log(e);
    }
  }, 1000);
};

export const configureConnectionForMessages = (
  onNewMessage: (msg: GetChatMessageDto) => void
) => {
  intervalTrying(() =>
    connection.on("GetChatMessage", (msg) => {
      onNewMessage(msg);
    })
  );
};

export const getMyBid = (
  lobby: GetLobbyDto | undefined,
  userId: number | undefined
) => {
  if (lobby === undefined || userId === undefined) return 0;
  const bidStatus = lobby.bids.find((b) => b.userId === userId);
  if (bidStatus) return bidStatus.bid;
  return 0;
};
export const getMyAward = (
  lobby: GetLobbyDto | undefined,
  userId: number | undefined
) => {
  if (lobby === undefined || userId === undefined) return 0;
  return lobby.awards.find((b) => b.userId === userId)?.award ?? 0;
};
export function formatNumber(num: number, useComma: boolean = true): string {
  const formattedNumber = num.toFixed(1);

  let [integerPart, decimalPart] = formattedNumber.split(".");

  integerPart = integerPart.replace(/\B(?=(\d{3})+(?!\d))/g, " ");

  const decimalSeparator = useComma ? "," : ".";
  return `${integerPart}${decimalSeparator}${decimalPart}`;
}

export const configureConnectionForNotifications = (
  onNote: (note: Notification) => void
) => {
  intervalTrying(() =>
    connection!.on("GetMessage", (note: Notification) => {
      onNote(note);
    })
  );
};

export const configureConnectionForPickMapSecondsNotifications = (
  onNote: (seconds: number) => void
) => {
  intervalTrying(() =>
    connection!.on("NotifyAboutAvailablePickMapSeconds", onNote)
  );
};

export const disposeTeamMessages = () => connection?.off("GetChatMessage");
export const disposeLobbyEvents = () => connection?.off("LobbyChanged");
export const disposeNotificationsAndInvites = () => {
  connection?.off("GetInvite");
  connection?.off("GetMessage");
};

export const disposeConnectionForPickMapSecondsNotifications = () =>
  connection?.off("NotifyAboutAvailablePickMapSeconds");

export const configureConnectionForInvites = (
  onInvite: (info: JoinToLobbyInfo) => void
) => {
  intervalTrying(() =>
    connection!.on("GetInvite", (info) => {
      onInvite(info);
    })
  );
};

export const handleMutationError = (e: any) => {
  let error = handleError(e);
  toast.error(error === undefined ? "Произошла ошибка" : error);
};

export const handleError = (
  error: FetchBaseQueryError | SerializedError | undefined
) => {
  const defaultMessage = "Произошла ошибка при выполнении операции";
  if (error === undefined) return undefined;
  if ("originalStatus" in error) return undefined;
  if (!("data" in error)) return defaultMessage;
  if (isProblemDetails(error.data)) {
    return getTemplatedError(error.data);
  }
  return defaultMessage;
};

export const updateConfig = (newCfg: LobbyAdminConfiguration) =>
  connection?.invoke("SendLobbyChange", newCfg).catch(console.log);

export const updateBid = (newBid: number) => {
  return connection?.invoke("ChangeUserBid", newBid);
};

export const pickMap = (req: MapPickRequest) => {
  return connection?.invoke("SelectMap", req.map, req.lobbyId);
};

export const updateTeamName = (teamName: string) => {
  return connection?.invoke("ChangeTeamName", teamName);
};

export const inviteUser = (userId: number) =>
  connection?.invoke("SendInvite", userId);

export const sendMessage = (req: SendMessageRequest) =>
  connection?.invoke("SendMessage", req);

export const sendAppealMessage = (req: SendMessageRequest) =>
  connection?.invoke("SendAppealMessage", req);

export const setAppealChatRead = (chatId: number) =>
  connection?.invoke("SetAppealChatRead", chatId);

export const startVeto = (lobbyId: number) =>
  connection?.invoke("StartVeto", lobbyId);

const createUse = (onAction: (value: any) => Promise<any>) => {
  return (): [(value: any) => Promise<any>, { condition: string }] => {
    const [condition, setCondition] = useState<string>("idle");
    const onUpdate = (value: any) => {
      setCondition("loading");
      let result = onAction(value);
      if (result != null)
        return result
          .then(() => setCondition("idle"))
          .catch(() => setCondition("Проблемы с соединением"));
      return result;
    };
    return [onUpdate, { condition }];
  };
};

export const useGetMenuLinks = (linksTemplate: IMenuLink[]) => {
  let id = 0;
  const [links, setLinks] = useState<IMenuLink[]>(linksTemplate);
  try {
    id = parseInt(getUserId());
  } catch {
    console.error("Invalid id");
  }

  const { data: status, isFetching, refetch } = useGetUserStatusQuery({ id });
  const handleLinks = (
    template: IMenuLink[],
    usrStatus: UserStatus | undefined
  ) => {
    if (template.find((item) => item.link.includes("create-lobby")))
      return template;
    return usrStatus.active
      ? [
          {
            text: "Лобби",
            link: `create-lobby/${usrStatus.lobbyId}`,
          },
          ...template,
        ]
      : template;
  };
  const handleRefetchStatus = () => {
    refetch()
      .unwrap()
      .then((s) => setLinks((prev) => handleLinks(prev, s)));
  };
  useEffect(() => {
    handleRefetchStatus();

    const timerId = setInterval(handleRefetchStatus, 1000);
    return () => {
      clearInterval(timerId);
    };
  }, [refetch]);
  return links;
};

export const useUpdateBid = createUse(updateBid);
export const useUpdateTeamName = createUse(updateTeamName);
export const useUpdateConfig = createUse(updateConfig);

export const useInviteUser = createUse(inviteUser);

export const useSendMessage = createUse(sendMessage);
export const useSendAppealMessage = createUse(sendAppealMessage);
export const useSetAppealChatRead = createUse(setAppealChatRead);

export const useSelectMap = createUse(pickMap);

export const useStartVeto = createUse(startVeto);

interface DecodedToken {
  [key: string]: any;
}
export function getValueFromJWT(key: string): string {
  const cookieString = document.cookie;
  function getCookieValue(cookieString: string, name: string): string {
    const value = `; ${cookieString}`;
    const parts = value.split(`; ${name}=`);
    if (parts.length === 2) return parts.pop()?.split(";").shift() || "";
    return "-1";
  }

  // Извлекаем JWT токен из куки
  let token = getCookieValue(cookieString, "app_identity");
  if (!token) {
    console.error("JWT токен не найден в куки.");
    return "-1";
  }

  try {
    // Декодируем токен
    const decoded: DecodedToken = jwtDecode(token);

    // Возвращаем значение нужного ключа из payload
    return decoded[key] || "-1";
  } catch (error) {
    console.error("Ошибка при декодировании JWT токена:", error);
    return "-1";
  }
}

export const isNeedAvatar = ({
  messages,
  index,
}: {
  messages: GetChatMessageDto[];
  index: number;
}): boolean => {
  const prevMessage = messages[index - 1] ? messages[index - 1] : null;

  if (prevMessage && prevMessage.user.isAdmin == messages[index].user.isAdmin)
    return false;
  return true;
};

const createGetter = (prop: string) => () => getValueFromJWT(prop);
export function truncateString(text: string, maxLength: number): string {
  if (text.length > maxLength) {
    return text.substring(0, maxLength - 3) + "...";
  }
  return text;
}

export function formatDate(date: Date): string {
  const day = String(date.getDate()).padStart(2, "0");
  const month = String(date.getMonth() + 1).padStart(2, "0"); // Месяцы начинаются с 0, поэтому добавляем 1
  const year = date.getFullYear();

  return `${day}.${month}.${year}`;
}

export const getUserId = createGetter("Id");
export function timeAgo(date: Date): string {
  const now = new Date();
  const diff = now.getTime() - date.getTime();

  const seconds = Math.floor(diff / 1000);
  const minutes = Math.floor(seconds / 60);
  const hours = Math.floor(minutes / 60);
  const days = Math.floor(hours / 24);
  const weeks = Math.floor(days / 7);
  const months = Math.floor(days / 30);
  const years = Math.floor(days / 365);

  if (years > 0) {
    return `Due ${years} year${years > 1 ? "s" : ""} ago`;
  } else if (months > 0) {
    return `Due ${months} month${months > 1 ? "s" : ""} ago`;
  } else if (weeks > 0) {
    return `Due ${weeks} week${weeks > 1 ? "s" : ""} ago`;
  } else if (days > 0) {
    return `Due ${days} day${days > 1 ? "s" : ""} ago`;
  } else if (hours > 0) {
    return `Due ${hours} hour${hours > 1 ? "s" : ""} ago`;
  } else if (minutes > 0) {
    return `Due ${minutes} minute${minutes > 1 ? "s" : ""} ago`;
  } else {
    return `Due ${seconds} second${seconds > 1 ? "s" : ""} ago`;
  }
}

export function formatNumberWithSpaces(value: number): string {
  return new Intl.NumberFormat("ru-RU", {
    useGrouping: true,
  }).format(value);
}

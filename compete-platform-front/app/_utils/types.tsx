import { MatchResult } from "../_components/Stat/Stat";

export type ProblemDetails = {
  title?: string;
  errors?: [{ [key: string]: string }];
  detail?: string;
};

export enum NotificationType {
  Error,
  Info,
}
export interface UserInfographicStats {
  replenishedSumm: number;
  outcomeFromBalance: number;
  earned: number;
  earnedPercent: number;
  outcomeFromBalancePercent: number;
  replenishedSummPercent: number;
}
export interface UserInfographicStats {
  replenishedSumm: number;
  outcomeFromBalance: number;
  earned: number;
}
export interface UserSuperficialStats {
  balance: number;
  incomeByMonth: number;
  outcomeFromBalance: number;
}
export interface GetPayDto {
  id: number;
  userId: number;
  amount: number;
  description: string;
  creationTime: string;
}
export interface GetUserRequest {
  userId?: number;
  includeFriends?: boolean;
}
export interface JoinToLobbyInfo {
  userId: number;
  inviterId?: number | null;
  teamId?: number | null;
  lobbyId: number;
  code?: string | null;
}
export interface Notification {
  type: NotificationType;
  message: string;
}
export interface UserStatus {
  lobbyId: number;
  active: boolean;
}
export interface GetTeamDto {
  id: number;
  creatorId: number;
  lobby: GetLobbyDto;
  name: string;
  users: GetUserDto[];
  chatId: number;
}

export interface GetMatchDto {
  id: number;
  teamId: number | null;
  playedMap: Map;
  firstTeamScore: number;
  secondTeamScore: number;
}
export interface LobbyAdminConfiguration {
  id: number;
  pickMaps: Map[];
  public: boolean;
  serverId: number;
  config: ServerConfig;
  playersAmount: Type;
  matchFormat: Format;
}

export interface GetUserBidDto {
  id: number;
  bid: number;
  userId: number;
}
export enum Map {
  Mirage,
  Inferno,
  Nuke,
  Anubis,
  Overpass,
  Vertigo,
  Ancient,
  Dust2,
  Office,
  Italy,
}
export interface GetServerDto {
  id: number;
  location: string;
  path: string;
  isHealthy: boolean;
}

export interface CancelLobbyRequest {
  id: number;
  offenders: number[] | null;
}
export interface ServerConfig {
  friendlyFire: boolean;
  freezeTime: number;
}

export enum Format {
  BO1 = 1,
  BO3 = 2,
  BO5 = 3,
}
export interface CreateReportDto {
  subject: string;
  content: string;
  userId: number;
  lobbyId: number;
  status: string;
  response: string;
  createdAt: string;
}
export interface GetBatchOfPagedEntitiesRequest {
  page: number;
  pageSize: number;
  orderProperty: string;
  order: string;
  searchParam: string | null;
}
export interface GetUserReportRequest {
  page: number;
  pageSize: number;
  userId: number;
}
export interface GetReportDto {
  id: number;
  subject: string;
  content: string;
  userId: number;
  user: GetUserDto | null;
  lobbyId: number;
  status: string;
  response: string;
  createdAt: string;
}
export interface GetMatchesForAdminRequest
  extends GetBatchOfPagedEntitiesRequest {
  findBy: string | null;
}

export enum Type {
  v1 = 1,
  v2 = 2,
  v3 = 3,
  v4 = 4,
  v5 = 5,
}

export enum LobbyStatus {
  Configuring,
  Veto,
  Playing,
  Canceled,
  Over,
  Warmup,
}
export interface SendMessageRequest {
  chatId: number;
  message: string;
}

export interface GetUserRateDto {
  id: number;
  steamId: string;
  name: string;
  avatarUrl: string;
  headshotPercent: number;
  income: number;
  killsByDeaths: number;
  ratePlace: number | null;
  winrate: number;
  rate: number;
  matches: number;
  isOnline: boolean;
  lastResults: string[];
}
export interface GetUserAwardDto {
  id: number;
  userId: number;
  award: number;
}

export interface GetServerPingDto {
  ip: string;
  pingTime: number;
  status: string;
  errorMessage: string;
}

export interface GetLobbyDto {
  id: number;
  creatorId: number;
  pickMaps: Map[];
  public: boolean;
  serverId: number;
  server: GetServerDto;
  matches: GetMatchDto[];
  chatId: number;
  awards: GetUserAwardDto[];
  teamWinner: number | null;
  bids: GetUserBidDto[];
  teams: GetTeamDto[];
  config: ServerConfig;
  mapActions: MapActionInfo[];
  status: LobbyStatus;
  playersAmount: Type;
  codeToConnect: string;
  matchFormat: Format;
  port: number | null;
}
export enum ConfirmationType {
  Redirect,
  External,
  Embedded,
  QR,
  MobileApplication,
}
export interface Confirmation {
  type: ConfirmationType;
  returnUrl: string;
  confirmationUrl: string;
  confirmationData: string;
  enforce: boolean | null;
  locale: string;
  confirmationToken: string;
}

export interface PayRequestDto {
  amount: number;
  userId: string | null;
  identifier?: string;
  variant: string;
}
export interface PayResponseDto {
  confirmation: Confirmation;
  text: string | null;
}
export interface PayoutRequest {
  identifier: string;
  amount: number;
}
export interface GetPayDto {
  id: number;
  userId: number;
  amount: number;
  description: string;
  creationTime: string;
}
export interface MapPickRequest {
  map: Map;
  lobbyId: number;
}

export interface GetServerDto {
  id: number;
  location: string;
  path: string;
}
export interface ActionInfo {
  newLobby: GetLobbyDto;
  isPickNow: boolean;
  nextPickUserId: number;
  pickingComplete: boolean;
}

export interface GetChatMessageDto {
  id: number;
  content: string;
  chatId: number;
  user: GetUserDto;
  sendTime: string;
}

export interface GetAppealChatDto {
  id: number;
  userIds: number[];
  isRead: boolean;
}

export interface MapActionInfo {
  teamId: number;
  isPicked: boolean;
  map: Map;
  actionTime: string;
}
export interface GetUserDto {
  id: number;
  name: string;
  steamId: string;
  balance: number;
  registrationDate: string;
  avatarUrl: string;
  status: string;
  friends: GetUserDto[];
  lastResults: MatchResult[];
  isOnline: boolean;
  headshotPercent: number;
  winrate: number;
  killsByDeaths: number;
  matches: number;
  canInvite: boolean;
  income: number;
  ratePlace: number | null;
  inLobby: boolean;
  isBanned: boolean;
  isAdmin: boolean;
}

export interface GetLobbyViewDto {
  id: number;
  creator: GetUserDto;
  bankSumm: number;
  server: GetServerDto;
  capacity: number;
  playersAmount: Type;
  matchFormat: Format;
  currentMap: Map | null;
}

export interface GetLobbyStatus {
  status: LobbyStatus;
}

export interface GetLobbyRequest {
  page: number;
  pageSize: number;

  status?: string;
  type?: string;
  mode?: string;
  maps?: string[];
  nickName?: string;
}
export interface GetUserView {
  id: number;
  steamId: string;
  balance: number;
  profit: number;
  name: string;
  isOnline: boolean;
  avatarUrl: string;
  friends: GetUserView[];
  ratePlace: number | null;
  winrate: number;
  rate: number;
  matches: number;
  lastResults: string[];
  isBanned: boolean;
  currentLobby: number | null;
}
export interface GetBannersDto {
  banners: string[];
}
export interface GetAdminStatsResponse {
  usersCount: number;
  serversCount: number;
  financialRotation: number;
  matchesCount: number;
  lobbyComissions: number;
}
export interface GetAdminStatsRequest {
  userInterval: string | null;
  isOnlineUsers: boolean | null;
  onlyHealthyServers: boolean | null;
  financialRotationInterval: string;
  matchesStatus: string;
  lobbyComissionsInterval: string;
}
export interface GetStatsRequest {
  shouldBe: boolean | null;
  interval: string | null;
}
export interface PlatformEvent {
  id: number;
  payload: string;
  ocurredOnUtc: string;
}
export interface GetPayEvent {
  id: number;
  userId: number;
  eventType: string;
  createdUtc: string;
  status: string;
  amount: number;
}
export interface GetSupportCover {
  rules: string;
  faq: string;
}
export interface GetContacts {
  telegram: string;
  youtube: string;
  discord: string;
  telegramForAd: string;
}
export interface GetUsersForAdminRequest {
  page: number;
  pageSize: number;
  orderProperty: string;
  order: string;
  searchParam?: string | null;
}

export interface GetMatchView {
  id: number;
  server: GetServerDto;
  status: LobbyStatus;
  port: number | null;
  playersAmount: Type;
  mapActions: any;
}

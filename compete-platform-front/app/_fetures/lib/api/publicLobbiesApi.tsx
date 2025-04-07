import { Stats } from "@/app/_components/BalancePlot/BalancePlot";
import {
  configureConnectionForLobbyEvents,
  configureConnectionForMessages,
  disposeLobbyEvents,
  disposeTeamMessages,
  distinctBy,
} from "@/app/_utils/functions";
import {
  ActionInfo,
  CancelLobbyRequest,
  CreateReportDto,
  GetAdminStatsRequest,
  GetAdminStatsResponse,
  GetAppealChatDto,
  GetBannersDto,
  GetBatchOfPagedEntitiesRequest,
  GetChatMessageDto,
  GetContacts,
  GetLobbyRequest,
  GetLobbyStatus,
  GetLobbyViewDto,
  GetMatchesForAdminRequest,
  GetMatchView,
  GetPayDto,
  GetPayEvent,
  GetReportDto,
  GetServerDto,
  GetServerPingDto,
  GetStatsRequest,
  GetSupportCover,
  GetUserDto,
  GetUserRateDto,
  GetUserReportRequest,
  GetUserRequest,
  GetUsersForAdminRequest,
  GetUserView,
  JoinToLobbyInfo,
  PayoutRequest,
  PayRequestDto,
  PayResponseDto,
  PlatformEvent,
  Type,
  UserInfographicStats,
  UserStatus,
  UserSuperficialStats,
} from "@/app/_utils/types";
import { EntityState, createEntityAdapter } from "@reduxjs/toolkit";
import { createApi, fetchBaseQuery } from "@reduxjs/toolkit/query/react";

export const messagesAdapter = createEntityAdapter<GetChatMessageDto>();

export const appealChatsAdapter = createEntityAdapter<GetAppealChatDto>();

export const lobbiesAdapter = createEntityAdapter<GetLobbyViewDto>();

export const paysAdapter = createEntityAdapter<GetPayDto>();

export const payEventsAdapter = createEntityAdapter<GetPayEvent>();

export const platformEventsAdapter = createEntityAdapter<PlatformEvent>();

export const usersForAdminAdapter = createEntityAdapter<GetUserView>();

export const ratingAdapter = createEntityAdapter<GetUserRateDto>();

export const matchesAdapter = createEntityAdapter<GetMatchView>();

export const reportsAdapter = createEntityAdapter({
  selectId: (e: GetReportDto) => e.id,
});

export const { selectAll } = messagesAdapter.getSelectors();

export const Api = createApi({
  reducerPath: "publicLobbyApi",
  baseQuery: fetchBaseQuery({ baseUrl: "/api/" }),
  tagTypes: ["lobbies", "users", "chat"],
  keepUnusedDataFor: Infinity,
  endpoints: (builder) => ({
    getUserProfile: builder.query<GetUserDto, GetUserRequest>({
      query: (arg) =>
        `user/${arg.userId !== undefined ? arg.userId : ""}?includeFriends=${
          arg.includeFriends !== undefined ? arg.includeFriends : false
        }`,
      providesTags: (res, error, arg) => [
        { type: "users", id: arg.userId },
        "users",
      ],
    }),
    getAllLobbies: builder.query<GetLobbyViewDto[], void>({
      query: () => "lobby/all",
      providesTags: ["lobbies"],
    }),
    getLobbies: builder.query<
      EntityState<GetLobbyViewDto, number>,
      GetLobbyRequest
    >({
      query: (req) => {
        let params = new URLSearchParams({
          Page: req.page.toString(),
          PageSize: req.pageSize.toString(),
        });

        if (req.status) {
          params.append("Status", req.status);
        }
        if (req.type) {
          params.append("Type", req.type);
        }
        if (req.mode) {
          params.append("Mode", req.mode);
        }
        if (req.maps?.length) {
          params.append("Maps", req.maps.join("-"));
        }
        if (req.nickName) {
          params.append("Nickname", req.nickName);
        }

        return `lobby?${params.toString()}`;
      },
      providesTags: ["lobbies"],
      serializeQueryArgs: ({ endpointName, queryArgs }) =>
        `${endpointName}-${JSON.stringify(queryArgs)}`,
      transformResponse: (res: GetLobbyViewDto[]) => {
        return lobbiesAdapter.addMany(lobbiesAdapter.getInitialState(), res);
      },
      forceRefetch: ({ currentArg, previousArg }) =>
        currentArg?.page !== previousArg?.page ||
        currentArg?.pageSize !== previousArg?.pageSize ||
        currentArg?.status !== previousArg?.status ||
        currentArg?.type !== previousArg?.type ||
        currentArg?.mode !== previousArg?.mode ||
        currentArg?.maps !== previousArg?.maps ||
        currentArg?.nickName !== previousArg?.nickName,
      merge: (currentState, incomingState) => {
        lobbiesAdapter.addMany(
          currentState,
          lobbiesAdapter.getSelectors().selectAll(incomingState)
        );
      },
    }),
    getServers: builder.query<GetServerDto[], void>({
      query: () => "lobby/servers",
    }),
    getUsersForAdmin: builder.query<
      EntityState<GetUserView, number>,
      GetUsersForAdminRequest
    >({
      query: (req) => ({
        url: `admin/users?page=${req.page}&pageSize=${req.pageSize}&order=${
          req.order
        }&orderProperty=${req.orderProperty}${
          req.searchParam ? `&searchParam=${req.searchParam}` : ""
        }`,
      }),
      transformResponse: (res: GetUserView[]) => {
        return usersForAdminAdapter.addMany(
          usersForAdminAdapter.getInitialState(),
          res
        );
      },
      keepUnusedDataFor: 1,
      serializeQueryArgs: ({ queryArgs, endpointName }) =>
        endpointName +
        queryArgs.pageSize +
        queryArgs.order +
        queryArgs.orderProperty +
        queryArgs.searchParam,
      forceRefetch: ({ currentArg, previousArg }) => {
        return (
          currentArg?.page !== previousArg?.page ||
          currentArg?.pageSize !== previousArg?.pageSize ||
          currentArg?.order !== previousArg?.order ||
          currentArg?.orderProperty !== previousArg?.orderProperty ||
          currentArg?.searchParam !== previousArg.searchParam
        );
      },
      merge: (currentState, incomingState) => {
        usersForAdminAdapter.addMany(
          currentState,
          usersForAdminAdapter.getSelectors().selectAll(incomingState)
        );
      },
    }),
    createLobby: builder.mutation<void, void>({
      query: (arg) => ({ url: "lobby/", method: "POST" }),
    }),
    getCommonChatId: builder.query<string, void>({
      query: () => ({ url: "chat", responseHandler: (res) => res.text() }),
    }),
    joinToLobby: builder.mutation<void, JoinToLobbyInfo>({
      query: (arg) => ({
        url:
          `lobby/${arg.lobbyId}/users/${arg.userId}?code=${
            arg.code ?? "00000000-0000-0000-0000-000000000000"
          }` +
          (arg.teamId && arg.inviterId
            ? `&inviterId=${arg.inviterId}&teamId=${arg.teamId}`
            : ""),
        method: "PUT",
        body: arg,
      }),
    }),
    getUserStatus: builder.query<UserStatus, { id: number }>({
      query: (req) => `user/${req.id}/status`,
    }),
    leaveFromLobby: builder.mutation<void, { userId: number; lobbyId: number }>(
      {
        query: (arg) => ({
          url: `lobby/${arg.lobbyId}/users/${arg.userId}`,
          method: "DELETE",
        }),
        invalidatesTags: ["lobbies"],
      }
    ),
    getServerPing: builder.query<GetServerPingDto, { ip: string }>({
      query: ({ ip }) => `lobby/ping?ip=${ip}`,
    }),
    getLobbyStatus: builder.query<GetLobbyStatus, { id: number }>({
      query: ({ id }) => `lobby/status?id=${id}`,
    }),
    getLobby: builder.query<ActionInfo, { id: number | undefined }>({
      query: (req) => `lobby/single/${req.id ? req.id : ""}`,
      providesTags: (res) => [{ type: "lobbies", id: res?.newLobby.id }],
      serializeQueryArgs: ({ queryArgs }) => `lobbyConfig${queryArgs.id}`,
      onCacheEntryAdded: async (
        arg,
        { updateCachedData, cacheDataLoaded, cacheEntryRemoved }
      ) => {
        try {
          await cacheDataLoaded;
          const newLobbyHandler = (newLobby: ActionInfo) => {
            updateCachedData((draft) => {
              Object.assign(draft, newLobby);
            });
          };
          configureConnectionForLobbyEvents(newLobbyHandler);
        } catch {}

        await cacheEntryRemoved;
        disposeLobbyEvents();
      },
    }),
    checkAuth: builder.mutation<void, void>({
      query: () => "auth/check",
    }),
    getUserPays: builder.query<
      EntityState<GetPayDto, number>,
      { id: number; page: number; pageSize: number }
    >({
      query: ({ id, page, pageSize }) =>
        `user/${id}/pays?page=${page}&pageSize=${pageSize}`,
      serializeQueryArgs: ({ endpointName, queryArgs }) =>
        `${endpointName}-${queryArgs.id}-${queryArgs.pageSize}`,
      transformResponse: (res: GetPayDto[]) => {
        return paysAdapter.addMany(paysAdapter.getInitialState(), res);
      },
      forceRefetch: ({ currentArg, previousArg }) =>
        currentArg?.page !== previousArg?.page ||
        currentArg?.pageSize !== previousArg?.pageSize ||
        currentArg?.id !== previousArg?.id,
      merge: (currentState, incomingState) => {
        paysAdapter.addMany(
          currentState,
          paysAdapter.getSelectors().selectAll(incomingState)
        );
      },
    }),
    getUserStats: builder.query<Stats, number>({
      query: (id) => `user/${id}/stats`,
    }),
    getInfographicUserStats: builder.query<UserInfographicStats, number>({
      query: (id) => `user/${id}/infographic-stats`,
    }),
    getSuperficialStats: builder.query<UserSuperficialStats, number>({
      query: (id) => `user/${id}/superficial-stats`,
    }),
    getRatingUsers: builder.query<
      EntityState<GetUserRateDto, number>,
      { page: number; pageSize: number }
    >({
      query: ({ page, pageSize }) =>
        `user/rating?page=${page}&pageSize=${pageSize}`,
      transformResponse: (res: GetUserRateDto[]) => {
        return ratingAdapter.addMany(ratingAdapter.getInitialState(), res);
      },
      serializeQueryArgs: ({ endpointName, queryArgs }) =>
        endpointName + queryArgs.pageSize,
      forceRefetch: ({ currentArg, previousArg }) => {
        return currentArg?.page !== previousArg?.page;
      },
      merge: (currentState, incomingState) => {
        ratingAdapter.addMany(
          currentState,
          ratingAdapter.getSelectors().selectAll(incomingState)
        );
      },
    }),
    getLobbyCounts: builder.query<number, Type>({
      query: (amount) => `lobby/count?playersAmount=${amount}`,
      serializeQueryArgs: ({ endpointName, queryArgs }) =>
        endpointName + queryArgs,
    }),
    doPay: builder.mutation<PayResponseDto, PayRequestDto>({
      query: (req) => ({
        url: `user/pay`,
        method: "POST",
        body: req,
      }),
    }),
    doPayout: builder.mutation<string, PayoutRequest>({
      query: (req) => ({
        url: `user/payout`,
        body: req,
        method: "POST",
        responseHandler: (res) => res.text(),
      }),
    }),
    getAppealChats: builder.query<
      EntityState<GetAppealChatDto, number>,
      { page: number; pageSize: number; searchParam: string }
    >({
      query: (arg) =>
        `admin/appeals?page=${arg.page}&pageSize=${arg.pageSize}${
          arg.searchParam ? `&searchParam=${arg.searchParam}` : ""
        }`,
      transformResponse: (res: GetAppealChatDto[]) => {
        return appealChatsAdapter.addMany(
          appealChatsAdapter.getInitialState(),
          res
        );
      },
      keepUnusedDataFor: 1,
      serializeQueryArgs: ({ endpointName, queryArgs }) =>
        `${endpointName}-${queryArgs.page}-${queryArgs.pageSize}-${queryArgs.searchParam}`,
      forceRefetch: ({ currentArg, previousArg }) => {
        return (
          currentArg?.page !== previousArg?.page ||
          currentArg?.pageSize !== previousArg?.pageSize ||
          currentArg?.searchParam !== previousArg?.searchParam
        );
      },
      merge: (currentState, incomingState) => {
        appealChatsAdapter.addMany(
          currentState,
          appealChatsAdapter.getSelectors().selectAll(incomingState)
        );
      },
      providesTags: ["chat"],
    }),
    getChatMessages: builder.query<
      EntityState<GetChatMessageDto, number>,
      { page: number; chatId: number }
    >({
      query: (arg) => `chat/${arg.chatId}?page=${arg.page}&pageSize=45`,
      serializeQueryArgs: ({ endpointName, queryArgs }) =>
        endpointName + queryArgs.chatId,
      transformResponse: (res: GetChatMessageDto[]) =>
        messagesAdapter.addMany(messagesAdapter.getInitialState(), res),
      merge: (currentState, incomingState) => {
        messagesAdapter.addMany(currentState, selectAll(incomingState));
      },
      providesTags: ["chat"],
      forceRefetch: ({ currentArg, previousArg }) => {
        return (
          currentArg?.page !== previousArg?.page ||
          currentArg?.chatId !== previousArg?.chatId
        );
      },
      onCacheEntryAdded: async (
        arg,
        { updateCachedData, cacheEntryRemoved, cacheDataLoaded }
      ) => {
        try {
          await cacheDataLoaded;

          const onNewMessage = (msg: GetChatMessageDto) => {
            updateCachedData((last) => {
              messagesAdapter.upsertOne(last, msg);
            });
          };

          configureConnectionForMessages(onNewMessage);
        } catch {}

        await cacheEntryRemoved;
        disposeTeamMessages();
      },
    }),
    getAppealChatIdByUserId: builder.query<number, number>({
      query: (userId) => `chat/appealchat/${userId}`,
    }),
    getAppealChatMessages: builder.query<
      EntityState<GetChatMessageDto, number>,
      { page: number; appealChatId: number }
    >({
      query: (arg) =>
        `chat/appeals/${arg.appealChatId}?page=${arg.page}&pageSize=45`,
      serializeQueryArgs: ({ endpointName, queryArgs }) =>
        endpointName + queryArgs.appealChatId,
      transformResponse: (res: GetChatMessageDto[]) =>
        messagesAdapter.addMany(messagesAdapter.getInitialState(), res),
      merge: (currentState, incomingState) => {
        messagesAdapter.addMany(currentState, selectAll(incomingState));
      },
      providesTags: ["chat"],
      forceRefetch: ({ currentArg, previousArg }) => {
        return (
          currentArg?.page !== previousArg?.page ||
          currentArg?.appealChatId !== previousArg?.appealChatId
        );
      },
      onCacheEntryAdded: async (
        arg,
        { updateCachedData, cacheEntryRemoved, cacheDataLoaded }
      ) => {
        try {
          await cacheDataLoaded;

          const onNewMessage = (msg: GetChatMessageDto) => {
            updateCachedData((last) => {
              messagesAdapter.upsertOne(last, msg);
            });
          };

          configureConnectionForMessages(onNewMessage);
        } catch {}

        await cacheEntryRemoved;
        disposeTeamMessages();
      },
    }),
    getMatchesForAdmin: builder.query<
      EntityState<GetMatchView, number>,
      GetMatchesForAdminRequest
    >({
      query: (req) => ({
        url: `admin/matches?page=${req.page}&pageSize=${req.pageSize}&order=${
          req.order
        }&orderProperty=${req.orderProperty}${
          req.searchParam ? `&searchParam=${req.searchParam}` : ""
        }${req.findBy ? `&findBy=${req.findBy}` : ""}`,
      }),
      transformResponse: (res: GetMatchView[]) => {
        return matchesAdapter.addMany(matchesAdapter.getInitialState(), res);
      },
      keepUnusedDataFor: 1,
      serializeQueryArgs: ({ endpointName, queryArgs }) =>
        queryArgs.pageSize +
        queryArgs.order +
        queryArgs.orderProperty +
        endpointName +
        queryArgs.searchParam,
      forceRefetch: ({ currentArg, previousArg }) => {
        return (
          currentArg?.page !== previousArg?.page ||
          currentArg?.pageSize !== previousArg?.pageSize ||
          currentArg?.order !== previousArg?.order ||
          currentArg?.orderProperty !== previousArg?.orderProperty ||
          currentArg?.searchParam !== previousArg?.searchParam
        );
      },
      merge: (currentState, incomingState) => {
        matchesAdapter.addMany(
          currentState,
          matchesAdapter.getSelectors().selectAll(incomingState)
        );
      },
    }),
    getSupportCover: builder.query<GetSupportCover, void>({
      query: () => `admin/supportCover`,
    }),
    updateSupportCover: builder.mutation<void, GetSupportCover>({
      query: (req) => ({
        url: "admin/supportCover",
        body: req,
        method: "POST",
      }),
    }),
    getContacts: builder.query<GetContacts, void>({
      query: () => `admin/contacts`,
    }),
    updateContacts: builder.mutation<void, GetContacts>({
      query: (req) => ({ url: "admin/contacts", body: req, method: "POST" }),
    }),
    getBanners: builder.query<GetBannersDto, void>({
      query: () => `admin/banners`,
    }),
    updateBanners: builder.mutation<void, FormData>({
      query: (req) => ({
        url: "admin/banners",
        body: req,
        method: "POST",
      }),
    }),

    getPayEvents: builder.query<
      EntityState<GetPayEvent, number>,
      GetUsersForAdminRequest
    >({
      query: (req) =>
        `admin/payEvents?page=${req.page}&pageSize=${req.pageSize}&order=${req.order}&orderProperty=${req.orderProperty}`,
      transformResponse: (res: GetPayEvent[]) => {
        return payEventsAdapter.addMany(
          payEventsAdapter.getInitialState(),
          res
        );
      },
      keepUnusedDataFor: 1,
      serializeQueryArgs: ({ endpointName, queryArgs }) =>
        queryArgs.pageSize +
        queryArgs.order +
        queryArgs.orderProperty +
        endpointName +
        queryArgs.searchParam,
      forceRefetch: ({ currentArg, previousArg }) => {
        return (
          currentArg?.page !== previousArg?.page ||
          currentArg?.pageSize !== previousArg?.pageSize ||
          currentArg?.order !== previousArg?.order ||
          currentArg?.orderProperty !== previousArg?.orderProperty ||
          currentArg?.searchParam !== previousArg?.searchParam
        );
      },
      merge: (currentState, incomingState) => {
        payEventsAdapter.addMany(
          currentState,
          payEventsAdapter.getSelectors().selectAll(incomingState)
        );
      },
    }),
    getPlatformEvents: builder.query<
      EntityState<PlatformEvent, number>,
      GetUsersForAdminRequest
    >({
      query: (req) =>
        `admin/events?page=${req.page}&pageSize=${req.pageSize}&order=${req.order}&orderProperty=${req.orderProperty}`,
      transformResponse: (res: PlatformEvent[]) => {
        return platformEventsAdapter.addMany(
          platformEventsAdapter.getInitialState(),
          distinctBy(res, (e) => e.payload)
        );
      },
      keepUnusedDataFor: 1,
      serializeQueryArgs: ({ endpointName, queryArgs }) =>
        queryArgs.pageSize +
        queryArgs.order +
        queryArgs.orderProperty +
        endpointName +
        queryArgs.searchParam,
      forceRefetch: ({ currentArg, previousArg }) => {
        return (
          currentArg?.page !== previousArg?.page ||
          currentArg?.pageSize !== previousArg?.pageSize ||
          currentArg?.order !== previousArg?.order ||
          currentArg?.orderProperty !== previousArg?.orderProperty ||
          currentArg?.searchParam !== previousArg?.searchParam
        );
      },
      merge: (currentState, incomingState) => {
        platformEventsAdapter.addMany(
          currentState,
          platformEventsAdapter.getSelectors().selectAll(incomingState)
        );
      },
    }),
    getAdminStats: builder.query<GetAdminStatsResponse, GetAdminStatsRequest>({
      query: (req) =>
        `admin/stats?financialRotationInterval=${
          req.financialRotationInterval
        }&MatchesStatus=${req.matchesStatus}&LobbyComissionsInterval=${
          req.lobbyComissionsInterval
        }${
          req.userInterval != null ? `&userInterval=${req.userInterval}` : ""
        }${
          req.onlyHealthyServers != null
            ? `&onlyHealthyServers=${req.onlyHealthyServers}`
            : ""
        }${
          req.isOnlineUsers != null ? `&isOnlineUsers=${req.isOnlineUsers}` : ""
        }`,
    }),
    setUserBan: builder.mutation<void, { userId: number; isBanned: boolean }>({
      query: (req) => ({
        url: `admin/users/${req.userId}/ban?isBanned=${req.isBanned}`,
        method: "PUT",
      }),
    }),
    downloadDemoFile: builder.mutation<Blob, { id: number; mapname: string }>({
      query: (req) => ({
        url: `admin/demo?lobbyId=${req.id}&mapname=${req.mapname}`,
        responseHandler: (response: any) => {
          if (!response.ok) {
            return { error: response.statusText, status: response.status };
          }
          return response.blob();
        },
      }),
    }),
    getUsersInLobby: builder.query<GetUserDto[], number>({
      query: (id) => `lobby/${id}/users`,
    }),
    cancelLobby: builder.mutation<void, CancelLobbyRequest>({
      query: (req) => ({ url: `admin/lobby`, body: req, method: "PUT" }),
    }),
    createReport: builder.mutation<void, CreateReportDto>({
      query: (req) => ({
        url: `User/${req.userId}/report`,
        method: "POST",
        body: req,
      }),
    }),
    getUserReports: builder.query<
      EntityState<GetReportDto, number>,
      GetUserReportRequest
    >({
      query: (req) =>
        `user/${req.userId}/reports?page=${req.page}&pageSize=${req.pageSize}`,
      transformResponse: (res: GetReportDto[]) => {
        return reportsAdapter.addMany(reportsAdapter.getInitialState(), res);
      },
      keepUnusedDataFor: 1,
      serializeQueryArgs: ({ endpointName, queryArgs }) =>
        queryArgs.pageSize + endpointName,
      forceRefetch: ({ currentArg, previousArg }) => {
        return (
          currentArg?.page !== previousArg?.page ||
          currentArg?.pageSize !== previousArg?.pageSize
        );
      },
      merge: (currentState, incomingState) => {
        reportsAdapter.addMany(
          currentState,
          reportsAdapter.getSelectors().selectAll(incomingState)
        );
      },
    }),
    getReports: builder.query<
      EntityState<GetReportDto, number>,
      GetBatchOfPagedEntitiesRequest
    >({
      query: (req) =>
        `admin/reports?page=${req.page}&pageSize=${req.pageSize}&order=${req.order}&orderProperty=${req.orderProperty}`,
      transformResponse: (res: GetReportDto[]) => {
        return reportsAdapter.addMany(reportsAdapter.getInitialState(), res);
      },
      keepUnusedDataFor: 1,
      serializeQueryArgs: ({ endpointName, queryArgs }) =>
        queryArgs.pageSize +
        queryArgs.order +
        queryArgs.orderProperty +
        endpointName +
        queryArgs.searchParam,
      forceRefetch: ({ currentArg, previousArg }) => {
        return (
          currentArg?.page !== previousArg?.page ||
          currentArg?.pageSize !== previousArg?.pageSize ||
          currentArg?.order !== previousArg?.order ||
          currentArg?.orderProperty !== previousArg?.orderProperty ||
          currentArg?.searchParam !== previousArg?.searchParam
        );
      },
      merge: (currentState, incomingState) => {
        reportsAdapter.addMany(
          currentState,
          reportsAdapter.getSelectors().selectAll(incomingState)
        );
      },
    }),
    closeReport: builder.mutation<void, { id: number; reportResponse: string }>(
      {
        query: (req) => ({
          url: `admin/reports/${req.id}?reportResponse=${req.reportResponse}`,
          method: "PUT",
        }),
      }
    ),
  }),
});

export const {
  useGetAppealChatMessagesQuery,
  useGetAppealChatIdByUserIdQuery,
  useGetAppealChatsQuery,
  useGetAllLobbiesQuery,
  useGetLobbyStatusQuery,
  useGetLobbiesQuery,
  useGetServersQuery,
  useCreateLobbyMutation,
  useJoinToLobbyMutation,
  useGetUserReportsQuery,
  useCheckAuthMutation,
  useDoPayoutMutation,
  useGetBannersQuery,
  useGetContactsQuery,
  useGetSupportCoverQuery,
  useGetUserProfileQuery,
  useLeaveFromLobbyMutation,
  useGetServerPingQuery,
  useGetLobbyQuery,
  useGetUserStatusQuery,
  useGetInfographicUserStatsQuery,
  useGetSuperficialStatsQuery,
  useGetChatMessagesQuery,
  useGetUserStatsQuery,
  useGetRatingUsersQuery,
  useGetUserPaysQuery,
  useGetLobbyCountsQuery,
  useDoPayMutation,
  useGetUsersForAdminQuery,
  useGetMatchesForAdminQuery,
  useUpdateBannersMutation,
  useUpdateContactsMutation,
  useUpdateSupportCoverMutation,
  useGetPayEventsQuery,
  useGetPlatformEventsQuery,
  useGetAdminStatsQuery,
  useSetUserBanMutation,
  useDownloadDemoFileMutation,
  useGetUsersInLobbyQuery,
  useCancelLobbyMutation,
  useGetCommonChatIdQuery,
  useCloseReportMutation,
  useGetReportsQuery,
  useCreateReportMutation,
} = Api;

import {
  Format,
  GetLobbyDto,
  LobbyStatus,
  Type,
  Map,
  GetBatchOfPagedEntitiesRequest,
} from "./types";

export const lobbyData: GetLobbyDto = {
  id: 123,
  creatorId: 456,
  pickMaps: [Map.Inferno, Map.Mirage],
  public: true,
  serverId: 789,
  server: {
    id: 789,
    isHealthy: true,
    location: "New York",
    path: "/servers/newyork",
  },
  matches: [
    {
      id: 1,
      teamId: 101,
      playedMap: Map.Inferno,
      firstTeamScore: 16,
      secondTeamScore: 8,
    },
    {
      id: 2,
      teamId: 102,
      playedMap: Map.Mirage,
      firstTeamScore: 12,
      secondTeamScore: 16,
    },
  ],
  chatId: 1001,
  awards: [
    {
      id: 1,
      userId: 111,
      award: 5,
    },
    {
      id: 2,
      userId: 112,
      award: 3,
    },
  ],
  teamWinner: 101,
  bids: [
    {
      id: 1,
      bid: 100,
      userId: 111,
    },
    {
      id: 2,
      bid: 150,
      userId: 112,
    },
  ],
  teams: [
    {
      id: 101,
      creatorId: 456,
      lobby: null, // This will usually reference the parent lobby but can be null in some cases
      name: "Team A",
      users: [
        {
          id: 111,
          name: "PlayerOne",
          steamId: "STEAM_0:1:11111111",
          balance: 500,
          registrationDate: "2023-01-15",
          avatarUrl: "http://example.com/avatar1.png",
          status: "Active",
          friends: [],
          lastResults: ["W", "L"],
          isOnline: true,
          headshotPercent: 45,
          winrate: 60,
          killsByDeaths: 1.5,
          matches: 10,
          canInvite: true,
          income: 2000,
          ratePlace: 1,
          inLobby: true,
          isBanned: false,
          isAdmin: false,
        },
      ],
      chatId: 2001,
    },
    {
      id: 102,
      creatorId: 789,
      lobby: null,
      name: "Team B",
      users: [
        {
          id: 112,
          name: "PlayerTwo",
          steamId: "STEAM_0:1:22222222",
          balance: 300,
          registrationDate: "2023-02-20",
          avatarUrl: "http://example.com/avatar2.png",
          status: "Active",
          friends: [],
          lastResults: ["L", "W"],
          isOnline: false,
          headshotPercent: 50,
          winrate: 55,
          killsByDeaths: 1.2,
          matches: 15,
          canInvite: false,
          income: 1500,
          ratePlace: 2,
          inLobby: false,
          isBanned: false,
          isAdmin: false,
        },
      ],
      chatId: 2002,
    },
  ],
  config: {
    friendlyFire: false,
    freezeTime: 15,
  },
  mapActions: [
    {
      teamId: 101,
      isPicked: true,
      map: Map.Inferno,
      actionTime: "2024-09-05T10:00:00Z",
    },
    {
      teamId: 102,
      isPicked: false,
      map: Map.Mirage,
      actionTime: "2024-09-05T10:05:00Z",
    },
  ],
  status: LobbyStatus.Playing,
  playersAmount: Type.v5,
  codeToConnect: "XYZ123",
  matchFormat: Format.BO3,
  port: 27015,
};

export const GetBatchOfPagedEntitiesMockRequest: GetBatchOfPagedEntitiesRequest =
  {
    page: 1,
    pageSize: 30,
    order: "asc",
    orderProperty: "id",
    searchParam: "",
  };

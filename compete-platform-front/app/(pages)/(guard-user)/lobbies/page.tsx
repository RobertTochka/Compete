"use client";

import { PropsWithChildren, useMemo, useState } from "react";
import {
  lobbiesAdapter,
  useGetLobbiesQuery,
  useGetUserProfileQuery,
  useGetUserStatusQuery,
  useJoinToLobbyMutation,
} from "@/app/_fetures/lib/api/publicLobbiesApi";
import { useRouter } from "next/navigation";

import { getUserId, handleMutationError } from "@/app/_utils/functions";
import { useScrollPagination } from "@/app/_utils/hooks/useScrollPagination";
import { JoinToLobbyInfo } from "@/app/_utils/types";
import CommonChat from "@/app/_components/CommonChat/CommonChat";

import Link from "next/link";

import Icon from "@/app/_components/Icon";
import Matches from "@/app/_components/Matches/Matches";
import Checkbox from "@/app/_components/Checkbox/Checkbox";

const FilterBlock = ({
  children,
  title,
}: PropsWithChildren<{ title: string }>) => {
  return (
    <div className="bg-[#00000040] rounded-[10px] px-[17px] py-[13px]">
      <h3 className="text-white text-[16px] font-medium mb-[9px]">{title}</h3>
      <div className="flex flex-col gap-[3px]">{children}</div>
    </div>
  );
};

const maps = [
  "Mirage",
  "Inferno",
  "Anubis",
  "Overpass",
  "Ancient",
  "Vertigo",
  "Nuke",
  "Dust2",
  "Office",
  "Italy",
];
const types = ["5v5", "4v4", "3v3", "2v2", "1v1"];
const modes = ["bo1", "bo3", "bo5"];

export interface Filters {
  status: string;
  type: string;
  mode: string;
  maps: string[];
  nickName: string;
}

export default function LobbiesPage() {
  const userReq = useMemo(
    () => ({ userId: undefined, includeFriends: false }),
    []
  );
  const id = parseInt(getUserId());
  const { data: status, isFetching } = useGetUserStatusQuery({ id });
  const { data: user } = useGetUserProfileQuery(userReq);
  const [joinToLobby, {}] = useJoinToLobbyMutation();
  const router = useRouter();
  const { page, pageSize } = useScrollPagination<HTMLDivElement>(true, true);
  const [filters, setFilters] = useState<Filters>({
    status: "",
    type: "",
    mode: "",
    maps: [],
    nickName: "",
  });
  const {
    data: matches,
    isLoading: lobbyLoading,
    error,
  } = useGetLobbiesQuery(
    {
      page,
      pageSize,
      ...filters,
    },
    {
      pollingInterval: 10000,
      selectFromResult: ({ data, ...res }) => ({
        data: lobbiesAdapter
          .getSelectors()
          .selectAll(data ?? lobbiesAdapter.getInitialState()),
        ...res,
      }),
    }
  );

  const onLobbyAdd = (lobbyId: number) => {
    const lobbyJoinInfo: JoinToLobbyInfo = {
      lobbyId,
      userId: user?.id,
      code: "00000000-0000-0000-0000-000000000000",
    };
    if (user)
      joinToLobby(lobbyJoinInfo)
        .unwrap()
        .then(() => router.push(`/create-lobby/${lobbyId}`))
        .catch(handleMutationError);
  };

  const onFiltersChange = (value: string, name: string, checked: boolean) => {
    if (Array.isArray(filters[name])) {
      if (checked) {
        setFilters((pr) => ({ ...pr, [name]: [...pr[name], value] }));
        return;
      }
      setFilters((pr) => ({
        ...pr,
        [name]: pr[name].filter((item) => item !== value),
      }));
      return;
    }
    setFilters((pr) => ({ ...pr, [name]: checked ? value : "" }));
  };

  const onInputChange = (value: string) =>
    setFilters((pr) => ({ ...pr, nickName: value }));

  const onCreateLobby = () => {
    if (status.lobbyId) {
      router.push(`create-lobby/${status.lobbyId}`);
      return;
    }
    router.push("create-lobby");
  };

  return (
    <section className="flex-1 overflow-auto mt-[15px] flex gap-[75px] max-w-[1570px] mx-auto w-full px-[15px] xl:gap-[25px]">
      <div className="flex flex-col w-[240px] overflow-auto">
        <button
          className="text-[32px] font-semibold text-white w-full flex justify-center py-[10px] bg-saturateBlue rounded-[10px]"
          onClick={onCreateLobby}
        >
          Создать
        </button>
        <h2 className="text-[24px] mb-[27px] mt-[30px] font-medium">Фильтры</h2>
        <div className="flex-1 overflow-auto custom-scrollbar pr-[5px] gap-[20px] flex flex-col">
          <FilterBlock title="Статус:">
            <Checkbox
              onChange={onFiltersChange}
              isChecked={filters.status === "Configuring"}
              name="status"
              value="Configuring"
            >
              Набор игроков
            </Checkbox>
            <Checkbox
              onChange={onFiltersChange}
              isChecked={filters.status === "Playing"}
              name="status"
              value="Playing"
            >
              Идет матч
            </Checkbox>
          </FilterBlock>
          <FilterBlock title="Тип:">
            {types.map((type) => (
              <Checkbox
                key={type}
                isChecked={filters.type === type.slice(1)}
                name="type"
                value={type.slice(1)}
                onChange={onFiltersChange}
              >
                {type}
              </Checkbox>
            ))}
          </FilterBlock>
          <FilterBlock title="Режим:">
            {modes.map((mode) => (
              <Checkbox
                key={mode}
                isChecked={filters.mode === mode.toUpperCase()}
                name="mode"
                value={mode.toUpperCase()}
                onChange={onFiltersChange}
              >
                {mode}
              </Checkbox>
            ))}
          </FilterBlock>
          <FilterBlock title="Карта:">
            {maps.map((map) => (
              <Checkbox
                squared={true}
                key={map}
                isChecked={filters.maps.includes(map)}
                name="maps"
                value={map}
                onChange={onFiltersChange}
              >
                {map}
              </Checkbox>
            ))}
          </FilterBlock>
          <div className="bg-[#00000040] rounded-[10px] min-h-[130px] flex justify-center flex-col items-center">
            <h3 className="text-[#707070] text-[14px] mb-[15px]">
              Ваша реклама:
            </h3>
            <Link
              target="_blank"
              href="https://t.me/off_Hellboy"
              className="text-saturateBlue"
            >
              @off_Hellboy
            </Link>
            <Link
              target="_blank"
              href="https://t.me/unicheel"
              className="text-saturateBlue"
            >
              @unicheel
            </Link>
          </div>
          <div className="bg-[#00000040] rounded-[10px] min-h-[130px] h-[130px] flex justify-center flex-col items-center">
            <h3 className="text-[#707070] text-[14px] mb-[15px]">
              Ваша реклама:
            </h3>
            <Link
              target="_blank"
              href="https://t.me/off_Hellboy"
              className="text-saturateBlue"
            >
              @off_Hellboy
            </Link>
            <Link
              target="_blank"
              href="https://t.me/unicheel"
              className="text-saturateBlue"
            >
              @unicheel
            </Link>
          </div>
        </div>
      </div>
      <div className="flex-1 flex flex-col">
        <div className="flex justify-between">
          <div className="flex justify-between bg-[#00000040] rounded-[20px] px-[22px] py-[15px] w-max gap-[40px]">
            <div className="flex gap-[7px] items-center text-[24px]">
              <div className="w-[8px] h-[8px] bg-red-700 rounded-full" />
              408
            </div>
            <div className="flex gap-[7px] items-center text-[24px]">
              <div className="w-[8px] h-[8px] bg-orange-400 rounded-full" />
              37
            </div>
            <div className="flex gap-[7px] items-center text-[24px]">
              <div className="w-[8px] h-[8px] bg-yellow-500 rounded-full" />
              12
            </div>
            <div className="flex gap-[7px] items-center text-[24px]">
              <div className="w-[8px] h-[8px] bg-green-500 rounded-full" />
              435
            </div>
            <div className="flex gap-[7px] items-center text-[24px]">
              <div className="w-[8px] h-[8px] bg-saturateBlue rounded-full" />2
              308 042
            </div>
            <div className="flex gap-[7px] items-center text-[24px]">
              <div className="w-[8px] h-[8px] bg-purple-900 rounded-full" />
              123 042
            </div>
          </div>
          <div className="flex gap-[10px] items-center w-[257px]">
            <input
              type="text"
              className="text-[16px] border-none placeholder-gray"
              placeholder="Введите никнейм игрока..."
              value={filters.nickName}
              onChange={(e) => onInputChange(e.target.value)}
            />
            <div className="w-[24px] h-[24px]">
              <Icon defaultColor="#fff" icon="search" />
            </div>
          </div>
        </div>
        <Matches
          onLobbyAdd={onLobbyAdd}
          error={error}
          matches={matches}
          lobbyLoading={lobbyLoading}
        />
      </div>
    </section>
  );
}

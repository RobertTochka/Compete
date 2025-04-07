"use client";

import Icon from "@/app/_components/Icon";
import { Information } from "@/app/_components/Loading/Loading";
import Avatar from "@/app/_components/Platform/Avatar";
import User from "@/app/_components/Platform/User";
import Stat from "@/app/_components/Stat/Stat";
import { useGetUserProfileQuery } from "@/app/_fetures/lib/api/publicLobbiesApi";
import { formatNumber, handleError } from "@/app/_utils/functions";

import { GetUserRequest } from "@/app/_utils/types";
import Image from "next/image";
import { useParams } from "next/navigation";
import { useRouter } from "next/navigation";

import { useMemo } from "react";
import { useCopyToClipboard } from "@/app/_utils/hooks/useCopyToClipboard";
import Payment from "@/app/_components/Payment/Payment";

export default function ProfilePage() {
  const router = useRouter();
  const params = useParams();
  const userRequest: GetUserRequest = useMemo(
    () => ({
      userId:
        params.id !== undefined && params.id.length > 0
          ? parseInt(params.id[0])
          : undefined,
      includeFriends: true,
    }),
    [params]
  );

  const { data: user, isLoading: isLoadingUser, error: userError } = useGetUserProfileQuery(userRequest);
  const { data: currentUser, isLoading: isLoadingCurrentUser, error: currentUserError } = useGetUserProfileQuery({ userId: undefined, includeFriends: false });
  const isLoading = isLoadingUser || isLoadingCurrentUser;

  const profileError = handleError(userError || currentUserError);
  const offlineFriends = user?.friends
    ?.filter((u) => !u.isOnline)
    .map((_) => (
      <User {..._} key={_.id} onClick={() => router.push(`/profile/${_.id}`)} />
    ));
  const onlineFriends = user?.friends
    ?.filter((u) => u.isOnline)
    .map((_) => (
      <User {..._} key={_.id} onClick={() => router.push(`/profile/${_.id}`)} />
    ));

  const [, copyToClipboard] = useCopyToClipboard();

  return (
    <section className="pt-10 pl-[25px] pr-[25px] max-w-[1516px] mx-auto my-0 w-full">
      {profileError || isLoading ? (
        <Information
          size={60}
          loading={isLoading}
          errorMessage={profileError}
        ></Information>
      ) : (
        <>
          <div className="flex gap-[26px]">
            <div className="px-[15px] bg-transparent-black rounded-[20px] pt-[55px] pb-[35px] flex flex-col justify-between min-w-[400px]">
              <div className="flex justify-between items-center">
                <div className="flex gap-[10px] items-center">
                  <Avatar
                    width={70}
                    height={70}
                    image_url={user.avatarUrl ?? ""}
                  />
                  <div>
                    <p className="tex-[22px]">{user.name}</p>
                    <p className="text-[14px] text-gray-new">₽{user !== undefined ? formatNumber(user.balance) : "0"}</p>
                  </div>
                </div>
                <button className="rounded-[10px] border-2 border-saturateBlue min-w-[99px] flex px-[8px] py-[11px] justify-end h-max"
                  onClick={() => copyToClipboard(user.id.toString())}
                >
                  <div>
                    <p className="text-[12px]">Id</p>
                    <p className="text-[10px] text-gray-new">{user.id}</p>
                  </div>
                  <Image src="/img/copy.png" alt="Копирование" width={30} height={30} />
                </button>
              </div>

              {
                user.id === currentUser.id && (
                  <Payment>
                    {({ onPayout, onTopUp }) => (
                      <div className="flex gap-[29px]">
                        <button className="bg-saturateBlue text-[#121530] flex py-[13px] px-[14px] text-[20px] rounded-[10px] font-semibold items-center hover:bg-secondaryBlue"
                          onClick={onTopUp}
                        >
                          <Image src="/img/wallet.png" alt="Кошелек" width={24} height={24} />
                          Пополнение
                        </button>
                        <button className="bg-saturateBlue text-[#121530] flex py-[13px] px-[14px] text-[20px] rounded-[10px] font-semibold w-[154px] justify-center items-center hover:bg-secondaryBlue"
                          onClick={onPayout}
                        >
                          <Image src="/img/wallet.png" alt="Кошелек" width={24} height={24} />
                          Вывод
                        </button>
                      </div>
                    )}
                  </Payment>
                )
              }
            </div>
            <div className="grid grid-cols-4 gap-[25px] xl:gap-10 flex-1">
              <Stat label="Матчей">{user?.matches}</Stat>
              <Stat label="Винрейт">{Math.round(user?.winrate ?? 0)}%</Stat>
              <Stat label="Пополнено">{formatNumber(user?.income ?? 0)} Р</Stat>
              <Stat label="K/D">
                {formatNumber(user?.killsByDeaths ?? 0, false)}
              </Stat>
              <Stat
                label="Последние результаты"
                lastResults={user?.lastResults ?? []}
              />
              <Stat label="HS">
                {formatNumber(user?.headshotPercent ?? 0, false)}%
              </Stat>
              <Stat label="Место в рейтинге">{user?.ratePlace ?? "-"}</Stat>
              <Stat label="Общий выигрыш">{formatNumber(user?.income ?? 0)} Р</Stat>
            </div>
          </div>
          {!!onlineFriends?.length || !!offlineFriends?.length && (
            <h2 className="mb-1 mt-[55px] text-[24px]">Друзья</h2>
          )}
          {!!onlineFriends?.length && (
            <>
              <p className="text-[14px] mt-[4px] text-[#707070] font-medium">
                Online
              </p>
              <div className="grid grid-cols-3 gap-9 xxl:grid-cols-2 mt-[25px]">
                {onlineFriends}
              </div>
            </>
          )}
          {!!offlineFriends?.length && (
            <>
              <p className="text-[14px] text-[#707070] font-medium mt-[4px]">
                Offline
              </p>
              <div className="grid grid-cols-3 gap-9 xxl:grid-cols-2 max-w-[1460px] mt-[25px]">
                {offlineFriends}
              </div>
            </>
          )}
        </>
      )
      }
    </section >
  );
}

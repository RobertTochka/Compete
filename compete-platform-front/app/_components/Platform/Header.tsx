"use client";

import Image from "next/image";
import Link from "next/link";
import Avatar from "./Avatar";
import { useMemo } from "react";
import { useGetUserProfileQuery } from "@/app/_fetures/lib/api/publicLobbiesApi";
import { formatNumber } from "@/app/_utils/functions";
import { useRouter } from "next/navigation";
import HeaderMenu from "./HeaderMenu";
import Payment from "../Payment/Payment";
import Icon from "../Icon";

export default function Header() {
  const userRequest = useMemo(
    () => ({ userId: undefined, includeFriends: false }),
    []
  );
  const { data: user } = useGetUserProfileQuery(userRequest);
  const router = useRouter();
  const onExit = () => {
    router.push("/api/auth/exit");
  };
  return (
    <header className="pl-7 pr-20 pb-8 pt-[30px] flex justify-between items-center max-w-header-container mx-auto my-0 w-full">
      <a href="/profile">
        <Image src="/img/logo-new.png" alt="Логотип" width={200} height={40} />
      </a>

      <HeaderMenu />

      <div className="flex items-center gap-[20px]">
        {user?.isAdmin && (
          <Link
            href={"/admin-panel/users"}
            className="text-gray-new text-[22px] font-medium hover:text-white"
          >
            Админка
          </Link>
        )}
        <Payment>
          {({ onTopUp }) => (
            <button
              className="bg-saturateBlue rounded-[10px] w-[40px] h-[40px] text-[40px] leading-none hover:bg-secondaryBlue"
              onClick={onTopUp}
            >
              +
            </button>
          )}
        </Payment>
        <p className="text-gray-new text-[22px] font-medium">
          ₽{user !== undefined ? formatNumber(user.balance) : "0"}
        </p>
        <Link href={`/profile/`} className="flex items-center gap-[26px] group">
          <span className="text-white text-[22px] font-medium group-hover:text-white transition-all">
            {user !== undefined ? user.name : "Пользователь"}
          </span>
          <div className="border-[2px] group-hover:border-white rounded-full border-[rgba(0,0,0,0)] transition-all">
            <Avatar
              width={50}
              height={50}
              image_url={user !== undefined ? user.avatarUrl : ""}
            />
          </div>
        </Link>
        <button
          onClick={onExit}
          className="text-gray-new text-[22px] font-medium bg-[#1E1E1E] rounded-full flex items-center justify-center w-[50px] h-[50px] shrink-0"
        >
          <Icon icon="exitProfile" defaultColor="#8A8A8A" hoverColor="#fff" />
        </button>
      </div>
    </header>
  );
}

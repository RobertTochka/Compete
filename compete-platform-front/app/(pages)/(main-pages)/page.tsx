"use client";

import Image from "next/image";
import Link from "next/link";
import { useLayoutEffect } from "react";
import { useCheckAuthMutation } from "@/app/_fetures/lib/api/publicLobbiesApi";
import { getUserId } from "@/app/_utils/functions";
import { useRouter } from "next/navigation";

export default function HomePage() {
  const [checkAuth] = useCheckAuthMutation();
  const router = useRouter();

  useLayoutEffect(() => {
    const timerId = setTimeout(() => {
      checkAuth()
        .unwrap()
        .then(() => router.push(`/profile/${getUserId()}`));
    }, 1000);
    return () => {
      clearInterval(timerId);
    };
  }, []);

  return (
    <div className="relative pt-[112px]">
      <div className="relative z-10 pb-[35px]">
        <h1 className="font-bold text-[60px] max-w-[904px] [text-shadow:_1px_1px_3px_rgb(0_0_0)]">Первая платформа CS2 позволяющая монетизировать каждый кастомный матч</h1>
        <p className="text-gray-new text-[24px] max-w-[671px] mt-[31px]">Играйте с друзьями и другими пользователями в кастомных матчах на деньги, в которых победители забирают ставки побежденных.</p>
        <p className="text-gray-new text-[24px] max-w-[671px] mt-[23px]">Ограниченному количеству новых пользователей прилагается стартовый баланс на совершение первой игры.</p>
        <div className="w-max">
          <Link href="api/auth/enter" className="w-[434px] flex bg-darkBlue rounded-[20px] text-[40px] font-semibold items-center mt-[61px]">
            <Image src="/img/shooter.png" width={76} height={76} alt="Стрелок" />
            Начать играть
          </Link>
          <p className="text-[11px] text-gray-new mt-[9px] text-center">
            Нажимая на кнопку вы соглашаетесь с <Link href="/rules" className="text-saturateBlue">Правилами проекта</Link>
          </p>
        </div>
      </div>
      <Image src="/img/agent.png" alt="Агент" className="z-1 absolute top-0 right-0" width={968} height={743} />
    </div>
  );
}

"use client";

import Link from "next/link";

import Icon from "../Icon";
import { usePathname } from "next/navigation";
import { link } from "fs";

const LINKS = [
  {
    text: "Пользователи",
    link: "users",
    icon: "users",
  },
  {
    text: "Матчи",
    link: "matches",
    icon: "servers",
  },
  {
    text: "Статистика",
    link: "statistics",
    icon: "donut",
  },
  {
    text: "Настройки",
    link: "settings",
    icon: "gear",
  },
  {
    text: "Жалобы",
    link: "reports",
    icon: "reports",
  },
  {
    text: "Чаты",
    link: "appealchats",
    icon: "message",
  },
];

export default function Menu() {
  const pathname = usePathname();

  return (
    <aside className="pl-[35px] pr-[50px] border-r-2 border-gray min-w-[322px] py-12 xl:min-w-[285px] xl:px-7">
      <nav className="flex flex-col gap-10 items-start">
        {LINKS.map(({ link, icon, text }) => (
          <Link
            key={icon}
            href={`/admin-panel/${link}`}
            className={`inline-flex gap-[15px] items-center font-semibold transition-all ${
              pathname.slice(1).split("/").at(-1) === link
                ? "brightness-200 cursor-default"
                : "text-secondGray hover:brightness-200"
            }`}
          >
            <Icon defaultColor="#9B9B9B" hoverColor="#fff" icon={icon} />
            {text}
          </Link>
        ))}
      </nav>
    </aside>
  );
}

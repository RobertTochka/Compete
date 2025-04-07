"use client";

import Link from "next/link";
import { usePathname } from "next/navigation";

import Icon from "../Icon";
import AdBlock from "./AdBlock";
import { useGetMenuLinks } from "@/app/_utils/functions";
import {
  useGetBannersQuery,
  useGetContactsQuery,
} from "@/app/_fetures/lib/api/publicLobbiesApi";
import { object } from "yup";
import { Social } from "./Social";

export interface IMenuLink {
  text: string;
  link: string;
  icon?: string;
}
const linksTemplate: IMenuLink[] = [
  {
    text: "Публичные лобби",
    link: "lobbies",
    icon: "lobbies",
  },
  {
    text: "Создать лобби",
    link: "create-lobby",
    icon: "people",
  },
  {
    text: "Баланс",
    link: "balance",
    icon: "wallet",
  },
  {
    text: "Рейтинг",
    link: "rating",
    icon: "rating",
  },
  {
    text: "Профиль",
    link: "profile",
    icon: "user",
  },
  {
    text: "Поддержка",
    link: "faq",
    icon: "message",
  },
];

export default function Menu() {
  const pathname = usePathname();
  const LINKS = useGetMenuLinks(linksTemplate);
  const { data: banners } = useGetBannersQuery();
  const { data: contacts } = useGetContactsQuery();
  return (
    <aside className="pl-[35px] pr-[50px] border-r-2 border-gray min-w-[322px] pt-12 xl:min-w-[285px] xl:px-7 max-w-[320px]">
      <nav className="flex flex-col gap-10 items-start">
        {LINKS.map(({ link, icon, text }) => (
          <Link
            key={icon}
            href={`/${link}`}
            className={`inline-flex gap-[15px] items-center font-semibold transition-all ${
              pathname.slice(1) === link
                ? "brightness-200 cursor-default"
                : "text-secondGray hover:brightness-200"
            }`}
          >
            <Icon defaultColor="#9B9B9B" hoverColor="#fff" icon={icon} />
            {text}
          </Link>
        ))}
      </nav>
      <div className="flex items-center gap-[30px] mt-10 mb-8">
        {contacts &&
          Object.keys(contacts)
            .filter((s) => !s.includes("ForAd"))
            .map((s) => <Social label={s} url={contacts[s]} key={s}></Social>)}
      </div>
      <div className="flex flex-col gap-5">
        {banners?.banners.map((banner) => (
          <AdBlock key={banner} image_url={banner} />
        ))}
        {contacts &&
          Object.keys(contacts)
            .filter((s) => s.includes("ForAd"))
            .map((k) => (
              <AdBlock
                key={k}
                contact_label={k.replace("ForAd", "")}
                contact_url={contacts[k]}
              />
            ))}
      </div>
    </aside>
  );
}

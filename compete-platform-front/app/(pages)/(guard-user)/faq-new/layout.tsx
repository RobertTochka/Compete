"use client";
import Link from "next/link";
import { usePathname } from "next/navigation";
import { text } from "stream/consumers";

const links = [
  {
    text: "Создать обращение",
    link: "faq-new",
  },
  {
    text: "Мои обращения",
    link: "faq-new/my-appeals",
  },
  {
    text: "Чат с администратором",
    link: "faq-new/support",
  },
];

export default function FaqLayout({ children }: { children: React.ReactNode }) {
  const pathname = usePathname();
  return (
    <div className="max-w-[1119px] mx-auto w-full flex-1 overflow-auto flex flex-col xl:mt-[30px]">
      <h1 className="text-[32px]">Жалобы на игроков:</h1>
      <div className="flex gap-[40px] mt-[20px]">
        {links.map(({ link, text }) => (
          <Link
            key={link}
            href={`/${link}`}
            className={`text-gray-new text-[16px] font-medium pb-[6px] hover:text-white ${
              pathname.slice(1) === link
                ? "pointer-events-none border-b-2 border-saturateBlue text-white"
                : "hover:brightness-200"
            }`}
          >
            {text}
          </Link>
        ))}
      </div>
      {children}
    </div>
  );
}

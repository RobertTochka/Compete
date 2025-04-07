"use client"
import Header from "@/app/_components/HomePage/Header";
import Link from "next/link";
import { usePathname } from "next/navigation";
import { PropsWithChildren } from "react";

const LINKS = {
  "/rules": "Общие положения",
  "/rules/privacy-policy": "Политика конфиденциальности",
  "/rules/license": "Лицензионное соглашение",
  "/rules/personal-data-policy": "Политика обработки персональных данных",
}

export default function RulesLayout({ children }: PropsWithChildren) {
  const pathname = usePathname()

  return (
    <div className="max-w-[1800px] flex gap-[175px] flex-1">
      <aside className="flex flex-col justify-center shrink-0">
        <h2 className="font-semibold text-[32px]">Категории</h2>
        <nav className="flex flex-col pl-[27px] mt-[25px] gap-[10px] max-w-[330px]">
          {Object.entries(LINKS).map(([path, name]) => (
            <Link key={name} href={path} className={`${pathname === path ? "text-white pointer-events-none" : "text-gray-new"} text-[18px] hover:text-white`}>{name}</Link>
          ))}
        </nav>
      </aside>
      {children}
    </div>
  );
}
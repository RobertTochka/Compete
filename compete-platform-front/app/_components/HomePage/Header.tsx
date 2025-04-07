"use client";

import Image from "next/image";
import Link from "next/link";
import { usePathname } from "next/navigation";

const LINKS = {
  "/": "О нас",
  "/rules": "Правила проекта",
}

export default function Header() {
  const pathname = usePathname();

  return (
    <header className="text-2xl flex items-center justify-between pt-[25px]">
      <Link href="/">
        <Image src="/img/logo.png" alt="Логотип" width={300} height={60} />
      </Link>
      <nav className="flex justify-center items-center w-full gap-[100px] text-[24px]">
        {Object.entries(LINKS).map(([path, name]) => (
          <Link key={name} href={path} className={`${pathname === path ? "text-white pointer-events-none" : "text-gray-new"} hover:text-white`}>{name}</Link>
        ))}
      </nav>
      {/* <a href={"api/auth/enter"} className={clsx(`hover:text-secondaryBlue`)}>
        Log In
      </a> */}
    </header>
  );
}

"use client"
import Header from "@/app/_components/HomePage/Header";
import Link from "next/link";
import { usePathname } from "next/navigation";
import { PropsWithChildren } from "react";

export default function RulesLayout({ children }: PropsWithChildren) {
  const pathname = usePathname()

  return (
    <section className="bg-gradient-to-tl from-GRADIK-2 to-GRADIK-1 overflow-auto h-screen flex">
      <div className="mx-auto px-5 max-w-[1675px] w-full flex-1 flex flex-col">
        <Header />
        {children}
      </div>
    </section>
  );
}
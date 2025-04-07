"use client";
import Image from "next/image";
import Icon from "../Icon";
import { forwardRef } from "react";
import Link from "next/link";
import { getUserId } from "@/app/_utils/functions";
import { useRouter } from "next/navigation";
interface IHeaderProps {}
const Header = forwardRef<HTMLElement, IHeaderProps>((props, ref) => {
  const router = useRouter();
  const onExit = () => router.push("/api/auth/exit");
  return (
    <header
      className="pl-7 pr-20 py-8 flex justify-between items-center"
      ref={ref}
    >
      <a href="/admin-panel/users">
        <Image src="/img/logo-cs2.png" alt="Логотип" width={246} height={31} />
      </a>

      <div className="flex items-center gap-4">
        <Link href={`/profile/${getUserId()}`}>
          <span className="text-gray text-[22px] font-medium">
            Администратор
          </span>
        </Link>

        <button onClick={onExit}>
          <Icon icon="exit" defaultColor="rgb(84,84,84)" hoverColor="white" />
        </button>
      </div>
    </header>
  );
});
Header.displayName = "Header";
export default Header;

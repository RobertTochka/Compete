import { useGetMenuLinks } from "@/app/_utils/functions";
import Link from "next/link";
import { usePathname } from "next/navigation";

const linksTemplate = [
  {
    text: "Матчи",
    link: "lobbies",
  },
  {
    text: "Рейтинг",
    link: "rating",
  },
  {
    text: "Техподдержка",
    link: "faq-new",
  },
];

export default function HeaderMenu() {
  const LINKS = useGetMenuLinks(linksTemplate);
  const pathname = usePathname();

  return (
    <nav className="flex gap-[100px] xl:gap-[25px]">
      {LINKS.map(({ link, text }) => (
        <Link
          key={link}
          href={`/${link}`}
          className={`text-gray-new text-[22px] font-medium hover:text-white ${
            pathname.slice(1) === link
              ? "brightness-200 pointer-events-none"
              : "text-secondGray hover:brightness-200"
          }`}
        >
          {text}
        </Link>
      ))}
    </nav>
  );
}

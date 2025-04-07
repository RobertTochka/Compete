"use client";

import { Information } from "@/app/_components/Loading/Loading";
import {
  useGetContactsQuery,
  useGetSupportCoverQuery,
} from "@/app/_fetures/lib/api/publicLobbiesApi";
import { useHandleError } from "@/app/_utils/hooks/useTemplatedError";
import { useEffect, useState } from "react";

const FaqBlock = ({
  children,
  title,
}: {
  children: React.ReactNode;
  title: string;
}) => {
  return (
    <div className="h-[600px] p-10 overflow-y-auto overflow-x-hidden basis-1/2 rounded-[10px] bg-GRADIK text-left custom-scrollbar">
      <h2 className="text-gray text-[20px] mb-8">{title}</h2>
      <div className="flex flex-col gap-5 text-[14px] text-[#D2D2D2] text-justify items-start">
        {children}
      </div>
    </div>
  );
};

export default function FaqPage() {
  const {
    data: contact,
    isLoading: contactsLoading,
    error: contactsError,
  } = useGetContactsQuery();
  const {
    data: supportCover,
    isLoading: supportCoverLoading,
    error: supportCoverError,
  } = useGetSupportCoverQuery();
  const commonErrorText = useHandleError(supportCoverError || contactsError);
  if (
    (!supportCover && supportCoverLoading) ||
    commonErrorText ||
    (contactsLoading && !contact)
  )
    return (
      <Information
        loading={!supportCover && supportCoverLoading}
        errorMessage={commonErrorText}
      ></Information>
    );
  return (
    <section className="px-20 text-center xl:px-10">
      <h1 className="text-center text-[64px] font-semibold">Центр поддержки</h1>
      <p className="text-gray text-[20px] font-medium mb-5">
        Найди ответ на любой вопрос
      </p>
      <div className="mt-[65px] flex items-stretch gap-10">
        <FaqBlock title="Правила платформы">
          <div className="[&_span]:-ml-2 [&_span]:font-medium ml-2">
            <p className="whitespace-pre-wrap">{supportCover?.rules}</p>
          </div>
        </FaqBlock>
        <FaqBlock title="Ответы на частые вопросы">
          <p className="whitespace-pre-wrap">{supportCover?.faq}</p>
          <p className="font-semibold max-w-[510px] -mt-1">
            Если хотите оставить фидбек, сделать нам коммерческое предложение
            или пожаловаться на игрока
          </p>
          <a
            className="text-[#0F9FFF] text-[20px] font-medium leading-[150%] inline-block text-left hover:text-blue group -mt-1"
            href={contact?.telegramForAd}
            target="_blank"
          >
            Написать{" "}
            <span className="relative after:absolute after:left-0 after:right-0 after:bottom-px after:bg-[#0F9FFF] after:h-px group-hover:after:bg-blue">
              в Telegram {"—>"}
            </span>
          </a>
        </FaqBlock>
      </div>
    </section>
  );
}

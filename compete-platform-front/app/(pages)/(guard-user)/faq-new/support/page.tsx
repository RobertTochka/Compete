"use client";

import AppealChat from "@/app/_components/Faq/AppealChat";
import {
  useGetAppealChatIdByUserIdQuery,
  useGetUserProfileQuery,
} from "@/app/_fetures/lib/api/publicLobbiesApi";
import { useEffect, useMemo, useState } from "react";

export default function FaqPage() {
  const [hasChatRefetched, setHasChatRefetched] = useState(false);
  const userReq = useMemo(
    () => ({ userId: undefined, includeFriends: false }),
    []
  );
  const { data: user } = useGetUserProfileQuery(userReq);
  const { data: chatId, refetch: refetchChat } =
    useGetAppealChatIdByUserIdQuery(user?.id, {
      skip: !user,
    });

  useEffect(() => {
    if (user && !hasChatRefetched) {
      refetchChat();
      setHasChatRefetched(true);
    }
  }, [user, hasChatRefetched, refetchChat]);

  return (
    <div className="bg-[#191B21] rounded-[20px] w-full pt-[40px] h-full mt-[60px] flex-1 overflow-auto flex flex-col">
      <AppealChat chatId={chatId} />
    </div>
  );
}

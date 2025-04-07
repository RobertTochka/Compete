"use client";

import { useEffect, useState } from "react";
import toast, { Toaster } from "react-hot-toast";
import {
  configureConnectionForInvites,
  configureConnectionForNotifications,
  disposeNotificationsAndInvites,
  handleMutationError,
  start,
} from "../_utils/functions";
import {
  JoinToLobbyInfo,
  Notification,
  NotificationType,
} from "../_utils/types";
import { InviteModal } from "../_components/InviteModal/InviteModal";
import { useRouter } from "next/navigation";
import {
  useCheckAuthMutation,
  useJoinToLobbyMutation,
} from "../_fetures/lib/api/publicLobbiesApi";

export default function GuardDeviceWidthLayout({
  children,
}: {
  children: React.ReactNode;
}) {
  const router = useRouter();
  const [checkAuth] = useCheckAuthMutation();
  const [windowWidth, setWindowWidth] = useState(0);
  const [joinInfo, setJoinInfo] = useState<JoinToLobbyInfo | null>(null);
  const [joinToLobby, { isLoading: joinLobbyLoading }] =
    useJoinToLobbyMutation();

  useEffect(() => {
    setWindowWidth(window.innerWidth);
    window.addEventListener("resize", () => {
      setWindowWidth(window.innerWidth);
    });
    return () => disposeNotificationsAndInvites();
  }, []);

  useEffect(() => {
    checkAuth()
      .unwrap()
      .then(() => {
        start();
        configureConnectionForNotifications(onNotification);
        configureConnectionForInvites(setJoinInfo);
      })
      .catch(() => router.push("/"));
  }, []);

  const onNotification = (note: Notification) => {
    const noteFunc =
      note.type === NotificationType.Error ? toast.error : toast.success;
    noteFunc(note.message);
  };
  const onInviteAccept = (joinInfo: JoinToLobbyInfo) => {
    joinToLobby(joinInfo)
      .unwrap()
      .then(() => {
        setJoinInfo(null);
        router.push("/create-lobby");
      })
      .catch(handleMutationError);
  };

  if (windowWidth) {
    if (window.innerWidth >= 1280) {
      return (
        <>
          {joinInfo != null && (
            <InviteModal
              onSubmit={onInviteAccept}
              isLoading={joinLobbyLoading}
              onClose={() => setJoinInfo(null)}
              joinInfo={joinInfo}
            ></InviteModal>
          )}
          <Toaster
            toastOptions={{
              position: "top-center",
              style: {
                zIndex: 999,
                position: "relative",
                fontWeight: 500,
                fontSize: "14px",
                background: "rgba(255,255,255,15%)",
                color: "white",
                overflow: "hidden",
              },
            }}
          />
          {children}
        </>
      );
    } else {
      return (
        <section className="text-center w-screen h-screen fixed top-0 let-0 right-0 bottom-0 flex-middle font-medium p-5 bg-deepBlue">
          К сожалению, мы пока не поддерживаем мобильные устройства
        </section>
      );
    }
  }
}

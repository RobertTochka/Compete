"use client";

import { redirect } from "next/navigation";
import { ReactNode } from "react";

export default function GuardAdmin({ children }: { children: ReactNode }) {
  if (/* user !== "no-auth" */ true) {
    if (/* user !== "loading" */ true) {
      if (/* user.is_admin */ true) {
        return children;
      } else {
        redirect(`/profile/${/* user.id */ 1}`);
      }
    }
  } else {
    redirect("/");
  }
}

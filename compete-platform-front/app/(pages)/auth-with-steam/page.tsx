"use client";

import { useSearchParams } from "next/navigation";

import { HashLoader } from "react-spinners";

export default function AuthWithSteamPage() {
  const params = useSearchParams();

  return (
    <div className="bg-gradient-to-tl from-GRADIK-2 to-GRADIK-1 w-full min-h-screen flex-middle">
      {<HashLoader color="white" size={90} />}
    </div>
  );
}

"use client";
import Header from "@/app/_components/AdminPanel/Header";
import Menu from "@/app/_components/AdminPanel/Menu";
import { useEffect, useRef, useState } from "react";

export default function PlatformLayout({
  children,
}: {
  children: React.ReactNode;
}) {
  const headerRef = useRef(null);
  const [menuHeight, setMenuHeight] = useState(0);

  useEffect(() => {
    if (headerRef.current) {
      setMenuHeight(headerRef.current.getBoundingClientRect().height);
    }
  }, [headerRef.current]);

  const containerStyle = {
    height: `calc(100vh - ${menuHeight}px)`,
  };
  return (
    <div className="w-full max-h-screen bg-gradient-to-tl from-GRADIK-2 to-GRADIK-1 pb-10 overflow-y-hidden">
      <Header ref={headerRef} />
      <div className="flex items-start" style={containerStyle}>
        <Menu />
        <div className="grow h-full mr-5 w-full overflow-x-hidden overflow-y-hidden">
          {children}
        </div>
      </div>
    </div>
  );
}

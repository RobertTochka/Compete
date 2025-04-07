import Header from "@/app/_components/Platform/Header";

export default function PlatformLayout({
  children,
}: {
  children: React.ReactNode;
}) {
  return (
    <div className="w-full min-h-screen bg-gradient-to-tl from-GRADIK-2 to-GRADIK-1 pb-10 max-h-screen flex flex-col overflow-auto">
      <Header />
      <div className="flex flex-col items-start flex-1 overflow-auto">
        <div className="flex-1 overflow-auto w-full flex flex-col custom-scrollbar">{children}</div>
      </div>
    </div>
  );
}

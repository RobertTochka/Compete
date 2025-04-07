import { AdminStats } from "@/app/_components/AdminPanel/AdminStats";
import { PayEvents } from "@/app/_components/AdminPanel/PayEvents";
import { PlatformEvents } from "@/app/_components/AdminPanel/PlatformEvents";

export default function AdminPanelStatisticsPage() {
  return (
    <section className="p-5 pr-20 h-full overflow-y-auto custom-scrollbar">
      <div className="grid grid-cols-3 gap-5">
        <AdminStats></AdminStats>
      </div>
      <div className="grid grid-cols-2 gap-5 mt-5 items-start xl:grid-cols-1">
        <PayEvents></PayEvents>
        <PlatformEvents></PlatformEvents>
      </div>
    </section>
  );
}

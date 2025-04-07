import { Contacts } from "@/app/_components/AdminPanel/Contacts";
import { Banners } from "@/app/_components/AdminPanel/Banners";
import { SupportCover } from "@/app/_components/AdminPanel/SupportCover";

const AdminPanelSettingsPage = () => {
  return (
    <section className="p-10 pt-0 pb-7 px-2 h-full flex-middle custom-scrollbar overflow-y-auto overflow-x-hidden">
      <div className="flex flex-wrap gap-10 pt-48">
        <Banners></Banners>
        <SupportCover></SupportCover>
        <Contacts></Contacts>
      </div>
    </section>
  );
};
export default AdminPanelSettingsPage;

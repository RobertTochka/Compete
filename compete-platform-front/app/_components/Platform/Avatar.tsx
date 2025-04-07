/* eslint-disable @next/next/no-img-element */

type TAvatar = {
  width: number;
  height: number;
  status_online?: boolean | "none";
  creator?: boolean;
  add_friend?: boolean;
  canInvite?: boolean;
  image_url?: string;
};

export default function Avatar({
  width,
  height,
  status_online = "none",
  creator,
  add_friend,
  canInvite,
  image_url,
}: TAvatar) {
  return (
    <div
      className="rounded-full relative"
      style={{
        width: `${width}px`,
        minWidth: `${width}px`,
        height: `${height}px`,
      }}
    >
      {!add_friend && (
        <img
          className="block w-full h-full object-cover rounded-full"
          src={image_url || "/img/avatar.png"}
          alt="Аватарка"
        />
      )}
      {add_friend && (
        <div className="text-[36px] font-medium flex-middle transition-opacity hover:opacity-60 cursor-pointer w-full h-full bg-[#1E202F] rounded-full">
          +
        </div>
      )}
      {status_online !== "none" && (
        <div
          className={`rounded-full absolute top-0 left-0 z-10 ${
            status_online ? "bg-POSITIVCHIK" : "bg-GRAYCHIK"
          }`}
          style={{
            width: width > 48 ? "16px" : "9px",
            height: height > 48 ? "16px" : "9px",
          }}
        />
      )}
      {creator && (
        <span className="absolute bottom-0 left-0 right-0 text-[10px] font-medium bg-white/20 text-center rounded-[4px] px-1 py-0.5">
          LEADER
        </span>
      )}
      {canInvite && (
        <button className="bg-white rounded-full absolute bottom-0 right-0 h-6 w-6 flex-middle hover:opacity-80 transition-opacity">
          <img
            className="w-4"
            src="/icons/invite.svg"
            alt="Приглашение в лобби"
          />
        </button>
      )}
    </div>
  );
}

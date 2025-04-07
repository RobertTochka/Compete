interface AdBlockProps {
  image_url?: string;
  contact_url?: string;
  contact_label?: string;
}
export default function AdBlock({
  image_url,
  contact_url,
  contact_label,
}: AdBlockProps) {
  if (image_url) {
    return (
      <div className="mx-auto border-[#29377E] border rounded-[10px] overflow-hidden shadow-lg shadow-[#151C33]">
        <img className="w-full" src={image_url} alt="Рекламный блок" />
      </div>
    );
  }
  return (
    <div className="bg-[#151C33] border-[#29377E] border rounded-[10px] pt-7 pb-8 px-5 flex-middle flex-col text-[14px] font-inter">
      <h6 className="mb-[15px] text-[#707070]">Реклама:</h6>
      <a
        target="_blank"
        href={contact_url}
        className="text-saturateBlue  hover:font-semibold  transition-all"
      >
        {contact_label}
      </a>
    </div>
  );
}

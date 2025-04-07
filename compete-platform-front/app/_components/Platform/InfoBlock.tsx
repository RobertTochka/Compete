export default function InfoBlock({
  label,
  value,
  line_length,
}: {
  label: string;
  value: string | number;
  line_length: number;
}) {
  return (
    <div className="pt-[18px] pb-[25px] px-[22px] xl:px-3.5 bg-[#29377E1A] rounded-[10px]">
      <h6 className="text-[14px] text-[#9B9B9B] font-medium xl:text-[12px]">
        {label}
      </h6>
      <div
        style={{
          width: line_length + "%",
        }}
        className="mb-[14px] mt-0.5 h-0.5 rounded-full bg-gradient-to-r from-[#03B5E3] to-[#1F5093]"
      />
      <p className="text-[40px] font-medium text-center xl:text-[26px] whitespace-pre-line">
        {value}
      </p>
    </div>
  );
}

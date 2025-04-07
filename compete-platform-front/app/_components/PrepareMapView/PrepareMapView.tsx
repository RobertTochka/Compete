export interface IPrepareMapViewProps {
  title: string;
  score: string;
  preview: string;
  isPickedByUs: boolean;
}

export const PrepareMapView = ({
  title,
  score,
  isPickedByUs,
  preview,
}: IPrepareMapViewProps) => {
  return (
    <div key={title} className="max-w-[400px] w-full relative flex-middle">
      <div
        className={`overflow-hidden rounded-[10px] h-[200px]  relative w-full`}
      >
        <img className="object-cover w-full h-full" src={preview} alt={title} />
        {score && (
          <div
            className={`absolute top-0 left-0 right-0 bottom-0 rounded-[10px] backdrop-blur-[2px] flex-middle ${
              !isPickedByUs ? "bg-negative/10" : "bg-positive/10"
            }`}
          />
        )}
      </div>
      <div className="left-0 right-0 absolute mx-auto text-center flex flex-col gap-1">
        <span className="font-semibold text-[24px]">{title}</span>
        {score && <span className="font-medium text-[40px]">{score}</span>}
      </div>
    </div>
  );
};

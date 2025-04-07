import React from "react";
import { HashLoader } from "react-spinners";

interface InformationProps {
  loading: boolean;
  errorMessage?: string;
  size?: number;
  label?: string;
  marginTop?: string;
}

export const Information = ({
  size = 25,
  loading,
  label,
  marginTop = "4",
  errorMessage,
}: InformationProps) => {
  return (
    <div className={`w-full flex-middle min-h-12 gap-y-2 mt-${marginTop}`}>
      {loading && <HashLoader color="white" size={size} />}
      {(errorMessage || label) && !loading ? (
        <div className="text-center d-flex mx-auto text-[16px] font-medium">
          {errorMessage || label}
        </div>
      ) : null}
    </div>
  );
};

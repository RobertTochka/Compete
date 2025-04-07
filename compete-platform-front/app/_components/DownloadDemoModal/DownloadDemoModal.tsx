import React, { useState } from "react";
import ModalWindow from "../ModalWindow";
import Select from "../AdminPanel/Statistics/Select";
import { IDefaultModalProps } from "../ChangeTeamNameModal/ChangeTeamNameModal";
import { Information } from "../Loading/Loading";

interface IDownloadDemoModalProps extends IDefaultModalProps {
  maps: string[];
}

export const DownloadDemoModal = ({
  maps,
  ...rest
}: IDownloadDemoModalProps) => {
  const [selectedMap, setSelectedMap] = useState<string>(maps[0]);
  return (
    <ModalWindow {...rest}>
      <div className="flex flex-col items-center relative pt-3">
        <div className="absolute top-1">
          <Select
            values={maps}
            setValue={setSelectedMap}
            value={selectedMap}
          ></Select>
        </div>
        <Information
          loading={rest.isLoading}
          errorMessage={rest.errorMessage}
        ></Information>
        <button
          onClick={() => rest.onSubmit(selectedMap)}
          className="rounded-[10px] w-full text-[20px] 
        font-medium leading-normal py-1.5 px-6 bg-saturateBlue mt-4 hover:bg-secondaryBlue"
        >
          Загрузить демо
        </button>
      </div>
    </ModalWindow>
  );
};

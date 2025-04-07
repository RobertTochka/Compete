import React, { useState } from "react";
import Icon from "../Icon";
import clsx from "clsx";
import toast from "react-hot-toast";
import { GetMatchView } from "@/app/_utils/types";
import {
  downloadBlob,
  formatCsMapName,
  mapToTitle,
} from "@/app/_utils/functions";
import { useDownloadDemoFileMutation } from "@/app/_fetures/lib/api/publicLobbiesApi";
import { DownloadDemoModal } from "../DownloadDemoModal/DownloadDemoModal";
import { useHandleError } from "@/app/_utils/hooks/useTemplatedError";
import { CancelMatchModal } from "../CancelMatchModal/CancelMatchModal";

const mapStatusToTextStatus = (status: number): string => {
  if (status === 4 || status === 3) return "Завершён";
  if (status === 2) return "В процессе";
  if (status === 5) return "Разминка";
  if (status === 1) return "Выбор карт";
  return "";
};

interface AdminMatchViewProps extends GetMatchView {
  shouldBeSelected: boolean;
}

export const AdminMatchView = ({
  shouldBeSelected,
  id,
  server,
  playersAmount,
  mapActions,
  status,
}: AdminMatchViewProps) => {
  const [showDeleteModal, setShowDeleteModal] = useState(false);
  const [showModal, setShowModal] = useState(false);
  const [downloadDemo, { isLoading: loadingDemo, error: demoError, reset }] =
    useDownloadDemoFileMutation();
  const demoErrorText = useHandleError(demoError);
  const maps = mapActions
    ?.filter((m) => m.isPicked)
    .map((m) => mapToTitle(m.map));
  const onDemoLoad = (mapname: string) => {
    downloadDemo({ mapname: formatCsMapName(mapname), id })
      .unwrap()
      .then((b) => downloadBlob(b, `${id}_${mapname}.dem`));
  };
  return (
    <>
      {showDeleteModal && (
        <CancelMatchModal
          onClose={() => setShowDeleteModal(false)}
          lobbyId={id}
        ></CancelMatchModal>
      )}
      {showModal && (
        <DownloadDemoModal
          onSubmit={onDemoLoad}
          onClose={() => {
            setShowModal(false);
            reset();
          }}
          isLoading={loadingDemo}
          errorMessage={demoErrorText}
          maps={maps}
        ></DownloadDemoModal>
      )}
      <tr
        key={id}
        className={clsx(shouldBeSelected && "bg-black bg-opacity-15")}
      >
        <td className="px-6 py-4 text-xs text-gray font-bold whitespace-nowrap">
          {id}
        </td>
        <td className="flex items-center px-6 py-4 whitespace-nowrap gap-3 lg:text-[14px]">
          <div className="flex items-center gap-2">
            <img
              className="w-4 h-4 rounded-full"
              src="/img/countries/russia.jpeg"
              alt="Россия"
            />
            <span className="font-medium">{server?.location || "N/A"}</span>
            <div
              className={`w-1.5 h-1.5 rounded-full ${
                server?.isHealthy ? "bg-positive" : "bg-negative"
              } mt-0.5`}
            />
          </div>
        </td>
        <td className="px-6 py-4 whitespace-nowrap">
          <div className="flex-middle gap-2">
            {server?.path || "-"}
            {server?.path && (
              <button
                className="hover:brightness-200 transition-all"
                onClick={() =>
                  navigator.clipboard
                    .writeText(`http://compete/lobby/${id}`)
                    .then((_) => toast.success("Скопировано"))
                }
              >
                <svg
                  xmlns="http://www.w3.org/2000/svg"
                  width="16"
                  height="16"
                  viewBox="0 0 16 16"
                  fill="none"
                >
                  <path
                    d="M8.66536 2H5.33203C4.22746 2 3.33203 2.89543 3.33203 4V12C3.33203 13.1046 4.22746 14 5.33203 14H10.6654C11.7699 14 12.6654 13.1046 12.6654 12V6M8.66536 2L12.6654 6M8.66536 2V5C8.66536 5.55228 9.11308 6 9.66536 6H12.6654"
                    stroke="#707070"
                    stroke-linecap="round"
                    strokeLinejoin="round"
                  />
                </svg>
              </button>
            )}
          </div>
        </td>
        <td className="px-6 py-4 text-gray font-bold whitespace-nowrap">
          <div className="flex-middle gap-1.5">
            {maps
              ? maps.map((map) => (
                  <img
                    key={map}
                    className="h-6"
                    src={`/img/maps/${map}.png`}
                    alt={map}
                  />
                ))
              : "-"}
          </div>
        </td>
        <td className="px-6 py-4 whitespace-nowrap">{playersAmount * 2}</td>
        <td className="px-6 py-4 whitespace-nowrap">
          <div
            className={clsx(
              "inline-flex items-center uppercase rounded-md px-3 py-0.5 font-semibold  text-xs",
              {
                "bg-positive/30": status === 3 || status === 4,
                "bg-[#ffc414]/30": status === 2 || status === 5,
                "bg-[#83b4eb]/30": status === 1,
              }
            )}
          >
            {mapStatusToTextStatus(status)}
          </div>
        </td>

        <td className="px-6 py-4 text-xs text-gray font-bold whitespace-nowrap text-right">
          <div className="flex items-center justify-end gap-2">
            <a href={`/lobby/${id}`} target="_blank">
              <Icon icon="eye" defaultColor="#545454" hoverColor="#ffffff" />
            </a>
            {status === 3 || status === 4 ? (
              <button onClick={() => setShowModal(true)}>
                <Icon
                  icon="cloud"
                  defaultColor="#545454"
                  hoverColor="#ffa500"
                />
              </button>
            ) : null}
            {status === 2 || status === 5 || status === 4 ? (
              <button onClick={() => setShowDeleteModal(true)}>
                <Icon
                  icon="delete"
                  defaultColor="#545454"
                  hoverColor="#ff0000"
                />
              </button>
            ) : null}
          </div>
        </td>
      </tr>
    </>
  );
};

"use client";
import { ReportModal } from "@/app/_components/ReportModal/ReportModal";
import { Report } from "../../../../../_components/Report/Report";
import {
  Api,
  reportsAdapter,
  useCloseReportMutation,
  useGetReportsQuery,
} from "@/app/_fetures/lib/api/publicLobbiesApi";
import { useScrollPagination } from "@/app/_utils/hooks/useScrollPagination";
import { useOrderProps } from "@/app/_utils/hooks/useOrderProps";
import { useHandleError } from "@/app/_utils/hooks/useTemplatedError";

import { Information } from "@/app/_components/Loading/Loading";
import { NotAvailable } from "@/app/_components/NotAvailable/NotAvailable";
import { useState } from "react";
import { GetReportDto } from "@/app/_utils/types";
import { useDispatch } from "react-redux";
import { ThunkDispatch, UnknownAction } from "@reduxjs/toolkit";
import { RootState } from "@reduxjs/toolkit/query";
import { useRouter } from "next/navigation";
import { ReportAdminModal } from "@/app/_components/ReportModal/ReportAdminModal";
const Reports = () => {
  const { order, orderProperty } = useOrderProps();
  const { page, pageSize, onScroll } = useScrollPagination(true, true);
  const dispatch = useDispatch() as ThunkDispatch<any, any, UnknownAction>;
  const [reportForRead, setReportRorRead] = useState<null | GetReportDto>(null);
  const {
    data: reports,
    isLoading: isReportsLoading,
    error: reportsError,
  } = useGetReportsQuery(
    {
      page,
      pageSize,
      order,
      orderProperty,
      searchParam: "",
    },
    {
      selectFromResult: ({ data, ...res }) => ({
        data: reportsAdapter
          .getSelectors()
          .selectAll(data ?? reportsAdapter.getInitialState()),
        ...res,
      }),
    }
  );
  const [closeReport, { error: closeReportError }] = useCloseReportMutation();
  const commonErrorText = useHandleError(reportsError || closeReportError);
  const router = useRouter();

  const onGoToLobby = (lobbyId: number) => {
    router.push(`/admin-panel/matches?search=${lobbyId}&findBy=${"FindById"}`);
  };
  const onCloseReport = (id: number, reportResponse: string) => {
    closeReport({ id, reportResponse })
      .unwrap()
      .then(() => {
        dispatch(
          Api.util.updateQueryData(
            "getReports",
            {
              page,
              pageSize,
              order,
              orderProperty,
              searchParam: "",
            },
            (draft) => {
              reportsAdapter.removeOne(draft, id);
            }
          )
        );
      })
      .catch((error) => {
        console.error("Failed to close report: ", error);
      });

    setReportRorRead(null);
  };
  if (commonErrorText || (isReportsLoading && reports.length == 0))
    return (
      <Information
        loading={isReportsLoading && reports.length == 0}
        errorMessage={commonErrorText}
      ></Information>
    );
  return (
    <section
      className="p-10 pt-0 xl:p-6 h-full overflow-y-auto custom-scrollbar "
      onScroll={isReportsLoading ? () => {} : onScroll}
    >
      {reportForRead != null && (
        <ReportAdminModal
          onClose={() => setReportRorRead(null)}
          {...reportForRead}
          onCloseReport={onCloseReport}
        ></ReportAdminModal>
      )}

      <div className="flex flex-col">
        {reports.length == 0 && (
          <NotAvailable text="Нет открытых жалоб"></NotAvailable>
        )}
        {reports.map((t) => (
          <Report
            key={t.id}
            {...t}
            onClick={() => setReportRorRead(t)}
            onGoToLobby={() => onGoToLobby(t.lobbyId)}
          ></Report>
        ))}
      </div>
    </section>
  );
};

export default Reports;

import { BalanceOperation } from "../BalanceOperation/BalanceOperation";
import {
  paysAdapter,
  useGetUserPaysQuery,
} from "@/app/_fetures/lib/api/publicLobbiesApi";
import { useScrollPagination } from "@/app/_utils/hooks/useScrollPagination";
import { useRef } from "react";
import { groupByCreationTime, handleError } from "@/app/_utils/functions";
import { GetPayDto } from "@/app/_utils/types";
import { Information } from "../Loading/Loading";
import React from "react";
import { IUserBasedStats } from "@/app/(pages)/(guard-user)/balance/page";

interface IBalanceOperationsProps extends IUserBasedStats {}

export const BalanceOperations = ({ userId }: IBalanceOperationsProps) => {
  const { page, pageSize, onScroll } = useScrollPagination<HTMLDivElement>(
    true,
    true
  );
  const {
    data: paysData,
    isLoading: paysLoading,
    error,
  } = useGetUserPaysQuery(
    { id: userId, page, pageSize },
    {
      pollingInterval: 10000,
      selectFromResult: ({ data, ...res }) => ({
        data: paysAdapter
          .getSelectors()
          .selectAll(data ?? paysAdapter.getInitialState()),
        ...res,
      }),
    }
  );
  const pays = groupByCreationTime(paysData);
  const paysErrorText = handleError(error);
  return (
    <div
      className="grow xl:w-full h-[600px]  custom-scrollbar overflow-y-auto pr-6 pl-5"
      onScroll={paysLoading ? () => {} : onScroll}
    >
      <h2 className="mb-9 font-medium text-[24px]">Транзакции</h2>
      {(paysLoading && paysData.length === 0) || error ? (
        <Information
          size={50}
          loading={paysLoading && paysData.length === 0}
          errorMessage={paysErrorText}
        ></Information>
      ) : (
        <>
          {Object.keys(pays).map((g) => (
            <React.Fragment key={g}>
              <div className="mt-5">
                <h6 className="text-[#545454] font-medium">{g}</h6>
                {pays[g].map((p: GetPayDto) => (
                  <BalanceOperation {...p} key={p.id}></BalanceOperation>
                ))}
              </div>
              <div className="w-full h-px bg-white" />
            </React.Fragment>
          ))}
          {paysLoading && paysData.length !== 0 && (
            <Information
              size={20}
              loading={paysLoading && paysData.length !== 0}
            ></Information>
          )}
        </>
      )}
    </div>
  );
};

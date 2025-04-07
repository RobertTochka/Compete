import {
  useCreateReportMutation,
  useGetContactsQuery,
} from "@/app/_fetures/lib/api/publicLobbiesApi";
import Link from "next/link";
import { useState } from "react";
import { ReportModal } from "../../ReportModal/ReportModal";
import { useHandleError } from "@/app/_utils/hooks/useTemplatedError";
import { getUserId } from "@/app/_utils/functions";

export interface ISendFeedbackProps {
  lobbyId: number;
  showReport?: boolean;
}

export default function SendFeedback({
  lobbyId,
  showReport,
}: ISendFeedbackProps) {
  const [showModal, setShowModal] = useState(false);
  const {
    data: contact,
    isLoading: contactsLoading,
    error: contactsError,
  } = useGetContactsQuery();
  const [createReport, { isLoading, error }] = useCreateReportMutation();
  const onSubmit = (value: any) => {
    const payload: { subject: string; content: string } = value;
    createReport({ ...payload, lobbyId, userId: parseInt(getUserId()) })
      .unwrap()
      .then(() => setShowModal(false));
  };
  const commonErrorText = useHandleError(error);
  return (
    <div className="font-inter">
      {showModal && (
        <ReportModal
          onSubmit={onSubmit}
          onClose={() => setShowModal(false)}
          isLoading={isLoading}
          errorMessage={commonErrorText}
        ></ReportModal>
      )}
      {showReport && (
        <button
          onClick={() => setShowModal(true)}
          className="mt-7 rounded-[10px] w-full text-[20px] font-medium leading-normal py-1.5 px-6 bg-[#AD0000]  hover:bg-[#bb5454]"
        >
          Пожаловаться
        </button>
      )}
      <p className="my-7 text-gray leading-[150%] font-medium">
        
      </p>
      {contact && (
        <Link href={contact.telegramForAd}>
          
        </Link>
      )}
    </div>
  );
}

"use client";
import React, { useEffect, useState } from "react";
import { Title } from "./Title";
import Textarea from "./Textarea";
import { SaveButton } from "./SaveButton";
import {
  useGetSupportCoverQuery,
  useUpdateSupportCoverMutation,
} from "@/app/_fetures/lib/api/publicLobbiesApi";
import { useHandleError } from "@/app/_utils/hooks/useTemplatedError";
import { Information } from "../Loading/Loading";
import { GetSupportCover } from "@/app/_utils/types";

export const SupportCover = () => {
  const [rules, setRules] = useState<string>("");
  const [faq, setFaq] = useState<string>("");
  const [
    updateSupportCover,
    { isLoading: updatingSupportCover, error: updateSupportCoverError },
  ] = useUpdateSupportCoverMutation();
  const {
    data: supportCover,
    isLoading: isSupportCoverLoading,
    refetch,
    error: supportCoverError,
  } = useGetSupportCoverQuery();
  useEffect(() => {
    if (supportCover) {
      setFaq(supportCover.faq);
      setRules(supportCover.rules);
    }
  }, [supportCover]);
  const onSave = () => {
    var payload: GetSupportCover = { faq: faq, rules };
    updateSupportCover(payload)
      .unwrap()
      .then((s) => refetch());
  };
  const commonErrorText = useHandleError(
    supportCoverError || updateSupportCoverError
  );
  if (
    (supportCover! && isSupportCoverLoading) ||
    commonErrorText ||
    updatingSupportCover
  )
    return (
      <Information
        loading={
          (supportCover! && isSupportCoverLoading) || updatingSupportCover
        }
        errorMessage={commonErrorText}
      ></Information>
    );
  return (
    <div className="max-w-[400px] w-full">
      <Title>Центр поддержки</Title>
      <div className="flex flex-col gap-3 mt-7">
        <Textarea
          label="Правила платформы"
          placeholder="Начните вводить здесь текст..."
          value={rules}
          onChange={(e) => {
            setRules(e.target.value);
          }}
        />
        <Textarea
          label="Ответы на частые вопросы"
          placeholder="Начните вводить здесь текст..."
          value={faq}
          onChange={(e) => {
            setFaq(e.target.value);
          }}
        />
        <SaveButton onClick={onSave} />
      </div>
    </div>
  );
};

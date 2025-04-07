"use client";
import React, { useEffect, useState } from "react";
import Input from "./Input";
import { Title } from "./Title";
import { SaveButton } from "./SaveButton";
import {
  useGetContactsQuery,
  useUpdateContactsMutation,
} from "@/app/_fetures/lib/api/publicLobbiesApi";
import { useHandleError } from "@/app/_utils/hooks/useTemplatedError";
import { Information } from "../Loading/Loading";
import { GetContacts } from "@/app/_utils/types";

export const Contacts = () => {
  const [contactValue, setContactValue] = useState<object>({});
  const {
    data: contact,
    isLoading: isContactsLoading,
    refetch,
    error: contactsError,
  } = useGetContactsQuery();
  const [
    updateContact,
    { isLoading: updatingContacts, error: updateContactsError },
  ] = useUpdateContactsMutation();
  const commonErrorText = useHandleError(contactsError || updateContactsError);
  useEffect(() => {
    if (contact) setContactValue(contact);
  }, [contact]);
  const onSave = () => {
    const payload: GetContacts = contactValue as GetContacts;
    updateContact(payload)
      .unwrap()
      .then((s) => refetch());
  };
  if ((!contact && isContactsLoading) || commonErrorText || updatingContacts)
    return (
      <Information
        loading={(!contact && isContactsLoading) || updatingContacts}
        errorMessage={commonErrorText}
      ></Information>
    );
  return (
    <div>
      <Title>Контакты</Title>
      <div className="flex flex-col gap-3 mt-7 max-w-[320px]">
        {Object.keys(contactValue).map((key, index) => (
          <Input
            name={key}
            key={index}
            label={key}
            placeholder="https://..."
            onChange={(e) =>
              setContactValue((prev) => ({ ...prev, [key]: e.target.value }))
            }
            value={contactValue[key]}
          />
        ))}
        <SaveButton onClick={onSave} />
      </div>
    </div>
  );
};

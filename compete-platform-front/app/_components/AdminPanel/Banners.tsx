"use client";
import React, { useEffect, useRef, useState } from "react";
import { Title } from "./Title";
import { AdminBanner } from "./AdminBanner";
import { SaveButton } from "./SaveButton";
import {
  useGetBannersQuery,
  useUpdateBannersMutation,
} from "@/app/_fetures/lib/api/publicLobbiesApi";
import { useHandleError } from "@/app/_utils/hooks/useTemplatedError";
import { Information } from "../Loading/Loading";

export const Banners = () => {
  const addBannerRef = useRef<HTMLInputElement>(null);
  const [currentBanners, setCurrentBanners] = useState<string[]>([]);
  const {
    data: banners,
    isLoading: isBannersLoading,
    refetch,
    error: bannersError,
  } = useGetBannersQuery();
  const [addedFiles, setAddedFiles] = useState<File[]>([]);
  useEffect(() => {
    if (banners) setCurrentBanners(banners.banners);
  }, [banners]);
  const [
    updateBanners,
    { isLoading: updatingBanners, error: updateBannersError },
  ] = useUpdateBannersMutation();
  const onSave = () => {
    const form = new FormData();
    currentBanners.forEach((b) => form.append("banners", b));
    addedFiles.forEach((f, i) => form.append(`f${i}`, f));
    updateBanners(form)
      .unwrap()
      .then((s) => {
        refetch();
        setAddedFiles([]);
      });
  };
  const commonErrorText = useHandleError(bannersError || updateBannersError);
  const onDeleteActiveBanner = (index: number) => {
    setCurrentBanners((prev) => prev.filter((b, i) => i !== index));
  };
  const onDeleteFileBanner = (index: number) => {
    setAddedFiles((files) => files.filter((f, i) => i !== index));
  };
  if ((isBannersLoading && !banners) || commonErrorText || updatingBanners)
    return (
      <Information
        loading={(isBannersLoading && !banners) || updatingBanners}
        errorMessage={commonErrorText}
      ></Information>
    );
  return (
    <div className="max-w-[380px] w-full">
      <Title>Рекламные баннеры</Title>
      <div className="flex flex-col gap-3 mt-7 relative">
        {currentBanners.map((file, index) => (
          <AdminBanner
            key={file}
            isNotSaved={false}
            onClick={() => onDeleteActiveBanner(index)}
            img={file}
          ></AdminBanner>
        ))}
        {addedFiles.map((file, index) => (
          <AdminBanner
            key={file.name}
            isNotSaved
            img={URL.createObjectURL(file)}
            onClick={() => onDeleteFileBanner(index)}
          ></AdminBanner>
        ))}
        <button
          className="text-center border-2 border-dashed border-gray text-gray rounded-lg font-semibold py-2.5 px-3 hover:border-white hover:text-white transition-all h-[220px] text-xl flex-middle gap-2"
          onClick={() => addBannerRef.current?.click()}
        >
          <span className="text-2xl">+</span> Добавить
        </button>
        <input
          onChange={(e) => {
            if (e.target.files && e.target.files.length > 0) {
              setAddedFiles([...addedFiles, e.target.files[0]]);
            }
          }}
          accept="image/png, image/gif, image/jpeg"
          ref={addBannerRef}
          type="file"
          className="absolute left-[9999px] opacity-0"
        />
        <SaveButton isBanners onClick={onSave} />
      </div>
    </div>
  );
};

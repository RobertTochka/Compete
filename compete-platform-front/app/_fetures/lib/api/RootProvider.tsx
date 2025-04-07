"use client";
import { ApiProvider } from "@reduxjs/toolkit/query/react";
import React from "react";
import { Api } from "./publicLobbiesApi";

export const RootProvider = ({ children }: { children: React.ReactNode }) => {
  return <ApiProvider api={Api}>{children}</ApiProvider>;
};

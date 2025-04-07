"use client";
import { MatchPrepare } from "@/app/_components/MatchPrepare/MatchPrepare";
import { lobbyData } from "@/app/_utils/fakeData";
import React from "react";

const Test = () => {
  return <MatchPrepare userId={5} {...lobbyData}></MatchPrepare>;
};
export default Test;

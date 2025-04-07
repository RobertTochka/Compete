import { useRef } from "react";
import MatchItem from "../Lobbies/MatchItem";
import { handleError } from "@/app/_utils/functions";
import { useScrollPagination } from "@/app/_utils/hooks/useScrollPagination";
import { Information } from "../Loading/Loading";
import { FetchBaseQueryError } from "@reduxjs/toolkit/query";
import { SerializedError } from "@reduxjs/toolkit";
import { GetLobbyViewDto } from "@/app/_utils/types";

interface MatchesProps {
  onLobbyAdd: (id: number) => void;
  error: FetchBaseQueryError | SerializedError;
  matches: GetLobbyViewDto[];
  lobbyLoading: boolean;
}

export default function Matches({
  onLobbyAdd,
  error,
  matches,
  lobbyLoading,
}: MatchesProps) {
  const scrollRef = useRef<HTMLDivElement>(null);
  const { onScroll } = useScrollPagination<HTMLDivElement>(
    scrollRef.current != null,
    true
  );

  const lobbiesError = handleError(error);

  return (
    <div
      className="flex-1 overflow-auto gap-[20px] flex flex-col custom-scrollbar pr-[5px] mt-[40px]"
      ref={scrollRef}
      onScroll={lobbyLoading ? () => {} : onScroll}
    >
      {(!!matches.length && lobbyLoading) || lobbiesError ? (
        <Information
          size={40}
          loading={!matches.length && lobbyLoading}
          errorMessage={lobbiesError}
        ></Information>
      ) : (
        <>
          {matches.map((match, index) => (
            <div key={index} onClick={() => onLobbyAdd(match.id)}>
              <MatchItem {...match} />
            </div>
          ))}
          {lobbyLoading && matches.length !== 0 && (
            <Information loading={lobbyLoading} size={20}></Information>
          )}
        </>
      )}
    </div>
  );
}

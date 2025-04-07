"use client";
import { RatedUser } from "@/app/_components/RatedUser/RatedUser";
import { useMemo, useRef, useState } from "react";
import { useScrollPagination } from "@/app/_utils/hooks/useScrollPagination";
import {
  ratingAdapter,
  useGetRatingUsersQuery,
} from "@/app/_fetures/lib/api/publicLobbiesApi";
import { handleError } from "@/app/_utils/functions";
import { Information } from "@/app/_components/Loading/Loading";
import { useRouter } from "next/navigation";
import Icon from "@/app/_components/Icon";
import { ratingLabels } from "@/app/_utils/constants/constants";

interface SortByType {
  colIndex: number;
  sortType: "up" | "down"
}

export default function RatingPage() {
  const scrollRef = useRef<HTMLDivElement>(null);
  const router = useRouter();
  const { page, pageSize, onScroll } = useScrollPagination<HTMLDivElement>(
    scrollRef.current != null,
    true
  );
  const [sortBy, setSortBy] = useState<SortByType>({ colIndex: 0, sortType: "down" });
  const {
    data: users,
    isLoading: usersLoading,
    error,
  } = useGetRatingUsersQuery(
    { page, pageSize },
    {
      selectFromResult: ({ data, ...res }) => ({
        data: ratingAdapter
          .getSelectors()
          .selectAll(data ?? ratingAdapter.getInitialState()),
        ...res,
      }),
    }
  );

  const sortedUsers = useMemo(() => {
    const sortByKey = Object.values(ratingLabels).find((item) => item.colIndex === sortBy.colIndex).dataName;
    return [...users].sort((user1, user2) => {
      if (sortBy.sortType === "up") {
        if (user1[sortByKey] < user2[sortByKey]) {
          return 1;
        }
        if (user1[sortByKey] > user2[sortByKey]) {
          return -1;
        }
        return 0
      }
      if (sortBy.sortType === "down") {
        if (user1[sortByKey] > user2[sortByKey]) {
          return 1;
        }
        if (user1[sortByKey] < user2[sortByKey]) {
          return -1;
        }
        return 0
      }
    })
  }, [sortBy, users])

  const userError = handleError(error);
  if (userError || (usersLoading && !users))
    return (
      <Information
        loading={usersLoading && !users}
        errorMessage={userError}
      ></Information>
    );
  return (
    <section className="pl-[60px] pr-20 overflow-auto flex flex-col flex-1 max-w-[1460px] w-full mx-auto">
      <div className="w-full grid grid-cols-7">
        {Object.keys(ratingLabels).map((text, index) => (
          <div
            key={text}
            className="pb-2 text-gray font-medium text-[14px]"
          >
            <div className={`flex ${index === 0 ? "justify-start" : "justify-center"} gap-[10px] items-center`}>
              {text}
              <div className="flex flex-col gap-[6px]">
                <button onClick={() => setSortBy({ colIndex: index, sortType: "up" })}>
                  <Icon defaultColor={sortBy.colIndex === index && sortBy.sortType === "up" ? "#fff" : "#33363F"} icon="arrow-up" />
                </button>
                <button onClick={() => setSortBy({ colIndex: index, sortType: "down" })}>
                  <Icon defaultColor={sortBy.colIndex === index && sortBy.sortType === "down" ? "#fff" : "#33363F"} icon="arrow-down" />
                </button>
              </div>
            </div>
          </div>
        ))}
      </div>
      <div className="flex-1 overflow-auto custom-scrollbar">
        {sortedUsers?.map((user, index) => (
          <RatedUser
            user={user}
            key={user.id}
            index={index}
            onClick={() => router.push(`/profile/${user.id}`)}
          ></RatedUser>
        ))}
      </div>
    </section>
  );
}

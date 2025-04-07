import React, { useEffect, useRef, useState } from "react";

export function useScrollPagination<T extends HTMLElement>(
  shouldCount: boolean,
  toEnd: boolean
) {
  const lastScrollTimeRef = useRef<number>(0);
  const [page, setPage] = useState(1);
  const [pageSize, setPageSize] = useState(30);

  useEffect(() => {
    setPage(1);
  }, [pageSize]);

  const onScroll = (e: React.UIEvent<T>) => {
    const { scrollTop, scrollHeight, clientHeight } = e.currentTarget;
    const now = Date.now();

    const handleScroll = () => {
      setPage((page) => page + 1);
      lastScrollTimeRef.current = now;
    };

    if (!toEnd) {
      if (scrollTop === 0 && shouldCount) {
        if (now - lastScrollTimeRef.current > 2000) {
          handleScroll();
        }
      }
    } else {
      if (scrollTop + clientHeight === scrollHeight && shouldCount) {
        if (now - lastScrollTimeRef.current > 2000) {
          handleScroll();
        }
      }
    }
  };

  return { page, setPage, pageSize, setPageSize, onScroll };
}

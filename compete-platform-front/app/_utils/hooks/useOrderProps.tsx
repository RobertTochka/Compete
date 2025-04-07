import { useState } from "react";

export const useOrderProps = () => {
  const [order, setOrder] = useState<string>("asc");
  const [orderProperty, setOrderProperty] = useState<string>("id");
  const onHeaderClick = (text: string) => {
    setOrderProperty((prev) => {
      if (prev === text) {
        setOrder((prevOrder) => {
          if (prevOrder === "asc") return "desc";
          return "asc";
        });
      } else setOrder("asc");
      return text;
    });
  };
  return { order, orderProperty, onHeaderClick };
};

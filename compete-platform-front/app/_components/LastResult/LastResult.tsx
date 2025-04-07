import React from "react";
interface LastResultProps {
  result: string;
}
export const LastResult = ({ result }: LastResultProps) => {
  return (
    <span className={result === "W" ? "text-positive" : "text-negative"}>
      {result}
    </span>
  );
};

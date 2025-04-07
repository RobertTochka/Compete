import React from "react";
import Button from "./Button";

interface SaveButtonProps {
  isBanners?: boolean;
  onClick: () => void;
}

export const SaveButton = ({ isBanners, onClick }: SaveButtonProps) => {
  return (
    <div className="mt-2">
      <Button onClick={onClick}>Сохранить</Button>
    </div>
  );
};

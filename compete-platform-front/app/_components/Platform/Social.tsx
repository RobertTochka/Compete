import React from "react";
import Icon from "../Icon";

interface SocialProps {
  url: string;
  label: string;
}

export const Social = ({ label, url }: SocialProps) => {
  return (
    <a href={url} target="_blank">
      <Icon defaultColor="#9B9B9B" hoverColor="#ffffff" icon={label} />
    </a>
  );
};

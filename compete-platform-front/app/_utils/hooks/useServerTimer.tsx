import { useEffect, useState } from "react";
import {
  configureConnectionForPickMapSecondsNotifications,
  disposeConnectionForPickMapSecondsNotifications,
} from "../functions";

export const useServerTimer = () => {
  const [seconds, setSeconds] = useState(0);
  useEffect(() => {
    configureConnectionForPickMapSecondsNotifications(setSeconds);
    return () => {
      disposeConnectionForPickMapSecondsNotifications();
    };
  }, []);
  return seconds;
};

import { SerializedError } from "@reduxjs/toolkit";
import { FetchBaseQueryError } from "@reduxjs/toolkit/query";
import { useMemo } from "react";
import { handleError } from "../functions";
import { ProblemDetails } from "../types";
export function isProblemDetails(error: any): error is ProblemDetails {
  return error?.detail !== undefined;
}
export const useHandleError = (
  error: FetchBaseQueryError | SerializedError | undefined
) => {
  return useMemo(() => handleError(error), [error]);
};

import { useState } from "react"
import toast from "react-hot-toast"

type CopiedValue = string | null
type CopyFunction = (text: string, successText?: string) => Promise<void>

export const useCopyToClipboard = () => {
  const [copiedText, setCopiedText] = useState<CopiedValue>(null)

  const copy: CopyFunction = async (text, successText?) => {
    try {
      await navigator.clipboard.writeText(text)
      setCopiedText(text)
      toast.success("Скопировано");
    } catch (error) {
      console.log(error);
      if (error instanceof Error) {
        toast.error("Не удалось скопировать текст")
      }
    }
  }

  return [copiedText, copy] as const;
}
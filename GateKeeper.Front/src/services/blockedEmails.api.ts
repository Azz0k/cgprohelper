import {queryClient} from "../main.tsx";
import {FetchData} from "./DataService.api.ts";

const blockedEmailsApiUrl = import.meta.env.VITE_BLOCKED_EMAILS_API_URL;

export const loadAllBlockedEmails = async () => {
  return await queryClient.fetchQuery({
    queryKey: ["BlockedEmails", "get"],
    queryFn: () => FetchData(blockedEmailsApiUrl).then(res => {
      if (res.status !== 200) {
        throw res.status;
      }
      return res.json();
    }),
    staleTime: 60_000,
  });
};
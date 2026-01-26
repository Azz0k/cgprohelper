import {queryClient} from "../main.tsx";
import {DeleteData, FetchData} from "./DataService.api.ts";

const foreignEmailsApiUrl = import.meta.env.VITE_FOREIGN_EMAILS_API_URL;

export const loadAllForeignEmails = async () => {
  return await queryClient.fetchQuery({
    queryKey: ["ForeignEmails", "get"],
    queryFn: () => FetchData(foreignEmailsApiUrl).then(res => {
      if (res.status !== 200) {
        throw res.status;
      }
      return res.json();
    }),
    staleTime: 60_000,
  });
};
export const deleteForeignEmail = async (id:number)=>{
  const res = await DeleteData(`${foreignEmailsApiUrl}/${id}`);
  return res.status;
}
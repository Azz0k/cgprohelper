import {queryClient} from "../main.tsx";
import {AddData, DeleteData, FetchData, UpdateData} from "./DataService.api.ts";

const localEmailApiUrl = import.meta.env.VITE_LOCAL_EMAILS_API_URL;

export const addLocalEmail = async (body:string)=>{
  const res = await AddData(localEmailApiUrl, body);
  if (res.status !== 200) {
    throw res.status;
  }
  return res.text();
}
export const updateLocalEmail = async (body:string)=>{
  const res = await UpdateData(localEmailApiUrl, body);
  if (res.status !== 200) {
    throw res.status;
  }
};
export const deleteLocalEmail = async (id:number)=>{
  const res = await DeleteData(`${localEmailApiUrl}/${id}`);
  return res.status;
}
export const loadAllLocalEmails = async () => {
  return await queryClient.fetchQuery({
    queryKey: ["localEmails", "get"],
    queryFn:()=> FetchData(localEmailApiUrl)
      .then(res => {
        if (res.status !== 200) {
          throw res.status;
        }
        return res.json();
      }),
    staleTime: 60_000,
  });
};

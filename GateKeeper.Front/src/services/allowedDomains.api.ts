import {queryClient} from "../main.tsx";
import {AddData, DeleteData, FetchData, UpdateData} from "./DataService.api.ts";

const allowedDomainsApiUrl = import.meta.env.VITE_ALLOWED_DOMAINS_API_URL;

export const addAllowedDomain = async (body:string)=>{
  const res = await AddData(allowedDomainsApiUrl, body);
  if (res.status !== 200) {
    throw res.status;
  }
  return res.json();
}
export const loadAllAllowedDomains = async () => {
  return await queryClient.fetchQuery({
    queryKey: ["allowedDomains", "get"],
    queryFn: () => FetchData(allowedDomainsApiUrl).then(res => {
      if (res.status !== 200) {
        throw res.status;
      }
      return res.json();
    }),
    staleTime: 60_000,
  });
};
export const updateAllowedDomain = async (body:string)=>{
  const res = await UpdateData(allowedDomainsApiUrl, body);
  if (res.status !== 200) {
    throw res.status;
  }
};
export const deleteAllowedDomain = async (id:number)=>{
  const res = await DeleteData(`${allowedDomainsApiUrl}/${id}`);
  return res.status;
}
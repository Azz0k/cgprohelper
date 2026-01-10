import {queryClient} from "../main.tsx";

const allowedDomainsApiUrl = 'http://localhost:64346/api/AllowedDomains';
export const addAllowedDomain = async (body:string)=>{
  const res = await fetch(allowedDomainsApiUrl, {
    method: 'POST',
    body: body,
    headers: {
      'Content-Type': 'application/json'
    }
  });
  if (res.status !== 200) {
    console.log(res);
    throw res.status;
  }
  return res.json();
}
export const loadAllAllowedDomains = async () => {
  return await queryClient.fetchQuery({
    queryKey: ["allowedDomains", "get"],
    queryFn: () => fetch(allowedDomainsApiUrl).then(res => res.json()),
    staleTime: 60_000,
  });
};
export const updateAllowedDomain = async (body:string)=>{
  const res = await fetch(allowedDomainsApiUrl, {
    method: 'PUT',
    body: body,
    headers: {
      'Content-Type': 'application/json'
    }
  });
  if (res.status !== 200) {
    console.log(res);
    throw res.status;
  }
};
export const deleteAllowedDomain = async (id:number)=>{
  const res = await fetch(`${allowedDomainsApiUrl}/${id}`, {
    method: 'DELETE',
  });
  console.log(res.status);
  return res.status;
}
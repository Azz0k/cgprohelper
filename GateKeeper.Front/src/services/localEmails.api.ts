import {queryClient} from "../main.tsx";
import {rootStore} from "../store/RootStore.ts";

const localEmailApiUrl = import.meta.env.VITE_LOCAL_EMAILS_API_URL;
export const updateLocalEmail = async (body:string)=>{
  const res = await fetch(localEmailApiUrl, {
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
export const deleteLocalEmail = async (id:number)=>{
  const res = await fetch(`${localEmailApiUrl}/${id}`, {
    method: 'DELETE',
  });
  console.log(res.status);
  return res.status;
}
export const loadAllLocalEmails = async () => {
  return await queryClient.fetchQuery({
    queryKey: ["localEmails", "get"],
    queryFn: () => fetch(localEmailApiUrl, {
      method: 'GET',
      headers: {
        'Authorization': `Bearer ${rootStore.token}`,
      }
    }).then(res => res.json()),
    staleTime: 60_000,
  });
};
export const addLocalEmail = async (body:string)=>{
  const res = await fetch(localEmailApiUrl, {
    method: 'POST',
    body: body,
    headers: {
      'Content-Type': 'application/json'
    }
  });
  if (res.status !== 200) {
    throw res.status;
  }
  return res.text();
}
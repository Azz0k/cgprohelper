import {queryClient} from "../main.tsx";

const foreignEmailsApiUrl = import.meta.env.VITE_FOREIGN_EMAILS_API_URL;

export const loadAllForeignEmails = async () => {
  return await queryClient.fetchQuery({
    queryKey: ["ForeignEmails", "get"],
    queryFn: () => fetch(foreignEmailsApiUrl).then(res => res.json()),
    staleTime: 60_000,
  });
};
export const deleteForeignEmail = async (id:number)=>{
  const res = await fetch(`${foreignEmailsApiUrl}/${id}`, {
    method: 'DELETE',
  });
  console.log(res.status);
  return res.status;
}
const authApiUrl = import.meta.env.VITE_AUTH_API_URL;
export const Authenticate = async (body:string) => {
  const res = await fetch(authApiUrl, {
    method: 'POST',
    body: body,
    headers: {
      'Content-Type': 'application/json'
    }
  });
  if (res.status !== 200) {
    throw res.status;
  }
  return res.json();
}
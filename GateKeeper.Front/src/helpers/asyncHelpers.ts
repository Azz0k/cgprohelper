type AsyncContext= {
    token: symbol,
    abortControllers: AbortController[]
};
type FetchFunction =  () => void;
const asyncContexts = new WeakMap<FetchFunction, AsyncContext>();
export const asyncHelpers = (fn:FetchFunction) => {
    let ctx = asyncContexts.get(fn);
    if (ctx) {
        ctx.abortControllers.forEach(abortController => abortController.abort());
    }
    ctx = {
        token: Symbol(),
        abortControllers: []
    }
    asyncContexts.set(fn, ctx);
    const currentToken = ctx.token;
    return {
        abortControllersArray: ctx.abortControllers,
        debounce (ms:number) {
            return new Promise<boolean>((resolve) => {
                setTimeout(()=>{
                    resolve(asyncContexts.get(fn)?.token===currentToken);
                },ms);
            })
        },
        stillActual(){
            return asyncContexts.get(fn)?.token === currentToken;
        }
    }
}
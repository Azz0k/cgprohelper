import { makeAutoObservable } from "mobx";
import {Router} from "../routes/Router.tsx";
import {routes} from "../routes/routes.ts";
import {localEmailsState} from "../pages/LocalEmails/LocalEmailsState.ts";
import {allowedDomainState} from "../pages/AllowedDomains/AllowedDomainState.ts";



class RootStore {
    globalMenuSelectedKey: string = "1";
    pathName!: string;
    constructor() {
        makeAutoObservable(this);
        Router.subscribe('onResolved', (evt)=>{
            this.pathName = evt.toLocation.pathname;
        });

        const pathname = Router.__store.state.location.pathname;
        const key = Object.keys(pathname).find(key => routes[key] === pathname);
        if(key)
        {
            this.globalMenuSelectedKey = key;
        }
    }
    handleMenuSelected = (e: { key: string; })=>{
        this.globalMenuSelectedKey = e.key;
        const href = routes[e.key as "1" | "2" | "3"];
        Router.navigate({href}).catch(console.error);
    }
    get localState(){
        return  this.pathName === '/'
          ? localEmailsState
          : allowedDomainState;
    }
}

export const rootStore= new RootStore();
import { makeAutoObservable } from "mobx";
import {Router} from "../routes/Router.tsx";
import {routes} from "../routes/routes.ts";


class RootStore {
    globalMenuSelectedKey: string = "1";
    constructor() {
        makeAutoObservable(this);
    }
    handleMenuSelected = (e: { key: string; })=>{
        this.globalMenuSelectedKey = e.key;
        const href = routes[e.key as "1" | "2" | "3"];
        Router.navigate({href}).catch(console.error);
    }
}

export const rootStore= new RootStore();
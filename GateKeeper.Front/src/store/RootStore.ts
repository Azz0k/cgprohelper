import { makeAutoObservable } from "mobx";
import {Router} from "../routes/Router.tsx";
import {routes} from "../routes/routes.ts";
import {localEmailsState} from "../pages/LocalEmails/LocalEmailsState.ts";
import {allowedDomainState} from "../pages/AllowedDomains/AllowedDomainState.ts";
import type {LocalState} from "../interfaces/LocalState.ts";
import {foreignEmailsState} from "../pages/ForeignEmails/ForeignEmailsState.ts";
import type {LoginPass} from "../interfaces/LoginPass.ts";
import {Authenticate} from "../services/Authenticate.api.ts";
import {usersState} from "../pages/Users/UsersState.ts";

class RootStore {
  globalMenuSelectedKey: string = "1";
  pathName!: string;
  token:string |null = null;
  userName:string |null = null;
  constructor() {
    makeAutoObservable(this);
    Router.subscribe('onResolved', (evt)=>{
      this.pathName = evt.toLocation.pathname;
    });
    this.token = localStorage.getItem("token");
    this.userName = localStorage.getItem("userName");
    const pathname = Router.__store.state.location.pathname;
    const key = Object.keys(pathname).find(key => routes[key] === pathname);
    if(key)
    {
      this.globalMenuSelectedKey = key;
    }
  }
  get isLoggedIn(): boolean {
    return this.token !== null;
  }
  get isAdmin(): boolean {
    if (this.token == null) return false;
    const tokenParts = this.token.split('.');
    if (tokenParts.length < 2) return false;
    const base64 = tokenParts[1].replace(/-/g, '+').replace(/_/g, '/');
    return atob(base64).includes('IsAdmin');
  }
  handleLogout = ()=>  {
    localStorage.removeItem("token");
    localStorage.removeItem("userName");
    this.token = null;
    this.userName = null;
  }
  handleFinishLoginForm =(values:LoginPass)=>{
    this.Authenticate(values).then();
  }
  async Authenticate(values:LoginPass): Promise<void> {
    try{
      const result= await Authenticate(JSON.stringify(values));
      const token=result.token;
      if(token){
        this.token = token;
        this.userName = values.Login;
        localStorage.setItem("token", token);
        localStorage.setItem("userName", values.Login);
      }
    }
    catch(error:unknown){
      switch (error){
        default:
          console.log(error);
          break;
      }
    }
  }

  handleMenuSelected = (e: { key: string; })=>{
    this.globalMenuSelectedKey = e.key;
    const href = routes[e.key as "1" | "2" | "3"];
    Router.navigate({href}).catch(console.error);
  }
  get localState(): LocalState{
    switch (this.pathName) {
      case '/':
        return localEmailsState;
      case '/alloweddomains':
        return allowedDomainState;
      case '/users':
        return usersState;
      default:
        return foreignEmailsState;
    }
  }
}

export const rootStore= new RootStore();
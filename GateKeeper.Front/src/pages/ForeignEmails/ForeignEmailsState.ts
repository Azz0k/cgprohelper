import {BasePageStore} from "../../store/BasePageStore.tsx";
import {computed, makeObservable, action, observable} from "mobx";
import {deleteForeignEmail, loadAllForeignEmails} from "../../services/foreignEmails.api.ts";
import {rootStore} from "../../store/RootStore.ts";


export type ForeignEmail = {
  id: number;
  email: string;
  receivedDate:string;
}
type ForeignEmails = ForeignEmail[];

class ForeignEmailsState  extends  BasePageStore{
  foreignEmails: ForeignEmails = [];
  constructor() {
    super();
    makeObservable(this, {
      foreignEmailFound: computed,
      foreignEmails: observable,
      loading: observable,
      errorEditEntity: observable,
      errorAddEntity: observable,
      editingId: observable,
      showDeleteDialogId: observable,
      addPopoverOpened: observable,
      searchText: observable,
      handleSearchChange: action,
      handleApplyClick: action,
      handleCancelEditClick: action,
      handleYesClickAfterDeleteClick: action,
      handleEditClick: action,
      LoadAllForeignEmails: action,
    });
  }
  get foreignEmailFound(){
    if (this.searchText)
      return this.foreignEmails.filter(value => value.email.includes(this.searchText));
    else
      return this.foreignEmails;
  }
  handleApplyClick=()=>{
  }
  handleEditClick=()=>{
  }
  handleCancelEditClick=()=>{
  }
  handleYesClickAfterDeleteClick=()=>{
    this.DeleteForeignEmail(this.showDeleteDialogId).then((result) => {
      if (result) {
        this.foreignEmails = this.foreignEmails.filter(value => value.id !== this.showDeleteDialogId);
      }
      this.showDeleteDialogId = -1;
    })
  }
  async LoadAllForeignEmails(){
    this.loading = true;
    try{
      this.foreignEmails = await loadAllForeignEmails() as ForeignEmails;
    }
    catch (error:unknown) {
      switch (error){
        case 401:
          this.foreignEmails = [];
          rootStore.handleLogout();
          break;
          default:
            break;
      }
    }
    finally{
      this.loading = false;
    }
  }
  async DeleteForeignEmail(id:number){
    try {
      const code = await deleteForeignEmail(id);
      if (code === 401){
        this.foreignEmails = [];
        rootStore.handleLogout();
        return false;
      }
      return code === 204;
    }
    catch  {
      return false;
    }
  }
}

export const foreignEmailsState = new ForeignEmailsState();
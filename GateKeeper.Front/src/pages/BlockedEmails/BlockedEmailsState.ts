import {BasePageStore} from "../../store/BasePageStore.tsx";
import {computed, makeObservable, action, observable} from "mobx";
import {rootStore} from "../../store/RootStore.ts";
import {loadAllBlockedEmails} from "../../services/blockedEmails.api.ts";


export type BlockedEmail = {
  id: number;
  senderEmail: string;
  recipientEmail:string;
  date: string;
  time: string;
}
const sortBlockedEmails = (a:BlockedEmail,b:BlockedEmail) => {
  if (a.date> b.date) {
    return -1;
  }
  if (a.date< b.date) {
    return 1;
  }
  if (a.date === b.date) {
    if (a.time > b.time) {
      return -1;
    }
    if (a.time < b.time) {
      return 1;
    }
  }
  return 0;
}
type BlockedEmails = BlockedEmail[];
class BlockedEmailsState extends BasePageStore {
  blockedEmails: BlockedEmails = [];
  constructor() {
    super();
    makeObservable(this, {
      blockedEmailFound: computed,
      blockedEmails: observable,
      loading: observable,
      errorEditEntity: observable,
      errorAddEntity: observable,
      editingId: observable,
      addPopoverOpened: observable,
      searchText: observable,
      handleSearchChange: action,
      handleApplyClick: action,
      handleCancelEditClick: action,
      handleYesClickAfterDeleteClick: action,
      handleEditClick: action,
      LoadAllBlockedEmails: action,
    });
  }
  get blockedEmailFound(){
    if (this.searchText)
      return this.blockedEmails.filter(value =>
        value.senderEmail.includes(this.searchText) ||
        value.recipientEmail.includes(this.searchText) ||
        value.date.includes(this.searchText) ||
        value.time.includes(this.searchText))
        .slice().sort(sortBlockedEmails);
    else
      return this.blockedEmails.slice().sort(sortBlockedEmails);
  }
  handleApplyClick = () => {
  }
  handleCancelEditClick = () => {
  }
  handleYesClickAfterDeleteClick = () => {
  }
  handleEditClick = () => {
  }
  async LoadAllBlockedEmails() {
    this.loading = true;
    try{
      this.blockedEmails = await loadAllBlockedEmails() as BlockedEmails;
    }
    catch (error:unknown) {
      switch (error){
        case 401:
          this.blockedEmails = [];
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
}

export const blockedEmailsState = new BlockedEmailsState();
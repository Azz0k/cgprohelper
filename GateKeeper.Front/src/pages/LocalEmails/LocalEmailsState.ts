import {makeObservable, observable, action, computed} from "mobx";
import React from 'react';
import {addLocalEmail, deleteLocalEmail, loadAllLocalEmails, updateLocalEmail} from "../../services/localEmails.api.ts";
import {BasePageStore} from "../../store/BasePageStore.tsx";
import {rootStore} from "../../store/RootStore.ts";

export type LocalEmail = {
  id: number;
  email: string;
  isReplyAllowed:boolean;
}
type NewEmail = {
  email: string;
  isReplyAllowed:boolean;
}

type LocalEmails = LocalEmail[];
class LocalEmailsState extends BasePageStore{
  localEmails: LocalEmail[] = [];
  originalLocalEmail!: LocalEmail;

  constructor() {
    super();
    makeObservable(this, {
      localEmails: observable,
      originalLocalEmail: observable,
      loading: observable,
      errorEditEntity: observable,
      errorAddEntity: observable,
      editingId :observable,
      showDeleteDialogId: observable,
      addPopoverOpened: observable,
      searchText:observable,
      LocalEmailsFound:computed,
      handleSearchChange:action,
      handleEditClick:action,
      handleCancelEditClick:action,
      handleApplyClick:action,
      handleDeleteClick:action,
      handleYesClickAfterDeleteClick:action,
      handleNoClickAfterDeleteClick:action,
      handleCheckedChange:action,
      handleInputChange:action,
      handlePlusClick:action,
      handleCancelAddClick:action,
      handleSaveClick:action,
      AddLocalEmail:action,
      LoadAllLocalEmails:action,
      UpdateLocalEmail:action,
      DeleteLocalEmail:action,
    });
  }
  get LocalEmailsFound(){
    if (this.searchText)
      return this.localEmails.filter(value => value.email.includes(this.searchText));
    else
      return this.localEmails;
  }

  handleEditClick = (id:number) => {
    this.editingId = id;
    const element = this.localEmails.find(value => value.id === id);
    if (element) {
      this.originalLocalEmail = element;
    }
  }
  handleCancelEditClick = () => {
    this.localEmails = this.localEmails.map(value => {
      if (value.id === this.editingId) return {...this.originalLocalEmail}
      else return value;
    });
    this.errorEditEntity = null;
    this.editingId = -1;
  }
  handleApplyClick = () => {
    this.UpdateLocalEmail(this.editingId).then(()=>{
      if (this.errorEditEntity === null) {
        this.editingId = -1
      }
    });
  }

  handleYesClickAfterDeleteClick =()=>{
    this.DeleteLocalEmail(this.showDeleteDialogId).then((result) => {
      if (result) {
        this.localEmails = this.localEmails.filter(value => value.id !== this.showDeleteDialogId);
      }
      this.showDeleteDialogId = -1;
    })
  }

  handleCheckedChange = (id:number) => {
    this.localEmails = this.localEmails.map(value => {
      if (value.id === id) {
        return {...value, isReplyAllowed:!value.isReplyAllowed};
      }
      return value;
    });
  }
  handleInputChange = (id:number, e:React.ChangeEvent<HTMLInputElement>) => {
    this.localEmails = this.localEmails.map(value => {
      if (value.id === id) {
        return {...value, email:e.target.value};
      }
      return value;
    });
  }

  handleSaveClick = (email:string, isReplyAllowed:boolean) => {
    const newEmail:NewEmail = {email:email, isReplyAllowed:isReplyAllowed};
    this.AddLocalEmail(newEmail).then((result)=>{this.addPopoverOpened = !result});
  }

  async AddLocalEmail(newEmail:NewEmail){
    this.errorAddEntity = null;
    try {
      const body = JSON.stringify(newEmail);
      const res: string = await addLocalEmail(body);
      const id = parseInt(res);
      if (!Number.isNaN(id)) {
        if (!this.localEmails.find(value => value.id === id)) {
          const newLocalEmail:LocalEmail = {...newEmail, id:id};
          this.localEmails = [...this.localEmails, newLocalEmail]
          return true;
        }
      }
      this.errorAddEntity = "Email already exists";
      return false;
    }
    catch(error:unknown){
      console.log(error);
      switch (error){
        case 401:
          this.localEmails = [];
          rootStore.handleLogout();
          break;
        default:
          this.errorAddEntity = 'Unknown error';
          break;
      }
      return false;
    }
  }
  async LoadAllLocalEmails(){
    this.loading = true;
    try{
      this.localEmails = await loadAllLocalEmails() as LocalEmails;
    }
    catch(error:unknown){
      switch (error){
        case 401:
          this.localEmails = [];
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
  async UpdateLocalEmail(id:number){
    this.errorEditEntity = null;
    try{
      const body = JSON.stringify(this.localEmails.find(value => value.id === id));
      await updateLocalEmail(body);
    }
    catch (error:unknown) {
      console.log(error);
      switch (error){
        case 401:
          this.localEmails = [];
          rootStore.handleLogout();
          break;
        case 400:
          this.errorEditEntity = 'Email already exists';
          break;
        case 404:
          this.errorEditEntity = 'Not Found. Please update this page.';
          break;
        default:
          this.errorEditEntity = 'Unknown error';
          break;
      }
    }
  }
  async DeleteLocalEmail(id:number){
    try {
      const code = await deleteLocalEmail(id);
      if (code === 401){
        this.localEmails = [];
        rootStore.handleLogout();
        return false;
      }
      return code===204;
    }
    catch  {
      return false;
    }
  }
}


export const localEmailsState = new LocalEmailsState();

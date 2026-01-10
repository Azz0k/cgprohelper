import {makeAutoObservable, computed} from "mobx";
import React from 'react';
import {addLocalEmail, deleteLocalEmail, loadAllLocalEmails, updateLocalEmail} from "../../services/localEmails.api.ts";

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
class LocalEmailsState {
  localEmails: LocalEmail[] = [];
  loading = false;
  error:string | null = null;
  errorAddEmail:string | null = null;
  editingId:number = -1;
  originalLocalEmail!: LocalEmail;
  showDeleteDialogId: number = -1;
  addPopoverOpened: boolean = false;
  searchText:string = "";
  constructor() {
    makeAutoObservable(this, {
      LocalEmailsFound:computed
    });
  }
  get LocalEmailsFound(){
    if (this.searchText)
      return this.localEmails.filter(value => value.email.includes(this.searchText));
    else
      return this.localEmails;
  }
  handleSearchChange = (e:React.ChangeEvent<HTMLInputElement>) => {
    this.searchText=e.target.value;
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
    this.error = null;
    this.editingId = -1;
  }
  handleApplyClick = () => {
    this.UpdateLocalEmail(this.editingId).then(()=>{
      if (this.error === null) {
        this.editingId = -1
      }
    });
  }
  handleDeleteClick = (id:number) => {
    this.showDeleteDialogId = id;
  }
  handleYesClickAfterDeleteClick =()=>{
    this.DeleteLocalEmail(this.showDeleteDialogId).then((result) => {
      console.log(result);
      if (result) {
        this.localEmails = this.localEmails.filter(value => value.id !== this.showDeleteDialogId);
      }
      this.showDeleteDialogId = -1;
    })
  }
  handleNoClickAfterDeleteClick = () => {
    this.showDeleteDialogId = -1;
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
  handlePlusClick = () => {
    this.addPopoverOpened = !this.addPopoverOpened;
  }
  handleCancelAddClick = () => {
    this.addPopoverOpened = false;
    this.errorAddEmail = null;
  }
  handleSaveClick =(email:string, isReplyAllowed:boolean) => {
    const newEmail:NewEmail = {email:email, isReplyAllowed:isReplyAllowed};
    this.AddLocalEmail(newEmail).then((result)=>{this.addPopoverOpened = !result});
  }
  async AddLocalEmail(newEmail:NewEmail){
    this.errorAddEmail = null;
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
      this.errorAddEmail = "Email already exists";
      return false;
    }
    catch(error:unknown){
      console.log(error);
      switch (error){
        default:
          this.errorAddEmail = 'Unknown error';
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
    finally{
      this.loading = false;
    }
  }
  async UpdateLocalEmail(id:number){
    this.error = null;
    try{
      const body = JSON.stringify(this.localEmails.find(value => value.id === id));
      await updateLocalEmail(body);
    }
    catch (error:unknown) {
      console.log(error);
      switch (error){
        case 400:
          this.error = 'Email already exists';
          break;
        case 404:
          this.error = 'Not Found. Please update this page.';
          break;
        default:
          this.error = 'Unknown error';
          break;
      }
    }
  }
  async DeleteLocalEmail(id:number){
    try {
      return await deleteLocalEmail(id)===204;
    }
    catch  {
      return false;
    }
  }
}


export const localEmailsState = new LocalEmailsState();

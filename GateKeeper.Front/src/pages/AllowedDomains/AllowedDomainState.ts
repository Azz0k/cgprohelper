import {computed, makeObservable, action, observable} from "mobx";
import React from "react";
import {BasePageStore} from "../../store/BasePageStore.tsx";
import {
  addAllowedDomain,
  deleteAllowedDomain,
  loadAllAllowedDomains,
  updateAllowedDomain
} from "../../services/allowedDomains.api.ts";


export type AllowedDomain = {
  id: number;
  domain: string;
}
export type NewAllowedDomain = {
  domain: string[];
}
type ResultCreatedAllowedDomain = {
  201:AllowedDomain[]
  400:AllowedDomain[]
}
type AllowedDomains = AllowedDomain[];
class AllowedDomainState extends BasePageStore{

  allowedDomains: AllowedDomain[] = [];
  originalAllowedDomain!: AllowedDomain;
  constructor() {
    super();
    makeObservable(this, {
      AllowedDomainFound:computed,
      allowedDomains: observable,
      originalAllowedDomain: observable,
      loading: observable,
      errorEditEntity: observable,
      errorAddEntity: observable,
      editingId :observable,
      showDeleteDialogId: observable,
      addPopoverOpened: observable,
      searchText:observable,
      handleSearchChange:action,
      handleEditClick:action,
      handleCancelEditClick:action,
      handleApplyClick:action,
      handleDeleteClick:action,
      handleYesClickAfterDeleteClick:action,
      handleNoClickAfterDeleteClick:action,
      handleInputChange:action,
      handlePlusClick:action,
      handleCancelAddClick:action,
      handleSaveClick:action,
      AddAllowedDomain:action,
      LoadAllAllowedDomains:action,
      UpdateAllowedDomain:action,
      DeleteAllowedDomain:action,
    });
  }
  get AllowedDomainFound(){
    if (this.searchText)
      return this.allowedDomains.filter(value => value.domain.includes(this.searchText));
    else
      return this.allowedDomains;
  }
  handleInputChange = (id:number, e:React.ChangeEvent<HTMLInputElement>) => {
    this.allowedDomains = this.allowedDomains.map(value => {
      if (value.id === id) {
        return {...value, domain:e.target.value};
      }
      return value;
    });
  }
  handleEditClick =  (id:number) => {
    this.editingId = id;
    const element = this.allowedDomains.find(value => value.id === id);
    if (element) {
      this.originalAllowedDomain = element;
    }
  }
  handleCancelEditClick= () => {
    this.allowedDomains = this.allowedDomains.map(value => {
      if (value.id === this.editingId) return {...this.originalAllowedDomain}
      else return value;
    });
    this.errorEditEntity = null;
    this.editingId = -1;
  }
  handleApplyClick= () => {
    this.UpdateAllowedDomain(this.editingId).then(()=>{
      if (this.errorEditEntity === null) {
        this.editingId = -1
      }
    });
  }
  handleYesClickAfterDeleteClick =()=>{
    this.DeleteAllowedDomain(this.showDeleteDialogId).then((result) => {
      if (result) {
        this.allowedDomains = this.allowedDomains.filter(value => value.id !== this.showDeleteDialogId);
      }
      this.showDeleteDialogId = -1;
    })
  }
  handleSaveClick = (newDomain:string) => {
    const domainArray:string[] = [];
    domainArray.push(newDomain);
    const newAllowedDomain:NewAllowedDomain = {domain:domainArray};
    this.AddAllowedDomain(newAllowedDomain).then((result)=>{this.addPopoverOpened = !result});
  }
  async AddAllowedDomain(newDomain:NewAllowedDomain){
    this.errorAddEntity = null;
    try {
      const body = JSON.stringify(newDomain);
      const res: ResultCreatedAllowedDomain = await addAllowedDomain(body);
      const id = res[201][0]?.id;
      if (id) {
        if (!this.allowedDomains.find(value => value.id === id)) {
          const newAllowedDomainElement:AllowedDomain = {domain:newDomain.domain[0], id:id};
          this.allowedDomains = [...this.allowedDomains, newAllowedDomainElement]
          return true;
        }
      }
      this.errorAddEntity = "Domain already exists";
      return false;
    }
    catch(error:unknown){
      console.log(error);
      switch (error){
        default:
          this.errorAddEntity = 'Unknown error';
          break;
      }
      return false;
    }
  }
  async LoadAllAllowedDomains(){
    this.loading = true;
    try{
      this.allowedDomains = await loadAllAllowedDomains() as AllowedDomains;
    }
    finally{
      this.loading = false;
    }
  }
  async UpdateAllowedDomain(id:number){
    this.errorEditEntity = null;
    try{
      const body = JSON.stringify(this.allowedDomains.find(value => value.id === id));
      await updateAllowedDomain(body);
    }
    catch (error:unknown) {
      console.log(error);
      switch (error){
        case 400:
          this.errorEditEntity = 'Domain already exists or invalid';
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
  async DeleteAllowedDomain(id:number){
    try {
      return await deleteAllowedDomain(id)===204;
    }
    catch  {
      return false;
    }
  }
}
export const allowedDomainState = new AllowedDomainState();
import React from 'react';
export class BasePageStore{
  loading = false;
  errorEditEntity:string | null = null;
  errorAddEntity:string | null = null;
  editingId:number = -1;
  showDeleteDialogId: number = -1;
  addPopoverOpened: boolean = false;
  searchText:string = "";

  handleSearchChange = (e:React.ChangeEvent<HTMLInputElement>) => {
    this.searchText=e.target.value;
  }
  handleDeleteClick = (id:number) => {
    this.showDeleteDialogId = id;
  }
  handleNoClickAfterDeleteClick = () => {
    this.showDeleteDialogId = -1;
  }
  handlePlusClick = () => {
    this.addPopoverOpened = !this.addPopoverOpened;
  }
  handleCancelAddClick = () => {
    this.addPopoverOpened = false;
    this.errorAddEntity = null;
  }
}
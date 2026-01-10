import React from "react";

export interface LocalState  {
  errorAddEntity:string | null;
  addPopoverOpened:boolean;
  searchText:string;
  errorEditEntity:string | null;
  editingId:number;
  showDeleteDialogId:number;
  handlePlusClick:()=>void;
  handleSearchChange:(e:React.ChangeEvent<HTMLInputElement>)=>void;
  handleApplyClick:()=>void;
  handleEditClick:(id:number)=>void;
  handleCancelEditClick:()=>void;
  handleDeleteClick:(id:number)=>void;
  handleYesClickAfterDeleteClick:()=>void;
  handleNoClickAfterDeleteClick:()=>void;
}
import {BasePageStore} from "../../store/BasePageStore.tsx";
import {action, computed, makeObservable, observable} from "mobx";
import React from "react";
import {rootStore} from "../../store/RootStore.ts";
import {addUser, deleteUser, loadAllUsers, updateUser} from "../../services/users.api.ts";

export type User = {
  id: number;
  userName: string;
  fullName: string,
  enabled: boolean,
  isAdmin: boolean,
}
type NewUser = {
  userName: string;
  fullName: string,
  password: string;
  enabled: boolean,
  isAdmin: boolean,
}
type Users = User[];
class UsersState extends BasePageStore {
  users: Users = [];
  originalUser!: User;
  ChangePasswordPopoverOpened: boolean = false;
  changePasswordId:number| null = null;
  constructor() {
    super();
    makeObservable(this, {
      ChangePasswordPopoverOpened:observable,
      changePasswordId: observable,
      users: observable,
      originalUser: observable,
      loading: observable,
      errorEditEntity: observable,
      errorAddEntity: observable,
      editingId: observable,
      showDeleteDialogId: observable,
      addPopoverOpened: observable,
      searchText: observable,
      UsersFound: computed,
      handleSearchChange: action,
      handleEditClick: action,
      handleCancelEditClick: action,
      handleApplyClick: action,
      handleDeleteClick: action,
      handleYesClickAfterDeleteClick: action,
      handleNoClickAfterDeleteClick: action,
      handleCheckedEnabledChange: action,
      handleCheckedIsAdminChange:action,
      handleUserNameInputChange: action,
      handleFullNameInputChange:action,
      handlePlusClick: action,
      handleCancelAddClick: action,
      handleSaveClick: action,
      AddUser: action,
      LoadAllUsers: action,
      UpdateUser: action,
      DeleteUser: action,
      handleChangePassword: action,
      handleChangePasswordCancel: action,
      handlePasswordSaveClick: action,
    });
  }
  get UsersFound(){
    if (this.searchText)
      return this.users.filter(value => value.userName.includes(this.searchText));
    else
      return this.users;
  }
  handleChangePassword = (id:number) => {
    if (this.editingId>-1 || this.showDeleteDialogId>-1) return;
    this.ChangePasswordPopoverOpened = true;
    this.changePasswordId = id;
  }
  handleChangePasswordCancel = () => {
    this.ChangePasswordPopoverOpened = false;
    this.changePasswordId = null;
  }
  handleEditClick = (id:number) => {
    if (this.showDeleteDialogId>-1 || this.changePasswordId!==null) return;
    this.editingId = id;
    const element = this.users.find(value => value.id === id);
    if (element) {
      this.originalUser = element;
    }
  }
  handleDeleteClick = (id:number) => {
    if (this.editingId === -1 && this.changePasswordId === null) {
      this.showDeleteDialogId = id;
    }
  }
  handleCancelEditClick = () => {
    this.users = this.users.map(value => {
      if (value.id === this.editingId) return {...this.originalUser};
      else return value;
    });
    this.errorEditEntity = null;
    this.editingId = -1;
  }
  handleApplyClick = () => {
    this.UpdateUser(this.editingId).then(()=>{
      if (this.errorEditEntity === null) {
        this.editingId = -1
      }
    });
  }
  handlePasswordSaveClick = (id:number, userName: string, fullName:string, password:string, enabled: boolean, isAdmin:boolean) => {
    const newUser:NewUser = {userName, fullName, password, enabled, isAdmin};
    this.UpdateUser(id, newUser).then(()=>{
      if (this.errorEditEntity === null) {
        this.handleChangePasswordCancel();
      }
    });
  }

  handleYesClickAfterDeleteClick =()=>{
    this.DeleteUser(this.showDeleteDialogId).then((result) => {
      if (result) {
        this.users = this.users.filter(value => value.id !== this.showDeleteDialogId);
      }
      this.showDeleteDialogId = -1;
    })
  }

  handleCheckedEnabledChange = (id:number) => {
    this.users = this.users.map(value => {
      if (value.id === id) {
        return {...value, enabled:!value.enabled};
      }
      return value;
    });
  }
  handleCheckedIsAdminChange = (id:number) => {
    this.users = this.users.map(value => {
      if (value.id === id) {
        return {...value, isAdmin:!value.isAdmin};
      }
      return value;
    });
  }
  handleUserNameInputChange = (id:number, e:React.ChangeEvent<HTMLInputElement>) => {
    this.users = this.users.map(value => {
      if (value.id === id) {
        return {...value, userName:e.target.value};
      }
      return value;
    });
  }
  handleFullNameInputChange = (id:number, e:React.ChangeEvent<HTMLInputElement>) => {
    this.users = this.users.map(value => {
      if (value.id === id) {
        return {...value, fullName:e.target.value};
      }
      return value;
    });
  }

  handleSaveClick = (userName: string, fullName:string, password:string, enabled: boolean, isAdmin:boolean) => {
    const newUser:NewUser = {userName, fullName, password, enabled, isAdmin};
    this.AddUser(newUser).then((result)=>{this.addPopoverOpened = !result});
  }
  async AddUser(newUser:NewUser) {
    this.errorAddEntity = null;
    try {
      const body = JSON.stringify(newUser);
      const res: string = await addUser(body);
      const id = parseInt(res);
      if (!Number.isNaN(id)) {
        if (!this.users.find(value => value.id === id)) {
          const user:User = {...newUser, id:id};
          this.users = [...this.users, user]
          return true;
        }
      }
      this.errorAddEntity = "User already exists";
      return false;
    }
    catch(error:unknown){
      console.log(error);
      switch (error){
        case 403:
        case 401:
          this.users = [];
          rootStore.handleLogout();
          break;
        default:
          this.errorAddEntity = 'Unknown error';
          break;
      }
      return false;
    }
  }
  async LoadAllUsers(){
    this.loading = true;
    try{
      this.users = await loadAllUsers() as Users;
    }
    catch(error:unknown){
      switch (error){
        case 403:
        case 401:
          this.users = [];
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
  async UpdateUser(id:number, user:NewUser |null = null) {
    this.errorEditEntity = null;
    try{
      let body: string;
      if (user === null) {
        body = JSON.stringify(this.users.find(value => value.id === id));
      }
      else
        body = JSON.stringify({...user, id});
      await updateUser(body);
    }
    catch (error:unknown) {
      console.log(error);
      switch (error){
        case 403:
        case 401:
          this.users = [];
          rootStore.handleLogout();
          break;
        case 400:
          this.errorEditEntity = 'User data is invalid';
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
  async DeleteUser(id:number) {
    try {
      const code = await deleteUser(id);
      if (code === 401 || code === 403) {
        this.users = [];
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

export const usersState = new UsersState();
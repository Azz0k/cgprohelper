import {makeAutoObservable} from "mobx";
import {QueryClient} from '@tanstack/react-query';
import React from 'react';

export type LocalEmail = {
  id: number;
  email: string;
  isReplyAllowed:boolean;
}
type LocalEmails = LocalEmail[];
class LocalEmailsState {
  localEmails: LocalEmail[] = [];
  loading = false;
  error:string | null = null;
  editingId:number = -1;
  constructor() {
    makeAutoObservable(this);
  }
  handleEditClick = (id:number) => {
    this.editingId = id;
  }
  handleApplyClick = () => {
    this.editingId = -1;
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
  async LoadAllLocalEmails(){
    this.loading = true;
    const queryClient = new QueryClient();
    try{
      this.localEmails = await queryClient.fetchQuery({
        queryKey: ["localEmails", "get"],
        queryFn: () => fetch('http://localhost:64346/api/localmonitoredemails').then(res => res.json()),
        staleTime: 60_000,
      }) as LocalEmails;
    }
    finally{
      this.loading = false;
    }
  }
  async UpdateLocalEmail(id:number){
    const queryClient = new QueryClient();
    try{
      const body = JSON.stringify(this.localEmails.find(value => value.id === id));
      const response = await queryClient.fetchQuery({
        queryKey: ["localEmails", "put", id],
        queryFn: () => fetch('http://localhost:64346/api/localmonitoredemails/', {
          method: 'PUT',
          body: body,
        })
      });
    }
    finally {

    }
  }
}

export const localEmailsState = new LocalEmailsState();

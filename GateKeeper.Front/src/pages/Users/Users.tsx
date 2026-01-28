import {observer } from "mobx-react";
import { reaction } from "mobx";
import {useEffect} from "react";
import { Table } from 'antd';
import {AddElementAndSearch} from "../../components/AddElementAndSearch.tsx";
import {rootStore} from "../../store/RootStore.ts";
import {usersState} from "./UsersState.ts";
import {usersColumns} from "./components/usersColumns.tsx";

export const Users = observer(() => {
  reaction(
    ()=>rootStore.isLoggedIn,
    ()=>usersState.LoadAllUsers().then()
  );
  useEffect(()=>{
    usersState.LoadAllUsers().then()
  },[]);


  return(

    <div className='relative flex w-full h-full'>
      <Table
        loading={usersState.loading}
        size="small"
        className='w-full'
        dataSource={usersState.UsersFound}
        columns={usersColumns}
        rowKey={(record)=>record.id}
        scroll={{ y: window.innerHeight-300 }}
      />
      <AddElementAndSearch/>
    </div>
  );
});
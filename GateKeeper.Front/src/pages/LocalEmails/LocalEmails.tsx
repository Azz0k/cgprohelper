import {observer } from "mobx-react";
import { reaction } from "mobx";
import {useEffect} from "react";
import {localEmailsState} from "./LocalEmailsState.ts";
import { Table } from 'antd';
import {localEmailsColumns} from "./components/localEmailsColumns.tsx";
import {AddElementAndSearch} from "../../components/AddElementAndSearch.tsx";
import {rootStore} from "../../store/RootStore.ts";

export const LocalEmails = observer(() => {
  reaction(
    ()=>rootStore.isLoggedIn,
    ()=>localEmailsState.LoadAllLocalEmails().then()
  );
  useEffect(()=>{
    localEmailsState.LoadAllLocalEmails().then();
  },[]);


  return(

      <div className='relative flex w-full h-full'>
        <Table
          loading={localEmailsState.loading}
          size="small"
          className='w-full'
          dataSource={localEmailsState.LocalEmailsFound}
          columns={localEmailsColumns}
          rowKey={(record)=>record.id}
          scroll={{ y: window.innerHeight-300 }}
        />
        <AddElementAndSearch/>
      </div>
    );
});
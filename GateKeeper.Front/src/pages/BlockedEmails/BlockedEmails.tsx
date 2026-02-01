import {observer} from "mobx-react";
import {useEffect} from "react";

import { Table } from 'antd';
import {AddElementAndSearch} from "../../components/AddElementAndSearch.tsx";
import {rootStore} from "../../store/RootStore.ts";
import {reaction} from "mobx";
import {blockedEmailsColumns} from "./components/BlockedEmailsColumns.tsx";
import {blockedEmailsState} from "./BlockedEmailsState.ts";

export const BlockedEmails = observer(() => {

  useEffect(()=>{
    return   reaction(
      ()=>rootStore.isLoggedIn,
      ()=>{
        if (rootStore.isLoggedIn){
          blockedEmailsState.LoadAllBlockedEmails().then();
        }
      },
      { fireImmediately: true }
    );
  },[]);
console.log(blockedEmailsState.blockedEmailFound);
  return(
    <div className='relative flex w-full h-full'>
      <Table
        loading={blockedEmailsState.loading}
        size="middle"
        className='w-full'
        dataSource={blockedEmailsState.blockedEmailFound}
        columns={blockedEmailsColumns}
        rowKey={(record)=>record.id}
        pagination={false}
        scroll={{ y: window.innerHeight-300 }}
      />
      <AddElementAndSearch
        showAddElement={false}
        showSearchElement={true}
      />
    </div>
  );
});
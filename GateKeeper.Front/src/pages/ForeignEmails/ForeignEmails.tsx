import {observer} from "mobx-react";
import {useEffect} from "react";

import { Table } from 'antd';
import {AddElementAndSearch} from "../../components/AddElementAndSearch.tsx";
import {foreignEmailsState} from "./ForeignEmailsState.ts";
import {foreignEmailsColumns} from "../../components/ForeignEmailsColumns.tsx";
import {rootStore} from "../../store/RootStore.ts";
import {reaction} from "mobx";

export const ForeignEmails = observer(() => {
  reaction(
    ()=>rootStore.isLoggedIn,
    ()=>foreignEmailsState.LoadAllForeignEmails().then()
  );
  useEffect(()=>{
    foreignEmailsState.LoadAllForeignEmails().then()
  },[]);

  return(
    <div className='relative flex w-full h-full'>
      <Table
        loading={foreignEmailsState.loading}
        size="middle"
        className='w-full'
        dataSource={foreignEmailsState.foreignEmailFound}
        columns={foreignEmailsColumns}
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
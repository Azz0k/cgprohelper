import {observer} from "mobx-react";
import {useEffect} from "react";

import { Table } from 'antd';
import {AddElementAndSearch} from "../../components/AddElementAndSearch.tsx";
import {foreignEmailsState} from "./ForeignEmailsState.ts";
import {foreignEmailsColumns} from "../../components/ForeignEmailsColumns.tsx";

export const ForeignEmails = observer(() => {
  useEffect(()=>{
    foreignEmailsState.LoadAllForeignEmails().catch(()=>console.log("Error"));
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
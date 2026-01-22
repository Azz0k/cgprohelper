import {observer} from "mobx-react";
import {useEffect} from "react";
import {localEmailsState} from "./LocalEmailsState.ts";
import { Table } from 'antd';
import {localEmailsColumns} from "../../components/localEmailsColumns.tsx";
import {AddElementAndSearch} from "../../components/AddElementAndSearch.tsx";


export const LocalEmails = observer(() => {
  useEffect(()=>{
    localEmailsState.LoadAllLocalEmails().catch(()=>console.log("Error"));
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
import {observer} from "mobx-react";
import {useEffect} from "react";
import {localEmailsState} from "./LocalEmailsState.ts";
import { Table } from 'antd';
import {localEmailsColumns} from "../../components/localEmailsColumns.tsx";

export const LocalEmails = observer(() => {
  useEffect(()=>{
    localEmailsState.LoadAllLocalEmails().catch(()=>console.log("Error"));
  },[]);

  return(

      <div className='flex w-full'>
        {localEmailsState.loading && 'Loading...'}
        <Table
          className='w-full'
          dataSource={localEmailsState.localEmails}
          columns={localEmailsColumns}
          rowKey={(record)=>record.id}
        />
      </div>
    );
});
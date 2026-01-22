import {observer} from "mobx-react";
import {useEffect} from "react";
import {allowedDomainState} from "./AllowedDomainState.ts";
import { Table } from 'antd';
import {AddElementAndSearch} from "../../components/AddElementAndSearch.tsx";
import {allowedDomainsColumns} from "../../components/AllowedDomainsColumns.tsx";

export const AllowedDomains = observer(() => {
  useEffect(()=>{
    allowedDomainState.LoadAllAllowedDomains().catch(()=>console.log("Error"));
  },[]);

  return(

    <div className='relative flex w-full h-full'>
      <Table
        loading={allowedDomainState.loading}
        size="small"
        className='w-full'
        dataSource={allowedDomainState.AllowedDomainFound}
        columns={allowedDomainsColumns}
        rowKey={(record)=>record.id}
        scroll={{ y: window.innerHeight-300 }}
      />
      <AddElementAndSearch/>
    </div>
  );
});
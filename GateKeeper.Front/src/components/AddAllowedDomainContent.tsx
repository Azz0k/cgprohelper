import {observer} from "mobx-react";
import {Button, Input, Space} from "antd";
import {useState} from "react";
import {allowedDomainState} from "../pages/AllowedDomains/AllowedDomainState.ts";

export const AddAllowedDomainContent = observer(()=>{
  const [domain, setDomain] = useState('');
  return(
    <div className="flex">
      <Space size="small">
        <Input
          value={domain}
          placeholder="domain.com"
          onChange={(e)=>setDomain(e.target.value)}
        />
        <Button type="primary" onClick={()=>allowedDomainState.handleSaveClick(domain)}>
          Save
        </Button>
        <Button type="default" onClick={allowedDomainState.handleCancelAddClick}>
          Cancel
        </Button>
      </Space>
    </div>
  )
});
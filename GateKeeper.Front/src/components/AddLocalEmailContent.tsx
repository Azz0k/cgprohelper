import {observer} from "mobx-react";
import {Button, Checkbox, Input, Space} from "antd";
import {localEmailsState} from "../pages/localEmails/LocalEmailsState.ts";
import {useState} from "react";

export const AddLocalEmailContent = observer(()=>{
  const [email, setEmail] = useState(import.meta.env.VITE_DEFAULT_NEW_EMAIL??'');
  const [isReplyAllowed, setReplyAllowed] = useState(false);
  return(
    <div className="flex">
      <Space size="small">
        <Input
          value={email}
          onChange={(e)=>setEmail(e.target.value)}
        />
        <Checkbox
          checked={isReplyAllowed}
          onChange={()=>setReplyAllowed(!isReplyAllowed)}
          >
          Разрешен ли ответ на письма
        </Checkbox>
        <Button type="primary" onClick={()=>localEmailsState.handleSaveClick(email,isReplyAllowed)}>
          Save
        </Button>
        <Button type="default" onClick={localEmailsState.handleCancelAddClick}>
          Cancel
        </Button>
      </Space>
    </div>
  )
});
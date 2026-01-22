import {observer} from "mobx-react";
import {Button, Checkbox, Input, Space} from "antd";
import {localEmailsState} from "../pages/LocalEmails/LocalEmailsState.ts";
import {useState} from "react";

export const AddLocalEmailContent = observer(({inputRef})=>{
  const [email, setEmail] = useState('');
  const inputPlaceholder = `email${import.meta.env.VITE_DEFAULT_NEW_EMAIL??'@domain.com'}`;
  const [isReplyAllowed, setReplyAllowed] = useState(false);
  return(
    <div className="flex">
      <Space size="small">
        <Input
          value={email}
          ref={inputRef}
          placeholder={inputPlaceholder}
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
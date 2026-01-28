import {observer} from "mobx-react";
import {Button, Checkbox, Input, Space} from "antd";
import {useState} from "react";
import {usersState} from "../UsersState.ts";

export const AddUsersContent = observer(({inputRef})=>{
  const [userName, setUserName] = useState('');
  const [fullName, setFullName] = useState('');
  const [password, setPassword] = useState('');
  const [enabled, setEnabled] = useState(true);
  const [isAdmin, setIsAdmin] = useState(false);
  return(
    <div className="flex">
      <Space size="small">
        <Input
          value={userName}
          ref={inputRef}
          placeholder='User Name'
          onChange={(e)=>setUserName(e.target.value)}
        />
        <Input
          value={fullName}
          placeholder='Full Name'
          onChange={(e)=>setFullName(e.target.value)}
        />
        <Input
          type = "password"
          value={password}
          placeholder='Password'
          onChange={(e)=>setPassword(e.target.value)}
        />
        <Checkbox
          checked={enabled}
          onChange={()=>setEnabled(!enabled)}
        >
          Включен
        </Checkbox>
        <Checkbox
          checked={isAdmin}
          onChange={()=>setIsAdmin(!isAdmin)}
        >
          Админ
        </Checkbox>
        <Button type="primary" onClick={()=>usersState.handleSaveClick(userName, fullName, password, enabled, isAdmin)}>
          Save
        </Button>
        <Button type="default" onClick={usersState.handleCancelAddClick}>
          Cancel
        </Button>
      </Space>
    </div>
  )
});
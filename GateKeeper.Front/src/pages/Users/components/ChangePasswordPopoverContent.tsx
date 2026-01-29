import {observer} from "mobx-react";
import {Button, Input} from "antd";
import {useState} from "react";
import {type User, usersState} from "../UsersState.ts";

export const ChangePasswordPopoverContent = observer(({id}) =>{
  const [password, SetPassword] = useState('');
  const {userName, fullName, enabled, isAdmin} = usersState.users.find(u=>u.id===id) as User;
  return(
    <>
      <Input
        type = "password"
        value={password}
        placeholder='Enter new password'
        onChange={(e)=>SetPassword(e.target.value)}
      />
      <Button type="primary" onClick={()=>usersState.handlePasswordSaveClick(id, userName, fullName, password, enabled, isAdmin)}>
        Save
      </Button>
    </>
  );
});
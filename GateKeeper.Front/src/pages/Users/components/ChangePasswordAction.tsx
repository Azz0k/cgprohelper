import {usersState} from "../UsersState.ts";
import {observer} from "mobx-react";

export const ChangePasswordAction =observer(({id}) => {
  return(
    <a onClick={()=>usersState.handleChangePassword(id)}>Change password</a>
  )
});
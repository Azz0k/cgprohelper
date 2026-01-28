import {usersState} from "../UsersState.ts";

export const ChangePasswordAction = ({id}) => {
  return(
    <a onClick={()=>usersState.handleChangePassword(id)}>Change password</a>
  )
}
import {observer} from "mobx-react";
import {Popover} from "antd";
import {usersState} from "../UsersState.ts";

export const ChangePasswordPopover = observer(({id})=>{
  return(
    <Popover
      open={usersState.ChangePasswordPopoverOpened}
      title={usersState.errorEditEntity}
      placement='left'
      content={<div>change password</div>}
    >
      <span>
        <a onClick={()=>usersState.handleChangePasswordCancel()}>Cancel</a>
      </span>
    </Popover>
  );
});
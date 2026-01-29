import {observer} from "mobx-react";
import {Popover} from "antd";
import {usersState} from "../UsersState.ts";
import {ChangePasswordPopoverContent} from "./ChangePasswordPopoverContent.tsx";

export const ChangePasswordPopover = observer(({id})=>{
  console.log(id);
  return(
    <Popover
      open={usersState.ChangePasswordPopoverOpened}
      title={usersState.errorEditEntity}
      placement='left'
      content={<ChangePasswordPopoverContent id={id}/>}
    >
      <span>
        <a onClick={()=>usersState.handleChangePasswordCancel()}>Cancel</a>
      </span>
    </Popover>
  );
});
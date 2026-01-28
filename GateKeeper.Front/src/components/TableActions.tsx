import {observer} from "mobx-react";
import {Space, Popover } from "antd";
import {EditAction} from "./EditAction.tsx";
import {YesOrNoPopoverContent} from "./YesOrNoPopoverContent.tsx";
import {PopoverApplyAction} from "./PopoverApplyAction.tsx";
import {rootStore} from "../store/RootStore.ts";
import {ChangePasswordPopover} from "../pages/Users/components/ChangePasswordPopover.tsx";
import {usersState} from "../pages/Users/UsersState.ts";
import {ChangePasswordAction} from "../pages/Users/components/ChangePasswordAction.tsx";

export const TableActions =observer(({id, showEdit=true, showPassword=false})=> {
  let editAction = null;
  let changePasswordAction = null;
  if (showEdit){
    editAction = id===rootStore.localState.editingId?<PopoverApplyAction id={id}/>:<EditAction id={id}/>
  }
  if (showPassword)
  {
    changePasswordAction = id===usersState.changePasswordId?<ChangePasswordPopover id={id}/>:<ChangePasswordAction id={id}/>
  }

  return (
      <Space size="middle">
        {editAction}
        <Popover
          overlayStyle={{ width: 110 }}
          open={rootStore.localState.showDeleteDialogId===id}
          title='Are you sure?'
          content={<YesOrNoPopoverContent/>}
        >
          <a onClick={()=>rootStore.localState.handleDeleteClick(id)}>Delete</a>
        </Popover>
        {changePasswordAction}
      </Space>
  );
})
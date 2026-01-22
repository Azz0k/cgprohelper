import {observer} from "mobx-react";
import {Space, Popover } from "antd";
import {EditAction} from "./EditAction.tsx";
import {YesOrNoPopoverContent} from "./YesOrNoPopoverContent.tsx";
import {PopoverApplyAction} from "./PopoverApplyAction.tsx";
import {rootStore} from "../store/RootStore.ts";

export const TableActions =observer(({id, showEdit=true})=> {
  let editAction = null;
  if (showEdit){
    editAction = id===rootStore.localState.editingId?<PopoverApplyAction id={id}/>:<EditAction id={id}/>
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

      </Space>
  );
})
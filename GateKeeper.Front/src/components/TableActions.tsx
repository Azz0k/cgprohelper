import {observer} from "mobx-react";
import {Space, Popover } from "antd";
import {localEmailsState} from "../pages/localEmails/LocalEmailsState.ts";
import {EditAction} from "./EditAction.tsx";
import {YesOrNoPopoverContent} from "./YesOrNoPopoverContent.tsx";
import {PopoverApplyAction} from "./PopoverApplyAction.tsx";

export const TableActions =observer(({id})=> {
  return (
      <Space size="middle">
        {id===localEmailsState.editingId?<PopoverApplyAction id={id}/>:<EditAction id={id}/>}
        <Popover
          overlayStyle={{ width: 110 }}
          open={localEmailsState.showDeleteDialogId===id}
          title='Are you sure?'
          content={<YesOrNoPopoverContent/>}
        >
          <a onClick={()=>localEmailsState.handleDeleteClick(id)}>Delete</a>
        </Popover>

      </Space>
  );
})
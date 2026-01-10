import {observer} from "mobx-react";
import {Popover} from "antd";
import {localEmailsState} from "../pages/localEmails/LocalEmailsState.ts";
import {ApplyAction} from "./ApplyAction.tsx";

export const PopoverApplyAction = observer(({id})=>{
  return(
    <Popover
      open={localEmailsState.error !== null}
      title={localEmailsState.error}
      content={<a onClick={localEmailsState.handleCancelEditClick}>Cancel</a>}
    >
      <span>
        {id===localEmailsState.editingId && <ApplyAction/>}
      </span>
    </Popover>
  );
});
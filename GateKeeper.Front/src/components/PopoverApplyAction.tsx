import {observer} from "mobx-react";
import {Popover} from "antd";
import {ApplyAction} from "./ApplyAction.tsx";
import {rootStore} from "../store/RootStore.ts";

export const PopoverApplyAction = observer(({id})=>{
  return(
    <Popover
      open={rootStore.localState.errorEditEntity !== null}
      title={rootStore.localState.errorEditEntity}
      content={<a onClick={rootStore.localState.handleCancelEditClick}>Cancel</a>}
    >
      <span>
        {id===rootStore.localState.editingId && <ApplyAction/>}
      </span>
    </Popover>
  );
});
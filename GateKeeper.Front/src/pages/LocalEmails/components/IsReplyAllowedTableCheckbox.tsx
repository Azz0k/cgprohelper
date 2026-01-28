import {observer} from "mobx-react";
import {localEmailsState} from "../LocalEmailsState.ts";
import {Checkbox} from "antd";

export const IsReplyAllowedTableCheckbox = observer(({id, checked})=>{
  return (
    <Checkbox
      disabled={id!=localEmailsState.editingId}
      checked={checked}
      onChange={() => localEmailsState.handleCheckedChange(id)}
    />
  );
});
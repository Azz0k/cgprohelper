import {observer} from "mobx-react";
import {localEmailsState} from "../pages/localEmails/LocalEmailsState.ts";
import {Checkbox} from "antd";

export const TableCheckbox = observer(({id, checked})=>{
  return (
    <Checkbox
      disabled={id!=localEmailsState.editingId}
      checked={checked}
      onChange={() => localEmailsState.handleCheckedChange(id)}
    />
  );
});
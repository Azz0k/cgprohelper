import {observer} from "mobx-react";
import {Checkbox} from "antd";
import {usersState} from "../UsersState.ts";

export const EnabledTableCheckbox = observer(({id, checked})=>{
  return (
    <Checkbox
      disabled={id!=usersState.editingId}
      checked={checked}
      onChange={() => usersState.handleCheckedEnabledChange(id)}
    />
  );
});
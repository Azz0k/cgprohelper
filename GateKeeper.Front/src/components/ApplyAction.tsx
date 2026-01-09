import {observer} from "mobx-react";
import {localEmailsState} from "../pages/localEmails/LocalEmailsState.ts";

export const ApplyAction = observer(()=>{
  return(
    <a onClick={()=>localEmailsState.handleApplyClick()}>Apply</a>
  );
});
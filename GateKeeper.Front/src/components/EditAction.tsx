import {observer} from "mobx-react";
import {localEmailsState} from "../pages/localEmails/LocalEmailsState.ts";


export const EditAction = observer(({id})=>{
  return(
    <a onClick={()=>localEmailsState.handleEditClick(id)}>Edit</a>
  );
})
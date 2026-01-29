import {observer} from "mobx-react";
import {rootStore} from "../store/RootStore.ts";


export const EditAction = observer(({id})=>{
  return(
    <a
      onClick={()=>rootStore.localState.handleEditClick(id)}

    >
      Edit
    </a>
  );
})
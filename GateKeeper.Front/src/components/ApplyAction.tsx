import {observer} from "mobx-react";
import {rootStore} from "../store/RootStore.ts";

export const ApplyAction = observer(()=>{
  return(
    <a onClick={()=>rootStore.localState.handleApplyClick()}>Apply</a>
  );
});
import {observer} from "mobx-react";
import {Space} from "antd";
import {rootStore} from "../store/RootStore.ts";


export const YesOrNoPopoverContent = observer(()=>{
  return (
    <Space size="small" className="w-full flex justify-center">
      <a onClick={()=>rootStore.localState.handleYesClickAfterDeleteClick()}>Yes</a>
      <a onClick={()=>rootStore.localState.handleNoClickAfterDeleteClick()}>No</a>
    </Space>
  );
});
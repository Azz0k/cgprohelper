import {observer} from "mobx-react";
import {Space} from "antd";
import {localEmailsState} from "../pages/localEmails/LocalEmailsState.ts";


export const YesOrNoPopoverContent = observer(()=>{
  return (
    <Space size="small" className="w-full flex justify-center">
      <a onClick={()=>localEmailsState.handleYesClickAfterDeleteClick()}>Yes</a>
      <a onClick={()=>localEmailsState.handleNoClickAfterDeleteClick()}>No</a>
    </Space>
  );
});
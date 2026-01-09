import {observer} from "mobx-react";
import {Space} from "antd";
import {localEmailsState} from "../pages/localEmails/LocalEmailsState.ts";
import {ApplyAction} from "./ApplyAction.tsx";
import {EditAction} from "./EditAction.tsx";

export const TableActions =observer(({id})=> {
  return (
      <Space size="middle">
        {id===localEmailsState.editingId?<ApplyAction/>:<EditAction id={id}/>}
        <a>Delete</a>
      </Space>
  );
})
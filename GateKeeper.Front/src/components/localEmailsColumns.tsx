import type { TableProps } from 'antd';
import { Input } from 'antd';
import {Observer} from "mobx-react";
import {localEmailsState} from "../pages/localEmails/LocalEmailsState.ts";
import type { LocalEmail } from "../pages/localEmails/LocalEmailsState.ts"
import {TableActions} from "./TableActions.tsx";
import {TableCheckbox} from "./TableCheckbox.tsx";

export const localEmailsColumns:TableProps<LocalEmail>['columns'] = [
  {
    title: 'id',
    dataIndex: 'id',
    key: 'id',
    hidden:true,
  },
  {
    title: 'Локальный адрес',
    dataIndex: 'email',
    key: 'email',
    render: (_, record  ) =>(
      <Observer>
        {
          ()=>(
            <Input disabled={record.id!=localEmailsState.editingId}
                   value={record.email}
                   onChange={(e)=>localEmailsState.handleInputChange(record.id,e)}
            />
          )
        }
      </Observer>
    )
  },
  {
    title: 'Разрешен ли ответ на письма',
    dataIndex: 'isReplyAllowed',
    key: 'isReplyAllowed',
    render: (_, record) => (
      <TableCheckbox id={record.id} checked={record.isReplyAllowed}/>
    )
  },
  {
    title: 'Действия',
    key: 'action',
    render: (_, record) => (
      <TableActions id={record.id}/>
    ),
  },
];
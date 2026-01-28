import type { TableProps } from 'antd';
import { Input } from 'antd';
import {Observer} from "mobx-react";
import {localEmailsState} from "../LocalEmailsState.ts";
import type { LocalEmail } from "../LocalEmailsState.ts"
import {TableActions} from "../../../components/TableActions.tsx";
import {IsReplyAllowedTableCheckbox} from "./IsReplyAllowedTableCheckbox.tsx";

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
            <Input disabled={record.id != localEmailsState.editingId}
                   value={record.email}
                   onChange={(e) => localEmailsState.handleInputChange(record.id, e)}
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
    width: '10%',
    render: (_, record) => (
      <IsReplyAllowedTableCheckbox id={record.id} checked={record.isReplyAllowed}/>
    ),
    filters:[{text: 'Разрешен', value: true},{text: 'Запрещен', value: false}],
    onFilter: (value, record) =>  record.isReplyAllowed === value
  },
  {
    title: 'Действия',
    key: 'action',
    width: '10%',
    render: (_, record) => (
      <TableActions id={record.id}/>
    ),
  },
];
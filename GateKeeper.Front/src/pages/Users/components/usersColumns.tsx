import type { TableProps } from 'antd';
import { Input } from 'antd';
import {Observer} from "mobx-react";
import {TableActions} from "../../../components/TableActions.tsx";
import {type User, usersState} from "../UsersState.ts";
import {EnabledTableCheckbox} from "./EnabledTableCheckbox.tsx";
import {IsAdminTableCheckbox} from "./IsAdminTableCheckbox.tsx";

export const usersColumns:TableProps<User>['columns'] = [
  {
    title: 'id',
    dataIndex: 'id',
    key: 'id',
    hidden:true,
  },
  {
    title: 'Имя пользователя',
    dataIndex: 'userName',
    key: 'userName',
    render: (_, record  ) =>(
      <Observer>
        {
          ()=>(
            <Input disabled={record.id != usersState.editingId}
                   value={record.userName}
                   onChange={(e) => usersState.handleUserNameInputChange(record.id, e)}
            />
          )
        }
      </Observer>
    )
  },
  {
    title: 'ФИО',
    dataIndex: 'fullName',
    key: 'fullName',
    render: (_, record  ) =>(
      <Observer>
        {
          ()=>(
            <Input disabled={record.id != usersState.editingId}
                   value={record.fullName}
                   onChange={(e) => usersState.handleFullNameInputChange(record.id, e)}
            />
          )
        }
      </Observer>
    )
  },
  {
    title: 'Включен',
    dataIndex: 'enabled',
    key: 'enabled',
    width: '10%',
    render: (_, record) => (
      <EnabledTableCheckbox id={record.id} checked={record.enabled}/>
    ),
    filters:[{text: 'Включен', value: true},{text: 'Выключен', value: false}],
    onFilter: (value, record) =>  record.enabled === value
  },
  {
    title: 'Администратор',
    dataIndex: 'isAdmin',
    key: 'isAdmin',
    width: '10%',
    render: (_, record) => (
      <IsAdminTableCheckbox id={record.id} checked={record.isAdmin}/>
    ),
    filters:[{text: 'Да', value: true},{text: 'Нет', value: false}],
    onFilter: (value, record) =>  record.isAdmin === value
  },
  {
    title: 'Действия',
    key: 'action',
    width: '20%',
    render: (_, record) => (
      <TableActions id={record.id} showPassword={true}/>
    ),
  },
];
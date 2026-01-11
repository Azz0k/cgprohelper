import type { TableProps } from 'antd';
import type {ForeignEmail} from "../pages/ForeignEmails/ForeignEmailsState.ts";


export const foreignEmailsColumns:TableProps<ForeignEmail>['columns'] = [
  {
    title: 'id',
    dataIndex: 'id',
    key: 'id',
    hidden:true,
  },
  {
    title: 'Адрес, окуда пришло письмо',
    dataIndex: 'email',
    key: 'email',
  },
  {
    title: 'Дата, когда пришло письмо',
    dataIndex: 'receivedDate',
    key: 'receivedDate',
  },
];
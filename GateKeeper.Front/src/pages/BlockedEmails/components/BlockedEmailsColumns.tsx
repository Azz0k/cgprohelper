import type { TableProps } from 'antd';
import type {BlockedEmail} from "../BlockedEmailsState.ts";


export const blockedEmailsColumns:TableProps<BlockedEmail>['columns'] = [
  {
    title: 'id',
    dataIndex: 'id',
    key: 'id',
    hidden:true,
  },
  {
    title: 'Отправитель',
    dataIndex: 'senderEmail',
    key: 'senderEmail',
    width: '30%',
  },
  {
    title: 'Получатель',
    dataIndex: 'recipientEmail',
    key: 'recipientEmail',
    width: '30%',
  },
  {
    title: 'Дата',
    dataIndex: 'date',
    key: 'date',
    width: '20%',
  },
  {
    title: 'Время',
    dataIndex: 'time',
    key: 'time',
    width: '20%',
  },
];
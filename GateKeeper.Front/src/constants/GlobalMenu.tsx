import {ClusterOutlined, UserOutlined} from "@ant-design/icons";
import type {MenuItemType} from "antd/es/menu/interface";


export const GlobalMenu:MenuItemType[] = [
  {
    key: '1', icon: <ClusterOutlined />, label: 'Локальные адреса',
  },
  {
    key: '2', icon: <ClusterOutlined />, label: 'Удаленные адреса',
  },
  {
    key: '3', icon: <ClusterOutlined />, label: 'Разрешенные домены',
  },
  {
    key: '4', icon: <UserOutlined />,
    label: 'Пользователи',
  },
];



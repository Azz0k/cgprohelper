import type { TableProps } from 'antd';
import { Input } from 'antd';
import {Observer} from "mobx-react";
import {allowedDomainState} from "../AllowedDomainState.ts";
import type { AllowedDomain } from "../AllowedDomainState.ts"
import {TableActions} from "../../../components/TableActions.tsx";


export const allowedDomainsColumns:TableProps<AllowedDomain>['columns'] = [
  {
    title: 'id',
    dataIndex: 'id',
    key: 'id',
    hidden:true,
  },
  {
    title: 'Разрешенный домен или шаблон домена',
    dataIndex: 'domain',
    key: 'domain',
    render: (_, record  ) =>(
      <Observer>
        {
          ()=>(
            <Input disabled={record.id != allowedDomainState.editingId}
                   value={record.domain}
                   onChange={(e) => allowedDomainState.handleInputChange(record.id, e)}
            />
          )
        }
      </Observer>
    )
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
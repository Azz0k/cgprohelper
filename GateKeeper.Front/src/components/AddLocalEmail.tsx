import {observer} from "mobx-react";
import { Popover, Space} from "antd";
import {localEmailsState} from "../pages/localEmails/LocalEmailsState.ts";
import {AddLocalEmailContent} from "./AddLocalEmailContent.tsx";
import Search from "antd/es/input/Search";

export const AddLocalEmail = observer(()=>{
  return(
    <div className="absolute -top-17 right-5">
      <Popover
        title={localEmailsState.errorAddEmail}
        placement="left"
        open={localEmailsState.addPopoverOpened}
        content={<AddLocalEmailContent/>}
      >
        <span>
          <Space>
            <a className="text-6xl select-none" onClick={localEmailsState.handlePlusClick}>+</a>
            <Search
              className="mt-3"
              placeholder="Input search text"
              enterButton="Search"
              size="middle"
              value={localEmailsState.searchText}
              onChange={e=>localEmailsState.handleSearchChange(e)}
              allowClear
            />
          </Space>

        </span>
      </Popover>

    </div>
  )
});
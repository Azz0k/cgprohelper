import {observer} from "mobx-react";
import { Popover, Space} from "antd";
import {localEmailsState} from "../pages/LocalEmails/LocalEmailsState.ts";
import {AddLocalEmailContent} from "./AddLocalEmailContent.tsx";
import Search from "antd/es/input/Search";
import {rootStore} from "../store/RootStore.ts";
import {allowedDomainState} from "../pages/AllowedDomains/AllowedDomainState.ts";
import {AddAllowedDomainContent} from "./AddAllowedDomainContent.tsx";

export const AddElementAndSearch = observer(()=>{
  let state;
  let content;
  switch (rootStore.pathName){
    case "/": state = localEmailsState;
      content = <AddLocalEmailContent />;
    break;
    default: state = allowedDomainState;
    content = <AddAllowedDomainContent/>;
    break;
  }
  return(
    <div className="absolute -top-17 right-5">
      <Popover
        title={state.errorAddEntity}
        placement="left"
        open={state.addPopoverOpened}
        content={content}
      >
        <span>
          <Space>
            <a className="text-6xl select-none" onClick={state.handlePlusClick}>+</a>
            <Search
              className="mt-3"
              placeholder="Input search text"
              enterButton="Search"
              size="middle"
              value={state.searchText}
              onChange={e=>state.handleSearchChange(e)}
              allowClear
            />
          </Space>

        </span>
      </Popover>

    </div>
  )
});
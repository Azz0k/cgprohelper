import {observer} from "mobx-react";
import { Popover, Space} from "antd";
import {AddLocalEmailContent} from "./AddLocalEmailContent.tsx";
import Search from "antd/es/input/Search";
import {rootStore} from "../store/RootStore.ts";
import {AddAllowedDomainContent} from "./AddAllowedDomainContent.tsx";


export const AddElementAndSearch = observer(()=>{
  let content;
  switch (rootStore.pathName){
    case "/":
      content = <AddLocalEmailContent />;
    break;
    default:
    content = <AddAllowedDomainContent/>;
    break;
  }
  return(
    <div className="absolute -top-17 right-5">
      <Popover
        title={rootStore.localState.errorAddEntity}
        placement="left"
        open={rootStore.localState.addPopoverOpened}
        content={content}
      >
        <span>
          <Space>
            <a className="text-6xl select-none" onClick={rootStore.localState.handlePlusClick}>+</a>
            <Search
              className="mt-3"
              placeholder="Input search text"
              enterButton="Search"
              size="middle"
              value={rootStore.localState.searchText}
              onChange={e=>rootStore.localState.handleSearchChange(e)}
              allowClear
            />
          </Space>

        </span>
      </Popover>

    </div>
  )
});
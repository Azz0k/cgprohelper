import {observer} from "mobx-react";
import {type InputRef, Popover, Space} from "antd";
import {AddLocalEmailContent} from "../pages/LocalEmails/components/AddLocalEmailContent.tsx";
import Search from "antd/es/input/Search";
import {rootStore} from "../store/RootStore.ts";
import {AddAllowedDomainContent} from "../pages/AllowedDomains/components/AddAllowedDomainContent.tsx";
import {type RefObject, useRef} from "react";
import {AddUsersContent} from "../pages/Users/components/AddUsersContent.tsx";


export const AddElementAndSearch = observer(({showAddElement = true, showSearchElement =true})=>{
  const inputRef:RefObject<InputRef| null>  = useRef(null);
  let content;
  switch (rootStore.pathName){
    case "/":
      content = <AddLocalEmailContent inputRef={inputRef} />;
    break;
    case "/users":
      content = <AddUsersContent inputRef={inputRef} />;
      break;
    default:
    content = <AddAllowedDomainContent inputRef={inputRef} />;
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
            <a className='text-6xl select-none data-[invisibleAddElement="true"]:invisible'
               data-invisibleAddElement={!showAddElement}
               onClick={()=>{rootStore.localState.handlePlusClick(); queueMicrotask(()=>inputRef.current?.focus())}}
            >+</a>
            <Search
              data-invisibleSearchElement={!showSearchElement}
              className="mt-3 data-[invisibleSearchElement='true']:invisible"
              placeholder="Input search text"
              enterButton="Search"
              size="middle"
              value={rootStore.localState.searchText}
              onChange={e=>rootStore.localState.handleSearchChange(e)}
              allowClear
            />;
          </Space>

        </span>
      </Popover>

    </div>
  )
});
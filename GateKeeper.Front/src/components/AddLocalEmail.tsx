import {observer} from "mobx-react";
import {Popover} from "antd";
import {localEmailsState} from "../pages/localEmails/LocalEmailsState.ts";
import {AddLocalEmailContent} from "./AddLocalEmailContent.tsx";

export const AddLocalEmail = observer(()=>{
  return(
    <div className="absolute bottom-5 left-5">
      <Popover
        title={localEmailsState.errorAddEmail}
        placement="right"
        open={localEmailsState.addPopoverOpened}
        content={<AddLocalEmailContent/>}
      >
        <span>
          <a className="text-6xl select-none" onClick={localEmailsState.handlePlusClick}>+</a>
        </span>
      </Popover>

    </div>
  )
});
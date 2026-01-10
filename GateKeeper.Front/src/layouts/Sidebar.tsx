import {Layout, Menu} from "antd";
import {GlobalMenu} from "../constants/GlobalMenu.tsx";
import { observer } from "mobx-react";
import {rootStore} from "../store/RootStore.ts";

export const Sidebar = observer(()=>{
  const style = {
    display: "flex",
    justifyContent: "center",
    alignItems: "center",
    backgroundColor: "#001529",
    color: "#fff",
  }
  const { Sider } = Layout;

  return (
    <Sider style={style} width={250}>
      <Menu
        selectedKeys={[rootStore.globalMenuSelectedKey]}
        mode="inline"
        theme="dark"
        inlineCollapsed={false}
        items={GlobalMenu}
        onClick={rootStore.handleMenuSelected}
      />
    </Sider>
  )
});


import './App.css'
import { Layout, } from "antd";
import { configure } from 'mobx';
import { Sidebar } from "./layouts/Sidebar.tsx";

import { Router } from "./routes/Router.tsx";
import { RouterProvider } from '@tanstack/react-router'
import {observer} from "mobx-react";
import {rootStore} from "./store/RootStore.ts";
import {LoginForm} from "./pages/LoginForm.tsx";

configure({
    enforceActions: 'never',
});
const { Content, Header, Footer } = Layout;
const App = observer(() => {

    return (
        <>
            {rootStore.isLoggedIn ?(
            <Layout className="Container">

                <Header className="Header">GateKeeper</Header>
                <Layout className="Container">
                    <Sidebar />
                    <Content className="Content">
                        <RouterProvider router={Router} />
                    </Content>
                </Layout>
                <Footer className="Footer">version 0.5</Footer>
            </Layout>
              ):(
              <LoginForm/>
              )}
        </>
    )
});

export default App

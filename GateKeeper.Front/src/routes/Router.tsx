import { createRootRoute, createRoute, createRouter } from '@tanstack/react-router';
import {LocalEmails} from "../pages/localEmails/LocalEmails.tsx";

const rootRoute = createRootRoute();
const indexRoute = createRoute({
    getParentRoute: () => rootRoute,
    path: '/',
    component: ()=><LocalEmails/>,
});
const foreignEmailsRoute = createRoute({
    getParentRoute: () => rootRoute,
    path: '/foreignemails',
    component: () => <div>Foreign Emails</div>,
});
const allowedDomainsRoute = createRoute({
    getParentRoute: () => rootRoute,
    path: '/alloweddomains',
    component: () => <div>Allowed Domains</div>,
});
const routeTree = rootRoute.addChildren([indexRoute, foreignEmailsRoute, allowedDomainsRoute]);
const Router = createRouter({ routeTree });
export { Router }
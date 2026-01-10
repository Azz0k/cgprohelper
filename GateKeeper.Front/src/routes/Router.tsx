import { createRootRoute, createRoute, createRouter } from '@tanstack/react-router';
import {LocalEmails} from "../pages/LocalEmails/LocalEmails.tsx";
import {AllowedDomains} from "../pages/AllowedDomains/AllowedDomains.tsx";

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
    component: () => <AllowedDomains/>,
});
const routeTree = rootRoute.addChildren([indexRoute, foreignEmailsRoute, allowedDomainsRoute]);
const Router = createRouter({ routeTree });
export { Router }
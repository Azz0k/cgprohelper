import { createRootRoute, createRoute, createRouter } from '@tanstack/react-router';
import {LocalEmails} from "../pages/LocalEmails/LocalEmails.tsx";
import {AllowedDomains} from "../pages/AllowedDomains/AllowedDomains.tsx";
import {ForeignEmails} from "../pages/ForeignEmails/ForeignEmails.tsx";


const rootRoute = createRootRoute();
const indexRoute = createRoute({
    getParentRoute: () => rootRoute,
    path: '/',
    component: ()=><LocalEmails />,

});
const foreignEmailsRoute = createRoute({
    getParentRoute: () => rootRoute,
    path: '/foreignemails',
    component: () => <ForeignEmails/>,
});
const allowedDomainsRoute = createRoute({
    getParentRoute: () => rootRoute,
    path: '/alloweddomains',
    component: () => <AllowedDomains/>,
});
const usersRoute = createRoute({
    getParentRoute: () => rootRoute,
    path: '/users',
    component: () => <div> Users</div>,
});
const routeTree = rootRoute.addChildren([indexRoute, foreignEmailsRoute, allowedDomainsRoute,usersRoute]);
const Router = createRouter({ routeTree });
export { Router }
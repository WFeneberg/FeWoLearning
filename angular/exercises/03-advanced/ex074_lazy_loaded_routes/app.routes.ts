import { Routes } from "@angular/router";

// Exercise 074 — lazy-loaded routes (advanced).
// Goal:   keep the initial bundle small by only downloading a route's component (or a whole
//         subtree of routes) the moment a user actually navigates there.
// Drills: loadComponent, loadChildren, and the `() => import(...)` thenable shape a bundler
//         recognizes as a code-splitting boundary.
// Passes: when `npx jest exercises/03-advanced/ex074_lazy_loaded_routes` is green.
//
// `component` and `loadComponent` look almost interchangeable in a route object, but they mean
// very different things to the bundler. `component: Foo` requires Foo's module to already be
// loaded, so it and everything it imports end up in whatever chunk contains the route table.
// `loadComponent: () => import('./foo').then(m => m.Foo)` is a function — nothing runs until the
// router actually activates that route, and a bundler that sees the literal `import()` call treats
// it as a split point, putting Foo in its own chunk that only downloads when the function runs.
// `loadChildren` is the same idea one level up: instead of a single component, the thenable
// resolves to an entire `Routes` array, so a whole feature area (with its own nested routes) can be
// kept out of the main bundle.
//
// There is no real bundler here — Jest runs everything from node_modules/ts-jest through plain
// CommonJS require() calls, so there is no "chunk" to observe. What *is* real and worth testing is
// the shape of the route table: which paths are lazy, that the lazy functions actually resolve to
// the right component/routes when called, and that a route which should be eager (no lazy loading
// needed) is not saddled with a needless dynamic import.

/**
 * TODO: build the top-level route table.
 *   - ""      -> eager redirect to "panel" (pathMatch: "full") — no lazy loading, it's not a page.
 *   - "panel" -> loadComponent, lazily importing LazyPanelComponent from "./lazy-panel.component".
 *   - "admin" -> loadChildren, lazily importing ADMIN_ROUTES from "./admin.routes".
 */
export function buildAppRoutes(): Routes {
  throw new Error("TODO: implement buildAppRoutes");
}

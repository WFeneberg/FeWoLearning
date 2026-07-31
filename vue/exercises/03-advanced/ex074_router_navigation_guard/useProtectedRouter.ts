// Exercise 074 — useProtectedRouter composable (advanced).
// Goal:   implement a `beforeEnter` navigation guard on a protected route
//         that redirects to `/login` when a mock `isAuthenticated()`
//         returns false, and a tiny router that resolves navigations
//         through that guard (mirrors Vue Router's `beforeEnter` contract:
//         return `true` to allow, `false` to cancel, or a path to redirect).
// Drills: Vue Router navigation guards, guard composition, redirect
//         resolution, ref-based current-route state.
import { ref, type Ref } from "vue";

export interface RouteRecord {
  path: string;
  name: string;
  beforeEnter?: NavigationGuard;
}

/** Same shape as a Vue Router guard's return value: allow, cancel, or redirect. */
export type GuardResult = true | false | string;

export type NavigationGuard = (to: RouteRecord, from: RouteRecord) => GuardResult;

export interface ProtectedRouter {
  currentRoute: Ref<RouteRecord>;
  resolve: (path: string) => RouteRecord;
  push: (path: string) => RouteRecord;
}

/**
 * Creates a `beforeEnter` guard: redirects to `loginPath` whenever
 * `isAuthenticated()` returns false, and otherwise allows navigation.
 */
export function createAuthGuard(
  _isAuthenticated: () => boolean,
  _loginPath = "/login",
): NavigationGuard {
  throw new Error("TODO: implement createAuthGuard");
}

/**
 * A minimal router that resolves a target path by running any
 * `beforeEnter` guard on the matched route (following redirects).
 */
export function useProtectedRouter(
  _routes: RouteRecord[],
  _initialPath = "/",
): ProtectedRouter {
  throw new Error("TODO: implement useProtectedRouter");
}

// Exercise 074 — useProtectedRouter composable (reference solution).
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
  isAuthenticated: () => boolean,
  loginPath = "/login",
): NavigationGuard {
  return (to) => {
    if (to.path === loginPath) return true;
    return isAuthenticated() ? true : loginPath;
  };
}

/**
 * A minimal router that resolves a target path by running any
 * `beforeEnter` guard on the matched route (following redirects).
 */
export function useProtectedRouter(
  routes: RouteRecord[],
  initialPath = "/",
): ProtectedRouter {
  const findRoute = (path: string): RouteRecord => {
    const match = routes.find((route) => route.path === path);
    if (!match) {
      throw new Error(`No route matches path "${path}"`);
    }
    return match;
  };

  const currentRoute = ref(findRoute(initialPath)) as Ref<RouteRecord>;

  const resolve = (path: string): RouteRecord => {
    const from = currentRoute.value;
    let target = findRoute(path);
    const visited = new Set<string>();

    while (target.beforeEnter) {
      if (visited.has(target.path)) {
        throw new Error(`Redirect loop detected while resolving "${path}"`);
      }
      visited.add(target.path);

      const result = target.beforeEnter(target, from);
      if (result === true) break;
      if (result === false) {
        throw new Error(`Navigation to "${target.path}" was cancelled by a guard`);
      }
      target = findRoute(result);
    }

    return target;
  };

  const push = (path: string): RouteRecord => {
    const resolved = resolve(path);
    currentRoute.value = resolved;
    return resolved;
  };

  return { currentRoute, resolve, push };
}

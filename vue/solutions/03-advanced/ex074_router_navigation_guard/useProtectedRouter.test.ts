import { describe, expect, it, vi } from "vitest";
import { createAuthGuard, useProtectedRouter, type RouteRecord } from "./useProtectedRouter";

describe("router navigation guard", () => {
  const buildRoutes = (isAuthenticated: () => boolean): RouteRecord[] => [
    { path: "/", name: "home" },
    { path: "/login", name: "login" },
    {
      path: "/dashboard",
      name: "dashboard",
      beforeEnter: createAuthGuard(isAuthenticated),
    },
  ];

  it("redirects to the login route when unauthenticated", () => {
    const isAuthenticated = vi.fn(() => false);
    const router = useProtectedRouter(buildRoutes(isAuthenticated));

    const resolved = router.push("/dashboard");

    expect(resolved.name).toBe("login");
    expect(resolved.path).toBe("/login");
    expect(router.currentRoute.value.path).toBe("/login");
    expect(isAuthenticated).toHaveBeenCalledTimes(1);
  });

  it("allows navigation to the protected route when authenticated", () => {
    const isAuthenticated = vi.fn(() => true);
    const router = useProtectedRouter(buildRoutes(isAuthenticated));

    const resolved = router.push("/dashboard");

    expect(resolved.name).toBe("dashboard");
    expect(router.currentRoute.value.path).toBe("/dashboard");
  });

  it("does not re-run the guard when navigating directly to /login", () => {
    const isAuthenticated = vi.fn(() => false);
    const router = useProtectedRouter(buildRoutes(isAuthenticated));

    const resolved = router.push("/login");

    expect(resolved.name).toBe("login");
    expect(isAuthenticated).not.toHaveBeenCalled();
  });

  it("re-evaluates authentication on each navigation attempt", () => {
    let authenticated = false;
    const router = useProtectedRouter(buildRoutes(() => authenticated));

    expect(router.push("/dashboard").name).toBe("login");

    authenticated = true;
    expect(router.push("/dashboard").name).toBe("dashboard");
  });
});

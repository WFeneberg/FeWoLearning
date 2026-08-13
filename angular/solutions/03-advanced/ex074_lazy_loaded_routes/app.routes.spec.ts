import { Routes } from "@angular/router";
import { ADMIN_ROUTES } from "./admin.routes";
import { buildAppRoutes } from "./app.routes";
import { LazyPanelComponent } from "./lazy-panel.component";

describe("buildAppRoutes", () => {
  let routes: Routes;

  beforeEach(() => {
    routes = buildAppRoutes();
  });

  it("redirects the empty path to panel eagerly, with no lazy loading", () => {
    const root = routes.find((r) => r.path === "");

    expect(root?.redirectTo).toBe("panel");
    expect(root?.pathMatch).toBe("full");
    expect(root?.loadComponent).toBeUndefined();
    expect(root?.loadChildren).toBeUndefined();
  });

  it("declares the panel route as lazy, not eager", () => {
    const panelRoute = routes.find((r) => r.path === "panel");

    expect(panelRoute?.component).toBeUndefined();
    expect(typeof panelRoute?.loadComponent).toBe("function");
  });

  it("resolves the panel route's loadComponent to LazyPanelComponent", async () => {
    const panelRoute = routes.find((r) => r.path === "panel")!;

    const loaded = await panelRoute.loadComponent!();

    expect(loaded).toBe(LazyPanelComponent);
  });

  it("declares the admin route as lazy child routes", () => {
    const adminRoute = routes.find((r) => r.path === "admin");

    expect(adminRoute?.children).toBeUndefined();
    expect(typeof adminRoute?.loadChildren).toBe("function");
  });

  it("resolves the admin route's loadChildren to ADMIN_ROUTES", async () => {
    const adminRoute = routes.find((r) => r.path === "admin")!;

    const loaded = await adminRoute.loadChildren!();

    expect(loaded).toBe(ADMIN_ROUTES);
  });
});

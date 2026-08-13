import { Routes } from "@angular/router";

// Exercise 074 — lazy-loaded routes (reference solution).

export function buildAppRoutes(): Routes {
  return [
    { path: "", pathMatch: "full", redirectTo: "panel" },
    {
      path: "panel",
      loadComponent: () => import("./lazy-panel.component").then((m) => m.LazyPanelComponent),
    },
    {
      path: "admin",
      loadChildren: () => import("./admin.routes").then((m) => m.ADMIN_ROUTES),
    },
  ];
}

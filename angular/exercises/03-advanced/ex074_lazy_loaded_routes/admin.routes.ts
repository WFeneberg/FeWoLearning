import { Component } from "@angular/core";
import { Routes } from "@angular/router";

// Supporting infra for exercise 074 — a small child-route table, kept in its own module for the
// same reason as LazyPanelComponent: `loadChildren` needs something real to dynamically import.
@Component({
  selector: "app-admin-home",
  standalone: true,
  template: `<p class="admin-home">Admin home</p>`,
})
export class AdminHomeComponent {}

export const ADMIN_ROUTES: Routes = [{ path: "", component: AdminHomeComponent }];

import { Component } from "@angular/core";
import { Routes } from "@angular/router";

@Component({
  selector: "app-admin-home",
  standalone: true,
  template: `<p class="admin-home">Admin home</p>`,
})
export class AdminHomeComponent {}

export const ADMIN_ROUTES: Routes = [{ path: "", component: AdminHomeComponent }];

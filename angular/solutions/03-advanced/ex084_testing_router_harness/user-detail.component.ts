import { Component, inject } from "@angular/core";
import { ActivatedRoute } from "@angular/router";

// Exercise 084 — RouterTestingHarness (reference solution).

@Component({
  selector: "app-user-detail",
  standalone: true,
  template: `<p class="label">{{ label() }}</p>`,
})
export class UserDetailComponent {
  private readonly route = inject(ActivatedRoute);

  label(): string {
    const id = this.route.snapshot.paramMap.get("id");
    return `User ${id}`;
  }
}

@Component({
  selector: "app-not-found",
  standalone: true,
  template: `<p class="not-found">User not found</p>`,
})
export class NotFoundComponent {}

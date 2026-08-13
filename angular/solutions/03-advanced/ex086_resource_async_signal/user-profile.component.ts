import { Component, resource, signal } from "@angular/core";

// Exercise 086 — resource(): async loading as signal state (reference solution).

export interface UserRecord {
  readonly id: number;
  readonly name: string;
}

async function fetchUser(id: number): Promise<UserRecord> {
  if (id <= 0) {
    throw new Error(`No user with id ${id}`);
  }
  return { id, name: `User ${id}` };
}

@Component({
  selector: "app-user-profile",
  standalone: true,
  template: `
    <button type="button" class="next" (click)="nextUser()">Next user</button>
    @if (userResource.isLoading()) {
      <p class="loading">Loading…</p>
    } @else if (userResource.error()) {
      <p class="error">{{ userResource.error()?.message }}</p>
    } @else if (userResource.hasValue()) {
      <p class="name">{{ userResource.value().name }}</p>
    }
  `,
})
export class UserProfileComponent {
  readonly userId = signal(1);

  readonly userResource = resource({
    params: () => this.userId(),
    loader: ({ params }) => fetchUser(params),
  });

  nextUser(): void {
    this.userId.update((id) => id + 1);
  }
}

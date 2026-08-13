import { Injectable, Signal, inject, signal } from "@angular/core";
import { CanActivateFn, Router } from "@angular/router";

// Exercise 071 — a functional CanActivate guard (reference solution).

@Injectable({ providedIn: "root" })
export class AuthService {
  private readonly loggedIn = signal(false);
  readonly isAuthenticated: Signal<boolean> = this.loggedIn.asReadonly();

  logIn(): void {
    this.loggedIn.set(true);
  }

  logOut(): void {
    this.loggedIn.set(false);
  }
}

export const authGuard: CanActivateFn = (_route, state) => {
  const auth = inject(AuthService);
  if (auth.isAuthenticated()) {
    return true;
  }

  // A UrlTree, not a navigate() call — the router applies it as part of this navigation.
  const router = inject(Router);
  return router.createUrlTree(["/login"], { queryParams: { redirectTo: state.url } });
};

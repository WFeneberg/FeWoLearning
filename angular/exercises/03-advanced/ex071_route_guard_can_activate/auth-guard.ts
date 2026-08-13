import { Injectable, Signal, signal } from "@angular/core";
import { CanActivateFn } from "@angular/router";

// Exercise 071 — a functional CanActivate guard (advanced).
// Goal:   keep an unauthenticated user off a protected route, and send them somewhere useful.
// Drills: CanActivateFn, inject() inside a guard, returning a UrlTree instead of navigating by
//         hand, and RouterStateSnapshot.url as the "come back here" address.
// Passes: when `npx jest exercises/03-advanced/ex071_route_guard_can_activate` is green.
//
// A functional guard is just a function — no class, no module to register, just something exported
// and referenced in a route's `canActivate` array. Angular calls it inside an injection context, so
// inject() works exactly as it would in a component or service constructor.
//
// The guard's return value does double duty. `true` means "proceed"; a `UrlTree` means "go here
// instead" — and returning one is the modern replacement for calling `router.navigate()` and then
// returning `false`. The router applies the redirect itself, as part of the same navigation, rather
// than the guard triggering a second one — which matters because a guard-triggered navigate() can
// race the navigation that is still resolving.
//
// `state.url` is the URL the user actually asked for, not the guard's own route — carrying it as a
// query parameter is what lets the login page send them back to where they started rather than
// dumping them on a generic home screen.

@Injectable({ providedIn: "root" })
export class AuthService {
  private readonly loggedIn = signal(false);

  /** Read-only — nobody outside this service flips the flag directly. */
  readonly isAuthenticated: Signal<boolean> = this.loggedIn.asReadonly();

  logIn(): void {
    this.loggedIn.set(true);
  }

  logOut(): void {
    this.loggedIn.set(false);
  }
}

/**
 * TODO: implement the guard.
 *
 * Authenticated: return true. Otherwise: return a UrlTree to "/login", carrying the attempted
 * url as a `redirectTo` query parameter.
 */
export const authGuard: CanActivateFn = (_route, _state) => {
  throw new Error("TODO: implement authGuard");
};

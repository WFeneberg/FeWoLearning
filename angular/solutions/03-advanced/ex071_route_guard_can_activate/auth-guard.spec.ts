import { TestBed } from "@angular/core/testing";
import {
  ActivatedRouteSnapshot,
  RouterStateSnapshot,
  UrlTree,
  provideRouter,
} from "@angular/router";
import { Router } from "@angular/router";
import { AuthService, authGuard } from "./auth-guard";

const routeSnapshot = {} as ActivatedRouteSnapshot;
const stateFor = (url: string): RouterStateSnapshot => ({ url }) as RouterStateSnapshot;

describe("authGuard", () => {
  let auth: AuthService;
  let router: Router;

  beforeEach(() => {
    TestBed.configureTestingModule({ providers: [provideRouter([])] });
    auth = TestBed.inject(AuthService);
    router = TestBed.inject(Router);
  });

  const runGuard = (url: string) =>
    TestBed.runInInjectionContext(() => authGuard(routeSnapshot, stateFor(url)));

  it("lets an authenticated user through", () => {
    auth.logIn();

    expect(runGuard("/dashboard")).toBe(true);
  });

  it("redirects an unauthenticated user to /login", () => {
    const result = runGuard("/dashboard") as UrlTree;

    expect(result).toBeInstanceOf(UrlTree);
    expect(router.serializeUrl(result)).toBe("/login?redirectTo=%2Fdashboard");
  });

  it("carries the attempted url so login can send the user back", () => {
    const result = runGuard("/settings/profile") as UrlTree;

    expect(router.serializeUrl(result)).toBe("/login?redirectTo=%2Fsettings%2Fprofile");
  });

  it("revokes access again after logging out", () => {
    auth.logIn();
    expect(runGuard("/dashboard")).toBe(true);

    auth.logOut();

    expect(runGuard("/dashboard")).not.toBe(true);
  });

  it("does not authenticate as a side effect of merely being asked", () => {
    runGuard("/dashboard");

    expect(auth.isAuthenticated()).toBe(false);
  });
});

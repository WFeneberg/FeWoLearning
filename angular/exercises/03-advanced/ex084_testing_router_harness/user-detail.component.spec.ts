import { TestBed } from "@angular/core/testing";
import { Routes, provideRouter } from "@angular/router";
import { RouterTestingHarness } from "@angular/router/testing";
import { NotFoundComponent, UserDetailComponent } from "./user-detail.component";

const routes: Routes = [
  { path: "users/:id", component: UserDetailComponent },
  { path: "**", component: NotFoundComponent },
];

describe("UserDetailComponent (RouterTestingHarness)", () => {
  beforeEach(() => {
    TestBed.configureTestingModule({ providers: [provideRouter(routes)] });
  });

  it("navigates to a parameterized route and activates the routed component", async () => {
    const harness = await RouterTestingHarness.create();
    const component = await harness.navigateByUrl("/users/42", UserDetailComponent);

    expect(component.label()).toBe("User 42");
  });

  it("renders the routed component's label in the DOM after navigation", async () => {
    const harness = await RouterTestingHarness.create();
    await harness.navigateByUrl("/users/7", UserDetailComponent);
    harness.detectChanges();

    expect(harness.routeNativeElement?.textContent).toContain("User 7");
  });

  it("reuses the same harness for a second navigation, updating the routed component", async () => {
    const harness = await RouterTestingHarness.create();
    const first = await harness.navigateByUrl("/users/1", UserDetailComponent);
    expect(first.label()).toBe("User 1");

    const second = await harness.navigateByUrl("/users/2", UserDetailComponent);
    expect(second.label()).toBe("User 2");
  });

  it("falls back to the wildcard route, then still resolves a valid route afterward", async () => {
    const harness = await RouterTestingHarness.create();
    const notFound = await harness.navigateByUrl("/does/not/exist", NotFoundComponent);
    expect(notFound).toBeInstanceOf(NotFoundComponent);

    const user = await harness.navigateByUrl("/users/9", UserDetailComponent);
    expect(user.label()).toBe("User 9");
  });
});

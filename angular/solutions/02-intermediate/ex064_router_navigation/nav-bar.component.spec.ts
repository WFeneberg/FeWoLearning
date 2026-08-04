import { ComponentFixture, TestBed } from "@angular/core/testing";
import { Router } from "@angular/router";
import { NavBarComponent } from "./nav-bar.component";

interface NavigateCall {
  readonly commands: readonly unknown[];
  readonly extras?: Record<string, unknown>;
}

/**
 * A Router that records instead of navigating.
 *
 * Deliberately not the real thing: this exercise is about what the component *asks for*, and a
 * recording double makes the request itself the assertion. Exercise 084 drives a real router.
 */
class RecordingRouter {
  readonly navigateCalls: NavigateCall[] = [];
  readonly urlCalls: string[] = [];

  /** What the next navigation should resolve to. */
  nextResult = true;

  navigate(commands: readonly unknown[], extras?: Record<string, unknown>): Promise<boolean> {
    this.navigateCalls.push({ commands, extras });
    return Promise.resolve(this.nextResult);
  }

  navigateByUrl(url: string): Promise<boolean> {
    this.urlCalls.push(url);
    return Promise.resolve(this.nextResult);
  }
}

describe("NavBarComponent", () => {
  let fixture: ComponentFixture<NavBarComponent>;
  let component: NavBarComponent;
  let router: RecordingRouter;

  const text = (selector: string): string => {
    const found = fixture.nativeElement.querySelector(selector) as HTMLElement | null;
    if (found === null) {
      throw new Error(`no element matched "${selector}" — is the template implemented?`);
    }
    return found.textContent?.trim() ?? "";
  };

  const lastCall = (): NavigateCall => {
    const call = router.navigateCalls[router.navigateCalls.length - 1];
    if (call === undefined) {
      throw new Error("no navigation was requested");
    }
    return call;
  };

  beforeEach(async () => {
    router = new RecordingRouter();
    await TestBed.configureTestingModule({
      imports: [NavBarComponent],
      providers: [{ provide: Router, useValue: router }],
    }).compileComponents();
    fixture = TestBed.createComponent(NavBarComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it("navigates home", () => {
    component.goHome();

    expect(lastCall().commands).toEqual(["/"]);
  });

  it("passes an id as a command, not a built string", () => {
    component.goToProduct(42);

    // The array is what makes encoding the router's problem rather than yours.
    expect(lastCall().commands).toEqual(["/product", 42]);
  });

  it("keeps an awkward id intact", () => {
    component.goToProduct("a/b c");

    expect(lastCall().commands).toEqual(["/product", "a/b c"]);
  });

  it("records a successful navigation", async () => {
    component.goToProduct(1);
    await fixture.whenStable();

    expect(component.lastResult()).toBe("ok");
  });

  it("records a rejected navigation", async () => {
    router.nextResult = false;

    component.goToProduct(1);
    await fixture.whenStable();

    // A guard refused. Code that assumed success would now disagree with the URL.
    expect(component.lastResult()).toBe("rejected");
  });

  it("renders the last result", async () => {
    component.goToProduct(1);
    await fixture.whenStable();
    fixture.detectChanges();

    expect(text("p.last")).toBe("ok");
  });

  it("navigates from the buttons", () => {
    (fixture.nativeElement.querySelector("button.home") as HTMLButtonElement).click();
    expect(lastCall().commands).toEqual(["/"]);

    (fixture.nativeElement.querySelector("button.product") as HTMLButtonElement).click();
    expect(lastCall().commands).toEqual(["/product", 42]);
  });

  it("sends a query parameter", () => {
    component.search("chairs");

    expect(lastCall().commands).toEqual(["/search"]);
    expect(lastCall().extras?.["queryParams"]).toEqual({ q: "chairs" });
  });

  it("drops existing parameters by default", () => {
    component.search("chairs");

    // The default really is to discard them, which is how a navigation loses the user's filters.
    expect(lastCall().extras?.["queryParamsHandling"]).toBeUndefined();
  });

  it("merges when asked to keep them", () => {
    component.searchKeepingFilters("chairs");

    expect(lastCall().extras?.["queryParams"]).toEqual({ q: "chairs" });
    expect(lastCall().extras?.["queryParamsHandling"]).toBe("merge");
  });

  it("follows a URL string", () => {
    component.followUrl("/deep/link?x=1");

    expect(router.urlCalls).toEqual(["/deep/link?x=1"]);
    // navigateByUrl, so no command array was involved at all.
    expect(router.navigateCalls).toEqual([]);
  });

  it("keeps a page change on the current route", () => {
    component.goToPage(3);

    // Empty commands: stay where we are and change only the parameters.
    expect(lastCall().commands).toEqual([]);
  });

  it("merges and replaces for a page change", () => {
    component.goToPage(3);

    expect(lastCall().extras?.["queryParams"]).toEqual({ page: 3 });
    expect(lastCall().extras?.["queryParamsHandling"]).toBe("merge");
    expect(lastCall().extras?.["replaceUrl"]).toBe(true);
  });
});

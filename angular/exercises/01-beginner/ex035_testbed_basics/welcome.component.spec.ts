import { ComponentFixture, TestBed } from "@angular/core/testing";
import { Clock, WelcomeComponent } from "./welcome.component";

/** A clock the test controls completely, so assertions can be exact. */
class FakeClock implements Clock {
  calls = 0;
  private readonly times = ["2026-01-01T00:00:00.000Z", "2026-06-15T12:30:00.000Z"];

  now(): string {
    const time = this.times[Math.min(this.calls, this.times.length - 1)];
    this.calls += 1;
    return time;
  }
}

describe("WelcomeComponent", () => {
  let fixture: ComponentFixture<WelcomeComponent>;
  let component: WelcomeComponent;
  let clock: FakeClock;

  const text = (selector: string): string =>
    (fixture.nativeElement.querySelector(selector) as HTMLElement | null)?.textContent?.trim() ??
    `MISSING ${selector}`;

  beforeEach(async () => {
    clock = new FakeClock();
    await TestBed.configureTestingModule({
      imports: [WelcomeComponent],
      // The component asks for Clock; this decides what it gets.
      providers: [{ provide: Clock, useValue: clock }],
    }).compileComponents();
    fixture = TestBed.createComponent(WelcomeComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it("gives the component the provided fake, not the real class", () => {
    expect(TestBed.inject(Clock)).toBe(clock);
    expect(TestBed.inject(Clock)).not.toBeInstanceOf(Clock);

    // And the component really is wired to that instance, not merely coexisting with it.
    component.refresh();

    expect(clock.calls).toBe(1);
    expect(component.stamp()).toBe("2026-01-01T00:00:00.000Z");
  });

  it("renders through componentInstance state", () => {
    expect(component.greeting()).toBe("Hello, World!");
    expect(text("h2.greeting")).toBe("Hello, World!");
  });

  it("capitalises whatever name it is given", () => {
    component.name.set("ada");
    fixture.detectChanges();

    expect(text("h2.greeting")).toBe("Hello, Ada!");
  });

  it("does not re-render until detectChanges runs", () => {
    component.name.set("grace");

    expect(text("h2.greeting")).toBe("Hello, World!");

    fixture.detectChanges();

    expect(text("h2.greeting")).toBe("Hello, Grace!");
  });

  it("has not called the clock until asked", () => {
    expect(clock.calls).toBe(0);
    expect(component.stamp()).toBe("");
    expect(component.refreshes()).toBe(0);

    component.refresh();

    expect(clock.calls).toBe(1);
    expect(component.refreshes()).toBe(1);
  });

  it("reads the time from the injected clock", () => {
    component.refresh();

    expect(clock.calls).toBe(1);
    expect(component.stamp()).toBe("2026-01-01T00:00:00.000Z");
  });

  it("renders the stamp after change detection", () => {
    component.refresh();
    fixture.detectChanges();

    expect(text("p.stamp")).toBe("2026-01-01T00:00:00.000Z");
  });

  it("refreshes from the button", () => {
    (fixture.nativeElement.querySelector("button.refresh") as HTMLButtonElement).click();
    fixture.detectChanges();

    expect(component.refreshes()).toBe(1);
    expect(text("p.stamp")).toBe("2026-01-01T00:00:00.000Z");
  });

  it("gets a different time on the second refresh", () => {
    component.refresh();
    component.refresh();

    expect(component.refreshes()).toBe(2);
    expect(component.stamp()).toBe("2026-06-15T12:30:00.000Z");
  });

  it("exposes the host element as nativeElement", () => {
    const element = fixture.nativeElement as HTMLElement;

    // Worth knowing: TestBed bootstraps the component onto a synthetic <div>, so the host
    // element here is a DIV rather than the <app-welcome> a real application would render.
    // Host bindings still land on it (exercise 027), but its tag name is not the selector.
    expect(element.tagName).toBe("DIV");

    // The host is the element *around* the template, so the greeting is inside it.
    expect(element.querySelector("h2.greeting")?.textContent?.trim()).toBe("Hello, World!");
  });

  it("exposes the same instance through debugElement", () => {
    expect(fixture.debugElement.componentInstance).toBe(component);
    expect(fixture.debugElement.nativeElement).toBe(fixture.nativeElement);

    // Same object, so a change made through one is visible through the other.
    (fixture.debugElement.componentInstance as WelcomeComponent).refresh();

    expect(component.stamp()).toBe("2026-01-01T00:00:00.000Z");
  });

  it("resolves the component's own injector", () => {
    // Same injector the component used, so it hands back the same fake...
    expect(fixture.debugElement.injector.get(Clock)).toBe(clock);

    // ...and driving that instance directly is indistinguishable from the component's own
    // use of it, because there is only one.
    component.refresh();

    expect(fixture.debugElement.injector.get(Clock).now()).toBe("2026-06-15T12:30:00.000Z");
  });

  it("builds an independent component per createComponent call", () => {
    const second = TestBed.createComponent(WelcomeComponent);
    second.detectChanges();

    component.refresh();

    expect(second.componentInstance.refreshes()).toBe(0);
    // ...but both were given the one provided clock.
    expect(second.componentInstance.stamp()).toBe("");
    expect(clock.calls).toBe(1);
  });

  it("survives being destroyed", () => {
    component.refresh();
    fixture.destroy();

    expect(component.stamp()).toBe("2026-01-01T00:00:00.000Z");
  });
});

describe("WelcomeComponent with the real Clock", () => {
  let fixture: ComponentFixture<WelcomeComponent>;

  beforeEach(async () => {
    TestBed.resetTestingModule();
    // No providers at all: Clock is providedIn "root", so the real one is used.
    await TestBed.configureTestingModule({
      imports: [WelcomeComponent],
    }).compileComponents();
    fixture = TestBed.createComponent(WelcomeComponent);
    fixture.detectChanges();
  });

  it("falls back to the real dependency", () => {
    expect(TestBed.inject(Clock)).toBeInstanceOf(Clock);

    fixture.componentInstance.refresh();

    // A real timestamp landed, so the component is using it rather than ignoring it.
    expect(fixture.componentInstance.stamp()).not.toBe("");
    expect(fixture.componentInstance.refreshes()).toBe(1);
  });

  it("produces a parseable timestamp", () => {
    fixture.componentInstance.refresh();

    const stamp = fixture.componentInstance.stamp();

    // The real clock is non-deterministic, so assert the shape rather than the value —
    // which is exactly why the other suite fakes it.
    expect(Number.isNaN(Date.parse(stamp))).toBe(false);
    expect(stamp).toMatch(/^\d{4}-\d{2}-\d{2}T/);
  });
});

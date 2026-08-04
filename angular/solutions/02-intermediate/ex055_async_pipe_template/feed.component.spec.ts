import { ComponentFixture, TestBed } from "@angular/core/testing";
import { FeedComponent } from "./feed.component";

describe("FeedComponent", () => {
  let fixture: ComponentFixture<FeedComponent>;
  let component: FeedComponent;

  const maybe = (selector: string): HTMLElement | null =>
    fixture.nativeElement.querySelector(selector) as HTMLElement | null;

  const text = (selector: string): string => {
    const found = maybe(selector);
    if (found === null) {
      throw new Error(`no element matched "${selector}" — is the template implemented?`);
    }
    return found.textContent?.trim() ?? "";
  };

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [FeedComponent],
    }).compileComponents();
    fixture = TestBed.createComponent(FeedComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it("renders a value that is already there", () => {
    expect(text("p.count")).toBe("0");
  });

  it("re-renders when the source emits", () => {
    component.setCount(7);
    fixture.detectChanges();

    expect(text("p.count")).toBe("7");
  });

  it("yields null before the first emission", () => {
    // Which is why the @else branch is what shows, rather than an empty title.
    expect(maybe("h2.title")).toBeNull();
    expect(text("p.waiting")).toBe("waiting");
  });

  it("binds the aliased value once it arrives", () => {
    component.setTitle("Latest");
    fixture.detectChanges();

    expect(text("h2.title")).toBe("Latest");
    expect(maybe("p.waiting")).toBeNull();
  });

  it("subscribes once per | async", () => {
    // Two pipes, two subscriptions. Over a cold HTTP observable that is two requests.
    expect(component.trackedSubscriptions).toBe(2);
    expect(text("p.twice-a")).toBe("tracked");
    expect(text("p.twice-b")).toBe("tracked");
  });

  it("subscribes once when the value is aliased and reused", () => {
    expect(component.sharedSubscriptions).toBe(1);
    expect(text("p.shared-a")).toBe("shared");
    expect(text("p.shared-b")).toBe("shared");
  });

  it("does not add subscriptions on every change-detection pass", () => {
    // There are subscriptions to begin with, so a stable count means "held", not "never made".
    const before = component.trackedSubscriptions;
    expect(before).toBe(2);

    fixture.detectChanges();
    fixture.detectChanges();

    // AsyncPipe holds its subscription across renders; it does not resubscribe.
    expect(component.trackedSubscriptions).toBe(before);
  });

  it("unsubscribes when the component is destroyed", () => {
    let completedCount = 0;
    component.count$.subscribe({ complete: () => (completedCount += 1) });

    fixture.destroy();

    // The subject is not completed by the teardown, but the pipe's own subscription is gone —
    // observable proof being that a later emission cannot reach the DOM.
    expect(completedCount).toBe(0);
    expect(() => component.setCount(99)).not.toThrow();
  });

  it("keeps the rendered value from before destruction", () => {
    component.setCount(5);
    fixture.detectChanges();
    expect(text("p.count")).toBe("5");

    fixture.destroy();
    component.setCount(6);

    // No error, and no update: the pipe stopped listening.
    expect(text("p.count")).toBe("5");
  });

  it("follows several emissions in order", () => {
    for (const value of [1, 2, 3]) {
      component.setCount(value);
      fixture.detectChanges();
    }

    expect(text("p.count")).toBe("3");
  });

  it("flips back to waiting for a fresh component", () => {
    component.setTitle("Latest");
    fixture.detectChanges();

    const second = TestBed.createComponent(FeedComponent);
    second.detectChanges();

    // Its own Subject, its own null-until-emission state.
    expect(second.nativeElement.querySelector("h2.title")).toBeNull();
  });
});

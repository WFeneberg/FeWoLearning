import { ComponentFixture, TestBed } from "@angular/core/testing";
import { ClickTrackerComponent } from "./click-tracker.component";

describe("ClickTrackerComponent", () => {
  let fixture: ComponentFixture<ClickTrackerComponent>;
  let component: ClickTrackerComponent;

  const query = <T extends Element>(selector: string): T => {
    const found = fixture.nativeElement.querySelector(selector) as T | null;
    if (found === null) {
      throw new Error(`no element matched "${selector}" — is the template contract implemented?`);
    }
    return found;
  };

  /** Dispatch a bubbling click, optionally with modifier keys, and hand the event back. */
  const clickWith = (selector: string, init: MouseEventInit = {}): MouseEvent => {
    const event = new MouseEvent("click", { bubbles: true, cancelable: true, ...init });
    query(selector).dispatchEvent(event);
    return event;
  };

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [ClickTrackerComponent],
    }).compileComponents();
    fixture = TestBed.createComponent(ClickTrackerComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it("renders its starting state with nothing recorded", () => {
    expect(component.taps()).toBe(0);
    expect(component.outerTaps()).toBe(0);
    expect(component.modifiers()).toEqual([]);
    expect(query("p.taps").textContent).toContain("Taps: 0");
    expect(query("p.outer-taps").textContent).toContain("Outer: 0");
  });

  it("counts a plain click", () => {
    query<HTMLButtonElement>("button.tap").click();

    expect(component.taps()).toBe(1);
    expect(component.modifiers()).toEqual(["plain"]);
  });

  it("counts several clicks", () => {
    const button = query<HTMLButtonElement>("button.tap");
    button.click();
    button.click();

    expect(component.taps()).toBe(2);
    expect(component.modifiers()).toEqual(["plain", "plain"]);
  });

  it("reads shiftKey off the event", () => {
    clickWith("button.tap", { shiftKey: true });

    expect(component.modifiers()).toEqual(["shift"]);
  });

  it("reads ctrlKey off the event", () => {
    clickWith("button.tap", { ctrlKey: true });

    expect(component.modifiers()).toEqual(["ctrl"]);
  });

  it("prefers shift when both modifiers are held", () => {
    clickWith("button.tap", { shiftKey: true, ctrlKey: true });

    expect(component.modifiers()).toEqual(["shift"]);
  });

  it("renders the tap count", () => {
    query<HTMLButtonElement>("button.tap").click();
    fixture.detectChanges();

    expect(query("p.taps").textContent).toContain("1");
  });

  it("renders the modifier log", () => {
    clickWith("button.tap", { shiftKey: true });
    query<HTMLButtonElement>("button.tap").click();
    fixture.detectChanges();

    expect(query("p.modifiers").textContent).toContain("shift,plain");
  });

  it("cancels the link's navigation", () => {
    const event = clickWith("a.link");

    expect(event.defaultPrevented).toBe(true);
  });

  it("records a blocked link click without counting it as a tap", () => {
    clickWith("a.link");

    expect(component.taps()).toBe(0);
    expect(component.modifiers()).toEqual(["blocked"]);
  });

  it("counts a click that lands on the outer div", () => {
    clickWith("div.outer");

    expect(component.outerTaps()).toBe(1);
    expect(component.taps()).toBe(0);
  });

  it("keeps an inner click from reaching the outer handler", () => {
    clickWith("button.inner");

    expect(component.taps()).toBe(1);
    // Without stopPropagation() the click would bubble and the div would count it too.
    expect(component.outerTaps()).toBe(0);
  });

  it("renders the outer count", () => {
    clickWith("div.outer");
    fixture.detectChanges();

    expect(query("p.outer-taps").textContent).toContain("1");
  });

  it("clears everything on reset", () => {
    clickWith("button.tap", { shiftKey: true });
    clickWith("div.outer");
    query<HTMLButtonElement>("button.reset").click();

    expect(component.taps()).toBe(0);
    expect(component.outerTaps()).toBe(0);
    expect(component.modifiers()).toEqual([]);
  });
});

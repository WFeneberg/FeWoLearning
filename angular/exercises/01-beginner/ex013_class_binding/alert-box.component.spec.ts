import { ComponentFixture, TestBed } from "@angular/core/testing";
import { AlertBoxComponent } from "./alert-box.component";

describe("AlertBoxComponent", () => {
  let fixture: ComponentFixture<AlertBoxComponent>;
  let component: AlertBoxComponent;

  const query = <T extends Element>(selector: string): T => {
    const found = fixture.nativeElement.querySelector(selector) as T | null;
    if (found === null) {
      throw new Error(`no element matched "${selector}" — is the template contract implemented?`);
    }
    return found;
  };

  const classesOf = (selector: string): string[] =>
    Array.from(query(selector).classList).sort();

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [AlertBoxComponent],
    }).compileComponents();
    fixture = TestBed.createComponent(AlertBoxComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it("renders the message", () => {
    expect(query("div.alert").textContent).toContain("All good");
  });

  it("keeps the static class alongside the bindings", () => {
    component.severity.set("error");
    fixture.detectChanges();

    // A class binding merges with class="alert"; it does not replace it.
    expect(classesOf("div.alert")).toEqual(["alert", "error"]);
  });

  it("adds a class when its flag turns on", () => {
    expect(query("div.alert").classList.contains("error")).toBe(false);

    component.severity.set("error");
    fixture.detectChanges();

    expect(query("div.alert").classList.contains("error")).toBe(true);
  });

  it("removes a class when its flag turns off", () => {
    component.severity.set("warning");
    fixture.detectChanges();
    expect(query("div.alert").classList.contains("warning")).toBe(true);

    component.severity.set("info");
    fixture.detectChanges();

    expect(query("div.alert").classList.contains("warning")).toBe(false);
  });

  it("combines independent flags", () => {
    component.severity.set("error");
    component.dismissed.set(true);
    fixture.detectChanges();

    expect(classesOf("div.alert")).toEqual(["alert", "dismissed", "error"]);
  });

  it("builds the badge class set with every key present", () => {
    expect(component.badgeClasses()).toEqual({
      info: true,
      warning: false,
      error: false,
      pinned: false,
      muted: false,
    });
  });

  it("moves the true key with the severity", () => {
    component.severity.set("error");

    expect(component.badgeClasses()).toEqual({
      info: false,
      warning: false,
      error: true,
      pinned: false,
      muted: false,
    });
  });

  it("reflects the pinned and dismissed flags", () => {
    component.severity.set("warning");
    component.pinned.set(true);
    component.dismissed.set(true);

    expect(component.badgeClasses()).toEqual({
      info: false,
      warning: true,
      error: false,
      pinned: true,
      muted: true,
    });
  });

  it("applies the object through [class]", () => {
    component.severity.set("error");
    component.pinned.set(true);
    fixture.detectChanges();

    expect(classesOf("div.badge")).toEqual(["badge", "error", "pinned"]);
  });

  it("turns classes back off through [class]", () => {
    component.pinned.set(true);
    fixture.detectChanges();
    expect(query("div.badge").classList.contains("pinned")).toBe(true);

    component.pinned.set(false);
    fixture.detectChanges();

    expect(query("div.badge").classList.contains("pinned")).toBe(false);
  });

  it("applies the same object through [ngClass]", () => {
    component.severity.set("warning");
    component.dismissed.set(true);
    fixture.detectChanges();

    // Same result, but this one needed NgClass imported to work at all.
    expect(classesOf("div.legacy")).toEqual(["legacy", "muted", "warning"]);
  });

  it("keeps the two badges in step", () => {
    component.severity.set("error");
    fixture.detectChanges();

    const modern = Array.from(query("div.badge").classList).filter((c) => c !== "badge");
    const legacy = Array.from(query("div.legacy").classList).filter((c) => c !== "legacy");

    expect(modern.sort()).toEqual(legacy.sort());
    expect(modern).toContain("error");
  });
});

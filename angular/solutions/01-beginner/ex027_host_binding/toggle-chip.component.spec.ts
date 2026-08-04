import { ComponentFixture, TestBed } from "@angular/core/testing";
import { DecoratedChipComponent, ToggleChipComponent } from "./toggle-chip.component";

describe("ToggleChipComponent", () => {
  let fixture: ComponentFixture<ToggleChipComponent>;
  let component: ToggleChipComponent;
  /** The component's own element — `<app-toggle-chip>`, not anything in its template. */
  let host: HTMLElement;

  const classes = (): string[] => Array.from(host.classList).sort();

  const press = (key: string): void => {
    host.dispatchEvent(new KeyboardEvent("keydown", { key, bubbles: true }));
    fixture.detectChanges();
  };

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [ToggleChipComponent],
    }).compileComponents();
    fixture = TestBed.createComponent(ToggleChipComponent);
    component = fixture.componentInstance;
    host = fixture.nativeElement as HTMLElement;
    fixture.detectChanges();
  });

  it("renders its label inside the template", () => {
    expect(host.querySelector("span.label")?.textContent?.trim()).toBe("chip");
  });

  it("puts a static class on its own element", () => {
    expect(classes()).toEqual(["chip"]);
  });

  it("adds a class from state", () => {
    component.active.set(true);
    fixture.detectChanges();

    expect(classes()).toEqual(["active", "chip"]);
  });

  it("removes the class again", () => {
    component.active.set(true);
    fixture.detectChanges();
    component.active.set(false);
    fixture.detectChanges();

    expect(classes()).toEqual(["chip"]);
  });

  it("carries a static ARIA role", () => {
    expect(host.getAttribute("role")).toBe("button");
  });

  it("reflects state into aria-pressed", () => {
    expect(host.getAttribute("aria-pressed")).toBe("false");

    component.active.set(true);
    fixture.detectChanges();

    expect(host.getAttribute("aria-pressed")).toBe("true");
  });

  it("is focusable by default", () => {
    expect(host.getAttribute("tabindex")).toBe("0");
  });

  it("drops out of the tab order when disabled", () => {
    component.disabled.set(true);
    fixture.detectChanges();

    expect(host.getAttribute("tabindex")).toBe("-1");
    expect(classes()).toEqual(["chip", "disabled"]);
  });

  it("toggles when its own element is clicked", () => {
    host.click();
    fixture.detectChanges();

    expect(component.active()).toBe(true);
    expect(component.toggles()).toBe(1);
    expect(classes()).toEqual(["active", "chip"]);
  });

  it("toggles back on a second click", () => {
    host.click();
    host.click();

    expect(component.active()).toBe(false);
    expect(component.toggles()).toBe(2);
  });

  it("toggles on Enter", () => {
    press("Enter");

    expect(component.active()).toBe(true);
    expect(component.toggles()).toBe(1);
  });

  it("ignores other keys", () => {
    press("a");
    press("Escape");

    // (keydown.enter) filters the key for us — no manual event.key check to get wrong.
    expect(component.toggles()).toBe(0);

    // And Enter still works, so the zero above is filtering rather than a dead listener.
    press("Enter");
    expect(component.toggles()).toBe(1);
  });

  it("refuses to toggle while disabled", () => {
    // Prove the chip works first, so the zero below means "blocked", not "never wired".
    host.click();
    expect(component.toggles()).toBe(1);
    component.active.set(false);

    component.disabled.set(true);
    fixture.detectChanges();

    host.click();
    press("Enter");

    expect(component.active()).toBe(false);
    expect(component.toggles()).toBe(1);
  });
});

describe("DecoratedChipComponent", () => {
  let fixture: ComponentFixture<DecoratedChipComponent>;
  let component: DecoratedChipComponent;
  let host: HTMLElement;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [DecoratedChipComponent],
    }).compileComponents();
    fixture = TestBed.createComponent(DecoratedChipComponent);
    component = fixture.componentInstance;
    host = fixture.nativeElement as HTMLElement;
    fixture.detectChanges();
  });

  it("binds the static class through @HostBinding", () => {
    expect(host.classList.contains("chip")).toBe(true);
  });

  it("binds a stateful class through @HostBinding", () => {
    expect(host.classList.contains("active")).toBe(false);

    component.active.set(true);
    fixture.detectChanges();

    expect(host.classList.contains("active")).toBe(true);
  });

  it("binds an attribute through @HostBinding", () => {
    component.active.set(true);
    fixture.detectChanges();

    expect(host.getAttribute("aria-pressed")).toBe("true");
  });

  it("listens through @HostListener", () => {
    host.click();
    fixture.detectChanges();

    expect(component.active()).toBe(true);
    expect(component.toggles()).toBe(1);
  });

  it("reaches the same result as the host-metadata form", () => {
    host.click();
    fixture.detectChanges();

    // Two spellings, one behaviour.
    expect(host.classList.contains("chip")).toBe(true);
    expect(host.classList.contains("active")).toBe(true);
    expect(host.getAttribute("aria-pressed")).toBe("true");
  });
});

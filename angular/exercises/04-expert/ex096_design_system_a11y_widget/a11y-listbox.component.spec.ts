import { ComponentFixture, TestBed } from "@angular/core/testing";
import { A11yListboxComponent } from "./a11y-listbox.component";

describe("A11yListboxComponent (ARIA listbox pattern, keyboard navigation)", () => {
  let fixture: ComponentFixture<A11yListboxComponent>;
  let component: A11yListboxComponent;

  const OPTIONS = ["Alpha", "Bravo", "Charlie", "Delta"] as const;

  const listboxEl = (): HTMLElement | null => fixture.nativeElement.querySelector("ul.listbox");
  const optionEls = (): HTMLElement[] =>
    Array.from(fixture.nativeElement.querySelectorAll("li.option"));

  const press = (key: string): KeyboardEvent => {
    const event = new KeyboardEvent("keydown", { key, bubbles: true, cancelable: true });
    listboxEl()?.dispatchEvent(event);
    fixture.detectChanges();
    return event;
  };

  beforeEach(async () => {
    await TestBed.configureTestingModule({ imports: [A11yListboxComponent] }).compileComponents();
    fixture = TestBed.createComponent(A11yListboxComponent);
    component = fixture.componentInstance;
    fixture.componentRef.setInput("options", OPTIONS);
    fixture.detectChanges();
  });

  it("renders role=listbox on the container and role=option on each item", () => {
    expect(listboxEl()?.getAttribute("role")).toBe("listbox");
    expect(optionEls()).toHaveLength(4);
    for (const option of optionEls()) {
      expect(option.getAttribute("role")).toBe("option");
    }
  });

  it("points aria-activedescendant at the active option's id, and it tracks activeIndex", () => {
    expect(listboxEl()?.getAttribute("aria-activedescendant")).toBe(component.optionElementId(0));

    press("ArrowDown");

    expect(listboxEl()?.getAttribute("aria-activedescendant")).toBe(component.optionElementId(1));
  });

  it("ArrowDown moves the active option forward", () => {
    press("ArrowDown");
    expect(component.activeIndex()).toBe(1);

    press("ArrowDown");
    expect(component.activeIndex()).toBe(2);
  });

  it("ArrowUp does not overshoot below the first option", () => {
    press("ArrowDown");
    press("ArrowDown");
    expect(component.activeIndex()).toBe(2); // sanity check we actually moved first

    press("ArrowUp");
    press("ArrowUp");
    press("ArrowUp"); // one extra ArrowUp past the start

    expect(component.activeIndex()).toBe(0);
  });

  it("ArrowDown does not overshoot past the last option", () => {
    press("ArrowDown");
    press("ArrowDown");
    press("ArrowDown");
    press("ArrowDown");
    press("ArrowDown"); // one extra ArrowDown past the end

    expect(component.activeIndex()).toBe(OPTIONS.length - 1);
  });

  it("End jumps to the last option, Home jumps back to the first", () => {
    press("End");
    expect(component.activeIndex()).toBe(OPTIONS.length - 1);

    press("Home");
    expect(component.activeIndex()).toBe(0);
  });

  it("Enter selects the active option and reflects aria-selected on exactly that option", () => {
    press("ArrowDown");
    press("Enter");

    expect(component.selectedIndex()).toBe(1);
    const options = optionEls();
    expect(options[0]?.getAttribute("aria-selected")).toBe("false");
    expect(options[1]?.getAttribute("aria-selected")).toBe("true");
    expect(options[2]?.getAttribute("aria-selected")).toBe("false");
  });

  it("Space also selects the active option", () => {
    press("End");
    press(" ");

    expect(component.selectedIndex()).toBe(OPTIONS.length - 1);
  });

  it("clicking an option selects it directly", () => {
    optionEls()[2]?.dispatchEvent(new MouseEvent("click", { bubbles: true }));
    fixture.detectChanges();

    expect(component.selectedIndex()).toBe(2);
    expect(component.activeIndex()).toBe(2);
  });

  it("ignores unrelated keys, without disturbing a prior move", () => {
    press("ArrowDown");
    expect(component.activeIndex()).toBe(1); // sanity check the widget actually responds to keys

    press("a");
    press("Escape");

    expect(component.activeIndex()).toBe(1); // unrelated keys changed nothing
    expect(component.selectedIndex()).toBeNull();
  });

  it("calls preventDefault for keys it handles, but not for keys it ignores", () => {
    const handled = press("ArrowDown");
    expect(handled.defaultPrevented).toBe(true);

    const ignored = press("a");
    expect(ignored.defaultPrevented).toBe(false);
  });
});

import { ComponentFixture, TestBed } from "@angular/core/testing";
import {
  RepeatHostComponent,
  RepeatTimesDirective,
  UnlessDirective,
} from "./repeat-times.directive";

describe("RepeatTimesDirective", () => {
  let fixture: ComponentFixture<RepeatHostComponent>;
  let component: RepeatHostComponent;

  const rows = (): string[] =>
    Array.from(fixture.nativeElement.querySelectorAll("p.row") as NodeListOf<HTMLElement>).map(
      (node) => node.textContent?.trim() ?? "",
    );

  beforeEach(async () => {
    RepeatTimesDirective.viewsCreated = 0;
    UnlessDirective.viewsCreated = 0;
    await TestBed.configureTestingModule({
      imports: [RepeatHostComponent],
    }).compileComponents();
    fixture = TestBed.createComponent(RepeatHostComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it("stamps the template once per count", () => {
    expect(rows()).toHaveLength(3);
  });

  it("exposes the index through a context variable", () => {
    expect(rows()).toEqual(["0", "1", "2!"]);
  });

  it("marks only the last view", () => {
    component.count = 2;
    fixture.detectChanges();

    expect(rows()).toEqual(["0", "1!"]);
  });

  it("renders nothing for a count of zero", () => {
    // Rows exist first, so the emptiness below is the count taking effect rather than a
    // directive that never rendered anything.
    expect(rows()).toHaveLength(3);

    component.count = 0;
    fixture.detectChanges();

    expect(rows()).toEqual([]);
  });

  it("rebuilds rather than appending when the count changes", () => {
    expect(rows()).toHaveLength(3);

    component.count = 2;
    fixture.detectChanges();

    // Without clear() this would be 5 — the leak that looks like a list growing on its own.
    expect(rows()).toHaveLength(2);
  });

  it("grows correctly too", () => {
    component.count = 5;
    fixture.detectChanges();

    expect(rows()).toEqual(["0", "1", "2", "3", "4!"]);
  });

  it("creates exactly the views it renders", () => {
    // Three on the first pass, and nothing accumulated behind the scenes.
    expect(RepeatTimesDirective.viewsCreated).toBe(3);
    expect(rows()).toHaveLength(3);
  });

  it("refuses a negative count", () => {
    component.count = -1;

    expect(() => fixture.detectChanges()).toThrow(RangeError);
  });
});

describe("UnlessDirective", () => {
  let fixture: ComponentFixture<RepeatHostComponent>;
  let component: RepeatHostComponent;

  const maybe = (): HTMLElement | null =>
    fixture.nativeElement.querySelector("p.secret") as HTMLElement | null;

  beforeEach(async () => {
    RepeatTimesDirective.viewsCreated = 0;
    UnlessDirective.viewsCreated = 0;
    await TestBed.configureTestingModule({
      imports: [RepeatHostComponent],
    }).compileComponents();
    fixture = TestBed.createComponent(RepeatHostComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it("renders while the condition is false", () => {
    expect(maybe()).not.toBeNull();
    expect(maybe()?.textContent?.trim()).toBe("visible");
  });

  it("removes the view when the condition becomes true", () => {
    expect(maybe()).not.toBeNull();

    component.hidden = true;
    fixture.detectChanges();

    // Removed from the DOM, exactly as @if does — not merely hidden.
    expect(maybe()).toBeNull();
  });

  it("brings it back", () => {
    component.hidden = true;
    fixture.detectChanges();
    component.hidden = false;
    fixture.detectChanges();

    expect(maybe()).not.toBeNull();
  });

  it("creates one view, not one per render", () => {
    const before = UnlessDirective.viewsCreated;
    expect(before).toBe(1);

    fixture.detectChanges();
    fixture.detectChanges();

    // The setter runs on every pass; creating a view each time would duplicate the content.
    expect(UnlessDirective.viewsCreated).toBe(before);
  });

  it("creates a second view only after a genuine removal", () => {
    component.hidden = true;
    fixture.detectChanges();
    component.hidden = false;
    fixture.detectChanges();

    expect(UnlessDirective.viewsCreated).toBe(2);
  });
});

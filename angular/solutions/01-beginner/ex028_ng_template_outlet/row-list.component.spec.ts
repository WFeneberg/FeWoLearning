import { ComponentFixture, TestBed } from "@angular/core/testing";
import { RowListComponent } from "./row-list.component";

describe("RowListComponent", () => {
  let fixture: ComponentFixture<RowListComponent>;
  let component: RowListComponent;

  const texts = (selector: string): string[] =>
    Array.from(fixture.nativeElement.querySelectorAll(selector) as NodeListOf<HTMLElement>).map(
      (node) => node.textContent?.trim() ?? "",
    );

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [RowListComponent],
    }).compileComponents();
    fixture = TestBed.createComponent(RowListComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it("stamps the compact template once per row", () => {
    expect(texts(".rows span.compact")).toEqual(["0:alpha", "1:beta"]);
  });

  it("renders nothing from the unused definition", () => {
    // The compact definition is being stamped...
    expect(texts(".rows span.compact")).toHaveLength(2);

    // ...and the detailed one exists but is not, because an ng-template is a definition
    // rather than output. Only an outlet renders it.
    expect(texts(".rows span.detailed")).toEqual([]);
  });

  it("swaps which template the outlet stamps", () => {
    component.dense.set(false);
    fixture.detectChanges();

    expect(texts(".rows span.compact")).toEqual([]);
    expect(texts(".rows span.detailed")).toEqual([
      "0/2 alpha — first",
      "1/2 beta — second",
    ]);
  });

  it("follows the data", () => {
    component.items.set([{ name: "solo", note: "only" }]);
    fixture.detectChanges();

    expect(texts(".rows span.compact")).toEqual(["0:solo"]);
  });

  it("recomputes total for the detailed template", () => {
    component.dense.set(false);
    component.items.set([
      { name: "a", note: "x" },
      { name: "b", note: "y" },
      { name: "c", note: "z" },
    ]);
    fixture.detectChanges();

    expect(texts(".rows span.detailed")).toEqual([
      "0/3 a — x",
      "1/3 b — y",
      "2/3 c — z",
    ]);
  });

  it("builds a context with the item as $implicit", () => {
    const context = component.contextFor({ name: "alpha", note: "first" }, 0);

    expect(context["$implicit"]).toEqual({ name: "alpha", note: "first" });
  });

  it("includes the named context keys", () => {
    const context = component.contextFor({ name: "beta", note: "second" }, 1);

    expect(context["index"]).toBe(1);
    expect(context["total"]).toBe(2);
  });

  it("stamps the same definition again with fixed data", () => {
    // One <ng-template>, two outlets, different context each time.
    expect(texts(".preview span.compact")).toEqual(["0:sample"]);
  });

  it("keeps the preview independent of the list", () => {
    component.items.set([{ name: "changed", note: "x" }]);
    fixture.detectChanges();

    expect(texts(".rows span.compact")).toEqual(["0:changed"]);
    expect(texts(".preview span.compact")).toEqual(["0:sample"]);
  });

  it("builds the fixed preview context", () => {
    const context = component.previewContext();

    expect(context["$implicit"]).toEqual({ name: "sample", note: "n/a" });
    expect(context["index"]).toBe(0);
  });

  it("renders an empty list without complaint", () => {
    component.items.set([]);
    fixture.detectChanges();

    expect(texts(".rows span.compact")).toEqual([]);
    // The preview does not depend on the list, so it is still there.
    expect(texts(".preview span.compact")).toEqual(["0:sample"]);
  });
});

import { ComponentFixture, TestBed } from "@angular/core/testing";
import { FormArray } from "@angular/forms";
import { ChecklistFormComponent } from "./checklist-form.component";

describe("ChecklistFormComponent", () => {
  let fixture: ComponentFixture<ChecklistFormComponent>;
  let component: ChecklistFormComponent;

  const rows = (): HTMLElement[] =>
    Array.from(fixture.nativeElement.querySelectorAll("div.item") as NodeListOf<HTMLElement>);

  const rowLabels = (): string[] =>
    rows().map((row) => (row.querySelector("input.label") as HTMLInputElement).value);

  const text = (selector: string): string =>
    (fixture.nativeElement.querySelector(selector) as HTMLElement | null)?.textContent?.trim() ??
    `MISSING ${selector}`;

  const seed = (...labels: string[]): void => {
    for (const label of labels) {
      component.addItem(label);
    }
    fixture.detectChanges();
  };

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [ChecklistFormComponent],
    }).compileComponents();
    fixture = TestBed.createComponent(ChecklistFormComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it("starts as an empty array", () => {
    expect(component.items()).toBeInstanceOf(FormArray);
    expect(component.items().length).toBe(0);
    expect(component.form.value).toEqual({ title: "", items: [] });
  });

  it("appends a row as a group", () => {
    component.addItem("milk");

    expect(component.items().length).toBe(1);
    expect(component.itemAt(0).value).toEqual({ label: "milk", done: false });
  });

  it("keeps rows in insertion order", () => {
    seed("a", "b", "c");

    expect(component.labels()).toEqual(["a", "b", "c"]);
  });

  it("refuses a blank label", () => {
    expect(() => component.addItem("  ")).toThrow(RangeError);
    expect(component.items().length).toBe(0);
  });

  it("refuses an out-of-range itemAt", () => {
    seed("a");

    expect(() => component.itemAt(1)).toThrow(RangeError);
    expect(() => component.itemAt(-1)).toThrow(RangeError);
  });

  it("inserts in the middle", () => {
    seed("a", "c");

    component.insertItem(1, "b");

    expect(component.labels()).toEqual(["a", "b", "c"]);
  });

  it("inserts at the end", () => {
    seed("a");

    component.insertItem(1, "b");

    expect(component.labels()).toEqual(["a", "b"]);
  });

  it("refuses an out-of-range insert", () => {
    seed("a");

    expect(() => component.insertItem(5, "x")).toThrow(RangeError);
    expect(() => component.insertItem(-1, "x")).toThrow(RangeError);
  });

  it("removes a row and renumbers the rest", () => {
    seed("a", "b", "c");

    component.removeAt(0);

    // The index is the control name, so everything after a removal shifts down.
    expect(component.labels()).toEqual(["b", "c"]);
    expect(component.itemAt(0).value).toEqual({ label: "b", done: false });
  });

  it("refuses an out-of-range removal", () => {
    seed("a");

    expect(() => component.removeAt(1)).toThrow(RangeError);
  });

  it("ticks a row", () => {
    seed("a", "b");

    component.setDone(1, true);

    expect(component.doneCount()).toBe(1);
    expect(component.itemAt(1).value).toEqual({ label: "b", done: true });
    expect(component.itemAt(0).value).toEqual({ label: "a", done: false });
  });

  it("unticks a row", () => {
    seed("a");
    component.setDone(0, true);

    component.setDone(0, false);

    expect(component.doneCount()).toBe(0);
  });

  it("drops the ticked rows only", () => {
    seed("a", "b", "c", "d");
    component.setDone(1, true);
    component.setDone(3, true);

    component.clearDone();

    expect(component.labels()).toEqual(["a", "c"]);
    expect(component.doneCount()).toBe(0);
  });

  it("empties the whole array", () => {
    seed("a", "b");

    component.clearAll();

    expect(component.items().length).toBe(0);
    expect(component.form.value).toEqual({ title: "", items: [] });
  });

  it("renders one row per control", () => {
    seed("a", "b");

    expect(rows()).toHaveLength(2);
    expect(rowLabels()).toEqual(["a", "b"]);
  });

  it("re-renders after an insertion", () => {
    seed("a", "c");
    component.insertItem(1, "b");
    fixture.detectChanges();

    expect(rowLabels()).toEqual(["a", "b", "c"]);
  });

  it("keeps the same DOM node for a row that only moved", () => {
    seed("a", "b");
    const aRow = rows()[0];

    component.insertItem(0, "z");
    fixture.detectChanges();

    // Tracked by the FormGroup reference, which survives renumbering. Tracking $index here
    // would have rebuilt every row below the insertion.
    expect(rows()[1]).toBe(aRow);
  });

  it("takes a typed label back into the control", () => {
    seed("a");
    const input = rows()[0].querySelector("input.label") as HTMLInputElement;
    input.value = "milk";
    input.dispatchEvent(new Event("input"));

    expect(component.labels()).toEqual(["milk"]);
  });

  it("takes a ticked checkbox back into the control", () => {
    seed("a");
    const box = rows()[0].querySelector("input.done") as HTMLInputElement;
    box.checked = true;
    box.dispatchEvent(new Event("change"));

    expect(component.doneCount()).toBe(1);
  });

  it("renders the counts", () => {
    seed("a", "b", "c");
    component.setDone(0, true);
    fixture.detectChanges();

    expect(text("p.count")).toBe("1/3");
  });

  it("counts controls, not values, when a row is disabled", () => {
    seed("a", "b");

    component.itemAt(1).disable();

    // Two controls, one value. This is the disagreement to be aware of.
    expect(component.items().length).toBe(2);
    expect((component.items().value as unknown[]).length).toBe(1);
  });
});

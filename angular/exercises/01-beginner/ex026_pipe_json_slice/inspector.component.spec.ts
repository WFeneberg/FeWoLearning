import { ComponentFixture, TestBed } from "@angular/core/testing";
import { InspectorComponent } from "./inspector.component";

describe("InspectorComponent", () => {
  let fixture: ComponentFixture<InspectorComponent>;
  let component: InspectorComponent;

  const query = <T extends HTMLElement>(selector: string): T => {
    const found = fixture.nativeElement.querySelector(selector) as T | null;
    if (found === null) {
      throw new Error(`no element matched "${selector}" — is the template contract implemented?`);
    }
    return found;
  };

  const text = (selector: string): string => query(selector).textContent?.trim() ?? "";

  const items = (selector: string): string[] =>
    Array.from(fixture.nativeElement.querySelectorAll(selector) as NodeListOf<HTMLElement>).map(
      (node) => node.textContent?.trim() ?? "",
    );

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [InspectorComponent],
    }).compileComponents();
    fixture = TestBed.createComponent(InspectorComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it("dumps an object as indented JSON", () => {
    expect(text("pre.json")).toBe(JSON.stringify(component.config(), null, 2));
  });

  it("re-dumps when the object changes", () => {
    component.config.set({ name: "worker", retries: 0, debug: true });
    fixture.detectChanges();

    expect(text("pre.json")).toContain('"name": "worker"');
    expect(text("pre.json")).toContain('"debug": true');
  });

  it("slices the front of an array", () => {
    expect(text("p.first-two")).toBe("alpha,beta");
  });

  it("slices from the end with a negative index", () => {
    expect(text("p.last-two")).toBe("gamma,delta");
  });

  it("follows a change to the array", () => {
    component.tags.set(["one", "two", "three"]);
    fixture.detectChanges();

    expect(text("p.first-two")).toBe("one,two");
    expect(text("p.last-two")).toBe("two,three");
  });

  it("slices a string and chains another pipe", () => {
    expect(text("p.initials")).toBe("ANG");
  });

  it("re-slices a changed string", () => {
    component.title.set("signals");
    fixture.detectChanges();

    expect(text("p.initials")).toBe("SIG");
  });

  it("sorts keyvalue entries by key by default", () => {
    // The object was built as zoe, adam, mia — keyvalue does not keep that order.
    expect(items("li.entry")).toEqual(["adam=90", "mia=70", "zoe=40"]);
  });

  it("ranks by value with a comparator", () => {
    expect(items("li.ranked-entry")).toEqual(["adam=90", "mia=70", "zoe=40"]);
  });

  it("ranks a differently ordered object correctly", () => {
    component.scores.set({ ann: 10, bob: 95, cy: 55 });
    fixture.detectChanges();

    expect(items("li.entry")).toEqual(["ann=10", "bob=95", "cy=55"]);
    // Same data, and now the two lists genuinely differ.
    expect(items("li.ranked-entry")).toEqual(["bob=95", "cy=55", "ann=10"]);
  });

  it("compares two entries directly", () => {
    const high = { key: "a", value: 90 };
    const low = { key: "b", value: 10 };

    expect(component.byValueDescending(high, low)).toBeLessThan(0);
    expect(component.byValueDescending(low, high)).toBeGreaterThan(0);
    expect(component.byValueDescending(high, { key: "c", value: 90 })).toBe(0);
  });

  it("keeps the comparator stable across renders", () => {
    const first = component.byValueDescending;
    fixture.detectChanges();

    // A fresh function each render would make the pipe re-sort every pass.
    expect(component.byValueDescending).toBe(first);
    expect(component.byValueDescending({ key: "a", value: 2 }, { key: "b", value: 1 }))
      .toBeLessThan(0);
  });

  it("summarises the tags in plain TypeScript", () => {
    expect(component.summary()).toBe("alpha, beta, gamma, delta");
  });

  it("re-summarises changed tags", () => {
    component.tags.set(["solo"]);

    expect(component.summary()).toBe("solo");
  });
});

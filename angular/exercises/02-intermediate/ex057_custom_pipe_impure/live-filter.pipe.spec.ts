import { ComponentFixture, TestBed } from "@angular/core/testing";
import {
  LiveFilterHostComponent,
  LiveFilterPipe,
  StaticFilterPipe,
} from "./live-filter.pipe";

describe("the pipes in isolation", () => {
  beforeEach(() => {
    LiveFilterPipe.calls = 0;
    StaticFilterPipe.calls = 0;
  });

  it("filters case-insensitively", () => {
    const pipe = new LiveFilterPipe();

    expect(pipe.transform(["Apple", "banana"], "A")).toBe("Apple, banana");
  });

  it("keeps only the matches", () => {
    const pipe = new LiveFilterPipe();

    expect(pipe.transform(["apple", "banana", "cherry"], "an")).toBe("banana");
  });

  it("returns nothing when nothing matches", () => {
    expect(new LiveFilterPipe().transform(["apple"], "zzz")).toBe("");
  });

  it("keeps everything for an empty term", () => {
    expect(new LiveFilterPipe().transform(["a", "b"], "")).toBe("a, b");
  });

  it("agrees with the pure version", () => {
    const items = ["apple", "banana", "cherry"];

    expect(new LiveFilterPipe().transform(items, "a")).toBe(
      new StaticFilterPipe().transform(items, "a"),
    );
  });
});

describe("purity in a template", () => {
  let fixture: ComponentFixture<LiveFilterHostComponent>;
  let component: LiveFilterHostComponent;

  const text = (selector: string): string => {
    const found = fixture.nativeElement.querySelector(selector) as HTMLElement | null;
    if (found === null) {
      throw new Error(`no element matched "${selector}" — is the template implemented?`);
    }
    return found.textContent?.trim() ?? "";
  };

  beforeEach(async () => {
    LiveFilterPipe.calls = 0;
    StaticFilterPipe.calls = 0;
    await TestBed.configureTestingModule({
      imports: [LiveFilterHostComponent],
    }).compileComponents();
    fixture = TestBed.createComponent(LiveFilterHostComponent);
    component = fixture.componentInstance;
    component.computedCalls = 0;
    fixture.detectChanges();
  });

  it("all three agree on the answer", () => {
    expect(text("p.impure")).toBe("apple, banana");
    expect(text("p.pure")).toBe("apple, banana");
    expect(text("p.computed")).toBe("apple, banana");
  });

  it("re-runs the impure pipe on every pass", () => {
    const before = LiveFilterPipe.calls;

    fixture.detectChanges();
    fixture.detectChanges();
    fixture.detectChanges();

    // Three renders, three more calls, with nothing having changed. This is the cost.
    expect(LiveFilterPipe.calls).toBeGreaterThanOrEqual(before + 3);
  });

  it("does not re-run the pure pipe on those passes", () => {
    const before = StaticFilterPipe.calls;
    expect(before).toBeGreaterThan(0);

    fixture.detectChanges();
    fixture.detectChanges();
    fixture.detectChanges();

    expect(StaticFilterPipe.calls).toBe(before);
  });

  it("does not re-run the computed on those passes", () => {
    const before = component.computedCalls;
    expect(before).toBeGreaterThan(0);

    fixture.detectChanges();
    fixture.detectChanges();

    // Memoised on its dependencies, so it costs nothing when they have not moved.
    expect(component.computedCalls).toBe(before);
  });

  it("picks up an in-place mutation in the impure pipe", () => {
    component.pushInPlace("avocado");
    fixture.detectChanges();

    // The problem impurity solves — and it solves it by redoing the work every single pass.
    expect(text("p.impure")).toBe("apple, banana, avocado");
  });

  it("misses an in-place mutation in the pure pipe", () => {
    component.pushInPlace("avocado");
    fixture.detectChanges();

    expect(text("p.pure")).toBe("apple, banana");
  });

  it("all three follow a term change", () => {
    component.term.set("an");
    fixture.detectChanges();

    expect(text("p.impure")).toBe("banana");
    expect(text("p.pure")).toBe("banana");
    expect(text("p.computed")).toBe("banana");
  });

  it("re-runs the computed only for a real dependency change", () => {
    const before = component.computedCalls;

    component.term.set("an");
    fixture.detectChanges();
    fixture.detectChanges();

    // One recalculation for the change, none for the extra render.
    expect(component.computedCalls).toBe(before + 1);
  });

  it("costs the impure pipe far more calls than the alternatives", () => {
    for (const _ of [1, 2, 3, 4, 5]) {
      fixture.detectChanges();
    }

    expect(LiveFilterPipe.calls).toBeGreaterThan(StaticFilterPipe.calls);
    expect(LiveFilterPipe.calls).toBeGreaterThan(component.computedCalls);
  });
});

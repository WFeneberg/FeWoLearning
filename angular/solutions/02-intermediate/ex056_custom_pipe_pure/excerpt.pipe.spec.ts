import { ComponentFixture, TestBed } from "@angular/core/testing";
import { ExcerptHostComponent, ExcerptPipe } from "./excerpt.pipe";

describe("ExcerptPipe", () => {
  let pipe: ExcerptPipe;

  beforeEach(() => {
    ExcerptPipe.calls = 0;
    pipe = new ExcerptPipe();
  });

  it("leaves a short string alone", () => {
    expect(pipe.transform("hello")).toBe("hello");
  });

  it("leaves a string of exactly the limit alone", () => {
    expect(pipe.transform("0123456789")).toBe("0123456789");
  });

  it("cuts a long string", () => {
    expect(pipe.transform("0123456789abc")).toBe("0123456789…");
  });

  it("honours a custom length", () => {
    expect(pipe.transform("0123456789", 4)).toBe("0123…");
  });

  it("honours a custom suffix", () => {
    expect(pipe.transform("0123456789", 4, "...")).toBe("0123...");
  });

  it("refuses a length below one", () => {
    expect(() => pipe.transform("abc", 0)).toThrow(RangeError);
    expect(() => pipe.transform("abc", -1)).toThrow(RangeError);
  });

  it("joins an array first", () => {
    expect(pipe.transform(["one", "two"], 20)).toBe("one, two");
  });

  it("cuts a joined array", () => {
    expect(pipe.transform(["one", "two"], 5)).toBe("one, …");
  });

  it("counts its calls", () => {
    pipe.transform("a");
    pipe.transform("b");

    expect(ExcerptPipe.calls).toBe(2);
  });
});

describe("ExcerptPipe in a template", () => {
  let fixture: ComponentFixture<ExcerptHostComponent>;
  let component: ExcerptHostComponent;

  const text = (selector: string): string => {
    const found = fixture.nativeElement.querySelector(selector) as HTMLElement | null;
    if (found === null) {
      throw new Error(`no element matched "${selector}" — is the template implemented?`);
    }
    return found.textContent?.trim() ?? "";
  };

  beforeEach(async () => {
    ExcerptPipe.calls = 0;
    await TestBed.configureTestingModule({
      imports: [ExcerptHostComponent],
    }).compileComponents();
    fixture = TestBed.createComponent(ExcerptHostComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it("renders with the default arguments", () => {
    expect(text("p.default")).toBe("hello");
  });

  it("passes the arguments through", () => {
    component.text.set("0123456789");
    fixture.detectChanges();

    expect(text("p.short")).toBe("01234…");
    expect(text("p.custom")).toBe("01234...");
  });

  it("renders a joined array", () => {
    expect(text("p.joined")).toBe("one, two");
  });

  it("does not re-run on a change-detection pass with no input change", () => {
    // The pipe has run — so a stable count below means caching, not a pipe nobody uses.
    const before = ExcerptPipe.calls;
    expect(before).toBeGreaterThan(0);

    fixture.detectChanges();
    fixture.detectChanges();
    fixture.detectChanges();

    // This is what "pure" buys: the cached result is reused.
    expect(ExcerptPipe.calls).toBe(before);
  });

  it("re-runs when the input changes", () => {
    const before = ExcerptPipe.calls;

    component.text.set("something else");
    fixture.detectChanges();

    expect(ExcerptPipe.calls).toBeGreaterThan(before);
  });

  it("misses an array mutated in place", () => {
    expect(text("p.joined")).toBe("one, two");
    const before = ExcerptPipe.calls;

    component.pushWordInPlace("three");
    fixture.detectChanges();

    // The reference did not change, so the pipe never re-ran and the DOM is stale. Same
    // reference-equality rule as signals, enforced in a different place.
    expect(ExcerptPipe.calls).toBe(before);
    expect(text("p.joined")).toBe("one, two");
  });

  it("sees a replaced array", () => {
    component.addWord("three");
    fixture.detectChanges();

    expect(text("p.joined")).toBe("one, two…");
  });

  it("shows the mutation once something else forces a re-run", () => {
    component.pushWordInPlace("three");
    fixture.detectChanges();
    expect(text("p.joined")).toBe("one, two");

    component.addWord("four");
    fixture.detectChanges();

    // The smuggled-in value appears now, which is what makes this bug so confusing.
    expect(text("p.joined")).toBe("one, two…");
  });
});

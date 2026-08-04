import { ComponentFixture, TestBed } from "@angular/core/testing";
import { ProgressBarComponent } from "./progress-bar.component";

describe("ProgressBarComponent", () => {
  let fixture: ComponentFixture<ProgressBarComponent>;
  let component: ProgressBarComponent;

  const query = <T extends HTMLElement>(selector: string): T => {
    const found = fixture.nativeElement.querySelector(selector) as T | null;
    if (found === null) {
      throw new Error(`no element matched "${selector}" — is the template contract implemented?`);
    }
    return found;
  };

  const at = (value: number, total = 100): void => {
    component.value.set(value);
    component.total.set(total);
    fixture.detectChanges();
  };

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [ProgressBarComponent],
    }).compileComponents();
    fixture = TestBed.createComponent(ProgressBarComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it("computes a percentage", () => {
    at(25);
    expect(component.percent()).toBe(25);

    at(3, 4);
    expect(component.percent()).toBe(75);
  });

  it("rounds to a whole number", () => {
    at(1, 3);

    expect(component.percent()).toBe(33);
  });

  it("clamps instead of overflowing", () => {
    at(150);
    expect(component.percent()).toBe(100);

    at(-20);
    expect(component.percent()).toBe(0);
  });

  it("refuses a total that is not a range", () => {
    component.total.set(0);
    expect(() => component.percent()).toThrow(RangeError);

    component.total.set(-5);
    expect(() => component.percent()).toThrow(RangeError);
  });

  it("appends the percent unit to the width", () => {
    at(42);

    // Without the .% suffix this would be the unitless "42", which CSS drops.
    expect(query("div.fill").style.width).toBe("42%");
  });

  it("moves the width with the value", () => {
    at(10);
    expect(query("div.fill").style.width).toBe("10%");

    at(90);
    expect(query("div.fill").style.width).toBe("90%");
  });

  it("picks a colour per band", () => {
    at(0);
    expect(component.color()).toBe("crimson");

    at(33);
    expect(component.color()).toBe("crimson");

    at(34);
    expect(component.color()).toBe("orange");

    at(66);
    expect(component.color()).toBe("orange");

    at(67);
    expect(component.color()).toBe("seagreen");

    at(100);
    expect(component.color()).toBe("seagreen");
  });

  it("binds the colour as a style", () => {
    at(100);

    expect(query("div.fill").style.backgroundColor).toBe("seagreen");
  });

  it("appends the pixel unit to the label size", () => {
    component.labelSize.set(20);
    fixture.detectChanges();

    expect(query("div.label").style.fontSize).toBe("20px");
  });

  it("renders the percentage as text too", () => {
    at(42);

    expect(query("div.label").textContent).toContain("42%");
  });

  it("builds the style object with both keys", () => {
    at(50);

    expect(component.boxStyles()).toEqual({ "border-color": "orange", opacity: "0.5" });
  });

  it("goes fully opaque when complete", () => {
    at(100);

    expect(component.boxStyles()).toEqual({ "border-color": "seagreen", opacity: "1" });
  });

  it("applies the object through [style]", () => {
    at(50);

    const boxed = query("div.boxed");
    expect(boxed.style.borderColor).toBe("orange");
    expect(boxed.style.opacity).toBe("0.5");
  });

  it("updates styles bound through the object", () => {
    at(50);
    expect(query("div.boxed").style.opacity).toBe("0.5");

    at(100);

    expect(query("div.boxed").style.opacity).toBe("1");
    expect(query("div.boxed").style.borderColor).toBe("seagreen");
  });

  it("applies the same object through [ngStyle]", () => {
    at(10);

    // Same result, but this one needed NgStyle imported to work at all.
    const legacy = query("div.legacy");
    expect(legacy.style.borderColor).toBe("crimson");
    expect(legacy.style.opacity).toBe("0.5");
  });
});

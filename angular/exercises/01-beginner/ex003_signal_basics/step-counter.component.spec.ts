import { ComponentFixture, TestBed } from "@angular/core/testing";
import { StepCounterComponent } from "./step-counter.component";

describe("StepCounterComponent", () => {
  let fixture: ComponentFixture<StepCounterComponent>;
  let component: StepCounterComponent;

  const query = <T extends Element>(selector: string): T => {
    const found = fixture.nativeElement.querySelector(selector) as T | null;
    if (found === null) {
      throw new Error(`no element matched "${selector}" — is the template contract implemented?`);
    }
    return found;
  };

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [StepCounterComponent],
    }).compileComponents();
    fixture = TestBed.createComponent(StepCounterComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it("renders its starting state", () => {
    expect(component.count()).toBe(0);
    expect(component.step()).toBe(1);
    expect(query("p.count").textContent).toContain("Count: 0");
    expect(query("p.step").textContent).toContain("Step: 1");
  });

  it("increments by the step", () => {
    component.increment();

    expect(component.count()).toBe(1);
  });

  it("increments by a larger step", () => {
    component.setStep(5);
    component.increment();
    component.increment();

    expect(component.count()).toBe(10);
  });

  it("decrements by the step", () => {
    component.setStep(2);
    component.increment();
    component.increment();
    component.decrement();

    expect(component.count()).toBe(2);
  });

  it("never goes below zero", () => {
    component.setStep(10);
    component.increment();
    component.decrement();
    component.decrement();

    expect(component.count()).toBe(0);
  });

  it("rejects a step below one", () => {
    expect(() => component.setStep(0)).toThrow(RangeError);
    expect(() => component.setStep(-3)).toThrow(RangeError);
  });

  it("rejects a fractional step", () => {
    expect(() => component.setStep(1.5)).toThrow(RangeError);
  });

  it("leaves the step untouched when a change is rejected", () => {
    component.setStep(4);

    expect(() => component.setStep(0)).toThrow(RangeError);
    expect(component.step()).toBe(4);
  });

  it("resets the count but keeps the step", () => {
    component.setStep(3);
    component.increment();
    component.reset();

    expect(component.count()).toBe(0);
    expect(component.step()).toBe(3);
  });

  it("renders both signals", () => {
    component.setStep(7);
    component.increment();
    component.increment();
    fixture.detectChanges();

    expect(query("p.count").textContent).toContain("14");
    expect(query("p.step").textContent).toContain("7");
  });

  it("does not touch the DOM until change detection runs", () => {
    component.increment();

    // The signal changed; the rendered text has not been recomputed yet.
    expect(query("p.count").textContent).toContain("0");

    fixture.detectChanges();

    expect(query("p.count").textContent).toContain("1");
  });

  it("increments from the + button", () => {
    query<HTMLButtonElement>("button.inc").click();

    expect(component.count()).toBe(1);
  });

  it("decrements from the - button", () => {
    component.increment();
    component.increment();
    query<HTMLButtonElement>("button.dec").click();

    expect(component.count()).toBe(1);
  });

  it("resets from the reset button", () => {
    component.increment();
    query<HTMLButtonElement>("button.reset").click();

    expect(component.count()).toBe(0);
  });
});

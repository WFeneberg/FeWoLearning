import { ComponentFixture, TestBed } from "@angular/core/testing";
import { CountdownComponent } from "./countdown.component";

describe("CountdownComponent", () => {
  let fixture: ComponentFixture<CountdownComponent>;
  let component: CountdownComponent;

  const text = (selector: string): string => {
    const found = fixture.nativeElement.querySelector(selector) as HTMLElement | null;
    if (found === null) {
      throw new Error(`no element matched "${selector}" — is the template implemented?`);
    }
    return found.textContent?.trim() ?? "";
  };

  beforeEach(async () => {
    await TestBed.configureTestingModule({ imports: [CountdownComponent] }).compileComponents();
    fixture = TestBed.createComponent(CountdownComponent);
    // A required signal input has no default — it must be set before the first detectChanges().
    fixture.componentRef.setInput("startFrom", 3);
    component = fixture.componentInstance;
  });

  it("seeds remaining from the input, readable straight off the instance", () => {
    // No render has happened yet — the signal is already correct.
    expect(component.remaining()).toBe(3);
  });

  it("does not update the DOM until detectChanges runs", () => {
    fixture.detectChanges();
    expect(text(".remaining")).toBe("3");

    component.tick();

    // The instance already agrees with the tick; the DOM has not been told yet.
    expect(component.remaining()).toBe(2);
    expect(text(".remaining")).toBe("3");

    fixture.detectChanges();
    expect(text(".remaining")).toBe("2");
  });

  it("never ticks below zero", () => {
    fixture.detectChanges();
    component.tick();
    component.tick();
    component.tick();
    component.tick();

    expect(component.remaining()).toBe(0);
    expect(component.finished()).toBe(true);
  });

  it("shows Done once finished", () => {
    fixture.detectChanges();
    component.tick();
    component.tick();
    component.tick();
    fixture.detectChanges();

    expect(text(".remaining")).toBe("Done");
  });

  it("ticks from the button in the template", () => {
    fixture.detectChanges();

    (fixture.nativeElement.querySelector("button.tick") as HTMLButtonElement).click();
    fixture.detectChanges();

    expect(text(".remaining")).toBe("2");
  });

  it("setInput re-seeds remaining, the way a fresh binding would", () => {
    fixture.detectChanges();
    component.tick();
    expect(component.remaining()).toBe(2);

    fixture.componentRef.setInput("startFrom", 10);
    fixture.detectChanges();

    expect(component.remaining()).toBe(10);
    expect(text(".remaining")).toBe("10");
  });

  it("setInput brings a finished countdown back to life", () => {
    fixture.detectChanges();
    component.tick();
    component.tick();
    component.tick();
    expect(component.finished()).toBe(true);

    fixture.componentRef.setInput("startFrom", 5);
    fixture.detectChanges();

    expect(component.finished()).toBe(false);
    expect(component.remaining()).toBe(5);
  });
});

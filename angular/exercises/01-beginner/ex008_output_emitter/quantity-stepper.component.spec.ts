import { Component } from "@angular/core";
import { ComponentFixture, TestBed } from "@angular/core/testing";
import { QuantityStepperComponent } from "./quantity-stepper.component";

/** A parent that listens the way a real template would, by the outputs' public names. */
@Component({
  selector: "app-host",
  standalone: true,
  imports: [QuantityStepperComponent],
  template: `
    <app-quantity-stepper
      [max]="2"
      (changed)="seen.push($event)"
      (limit)="limits.push($event)"
    />
  `,
})
class HostComponent {
  readonly seen: number[] = [];
  readonly limits: number[] = [];
}

describe("QuantityStepperComponent", () => {
  let fixture: ComponentFixture<QuantityStepperComponent>;
  let component: QuantityStepperComponent;
  let emitted: number[];
  let limits: number[];

  const query = <T extends Element>(selector: string): T => {
    const found = fixture.nativeElement.querySelector(selector) as T | null;
    if (found === null) {
      throw new Error(`no element matched "${selector}" — is the template contract implemented?`);
    }
    return found;
  };

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [QuantityStepperComponent, HostComponent],
    }).compileComponents();
    fixture = TestBed.createComponent(QuantityStepperComponent);
    component = fixture.componentInstance;
    emitted = [];
    limits = [];
    // An EventEmitter is an RxJS Subject, so a test can listen without a parent.
    component.changed.subscribe((value) => emitted.push(value));
    component.limitReached.subscribe((value) => limits.push(value));
    fixture.detectChanges();
  });

  it("emits the new value on the way up", () => {
    component.inc();

    expect(component.value()).toBe(1);
    expect(emitted).toEqual([1]);
  });

  it("emits once per step", () => {
    component.inc();
    component.inc();
    component.inc();

    expect(emitted).toEqual([1, 2, 3]);
  });

  it("emits on the way down too", () => {
    component.inc();
    component.inc();
    component.dec();

    expect(emitted).toEqual([1, 2, 1]);
  });

  it("stops at zero and stays silent", () => {
    component.dec();

    expect(component.value()).toBe(0);
    // Nothing changed, so there is nothing to announce.
    expect(emitted).toEqual([]);
  });

  it("stops at max and stays silent", () => {
    component.max = 2;
    component.inc();
    component.inc();
    emitted.length = 0;

    component.inc();

    expect(component.value()).toBe(2);
    expect(emitted).toEqual([]);
  });

  it("announces reaching the ceiling exactly once", () => {
    component.max = 2;
    component.inc();
    expect(limits).toEqual([]);

    component.inc();
    expect(limits).toEqual([2]);

    // Already at the top: no further limit events.
    component.inc();
    expect(limits).toEqual([2]);
  });

  it("renders the value", () => {
    component.inc();
    fixture.detectChanges();

    expect(query("p.value").textContent).toContain("1");
  });

  it("steps from the buttons", () => {
    query<HTMLButtonElement>("button.inc").click();
    query<HTMLButtonElement>("button.inc").click();
    query<HTMLButtonElement>("button.dec").click();

    expect(component.value()).toBe(1);
    expect(emitted).toEqual([1, 2, 1]);
  });

  it("reaches a parent listening in a template", () => {
    const host = TestBed.createComponent(HostComponent);
    host.detectChanges();

    const stepper: HTMLElement = host.nativeElement.querySelector("app-quantity-stepper");
    const inc = stepper.querySelector("button.inc") as HTMLButtonElement;
    inc.click();
    inc.click();
    host.detectChanges();

    expect(host.componentInstance.seen).toEqual([1, 2]);
  });

  it("reaches a parent through the alias", () => {
    const host = TestBed.createComponent(HostComponent);
    host.detectChanges();

    const stepper: HTMLElement = host.nativeElement.querySelector("app-quantity-stepper");
    const inc = stepper.querySelector("button.inc") as HTMLButtonElement;
    inc.click();
    inc.click();
    host.detectChanges();

    // The parent binds (limit), not (limitReached).
    expect(host.componentInstance.limits).toEqual([2]);
  });
});

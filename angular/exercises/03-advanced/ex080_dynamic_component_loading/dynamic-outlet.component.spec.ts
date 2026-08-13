import { Component, input, viewChild } from "@angular/core";
import { ComponentFixture, TestBed } from "@angular/core/testing";
import { DynamicOutletComponent } from "./dynamic-outlet.component";

@Component({
  selector: "app-greeting-card",
  standalone: true,
  template: `<p class="greeting">Hello, {{ name() }}{{ shout() ? "!!!" : "." }}</p>`,
})
class GreetingCardComponent {
  readonly name = input.required<string>();
  readonly shout = input(false);
}

@Component({
  standalone: true,
  imports: [DynamicOutletComponent],
  template: `<app-dynamic-outlet />`,
})
class HostComponent {
  readonly outlet = viewChild.required(DynamicOutletComponent);
}

describe("DynamicOutletComponent", () => {
  let fixture: ComponentFixture<HostComponent>;
  let outlet: DynamicOutletComponent;
  let hostEl: HTMLElement;

  beforeEach(() => {
    TestBed.configureTestingModule({ imports: [HostComponent] });
    fixture = TestBed.createComponent(HostComponent);
    fixture.detectChanges();
    outlet = fixture.componentInstance.outlet();
    hostEl = fixture.nativeElement as HTMLElement;
  });

  it("creates the component with the inputs passed to load()", () => {
    outlet.load(GreetingCardComponent, { name: "Ada" });
    fixture.detectChanges();

    expect(hostEl.querySelector(".greeting")?.textContent).toBe("Hello, Ada.");
  });

  it("applies every input passed to load(), not just the first", () => {
    outlet.load(GreetingCardComponent, { name: "Ada", shout: true });
    fixture.detectChanges();

    expect(hostEl.querySelector(".greeting")?.textContent).toBe("Hello, Ada!!!");
  });

  it("destroys the previous instance when loading a new component", () => {
    const first = outlet.load(GreetingCardComponent, { name: "Ada" });
    outlet.load(GreetingCardComponent, { name: "Grace" });

    expect(first.hostView.destroyed).toBe(true);
  });

  it("only ever mounts one component at a time", () => {
    outlet.load(GreetingCardComponent, { name: "Ada" });
    outlet.load(GreetingCardComponent, { name: "Grace" });
    fixture.detectChanges();

    const greetings = hostEl.querySelectorAll(".greeting");
    expect(greetings.length).toBe(1);
    expect(greetings[0].textContent).toBe("Hello, Grace.");
  });

  it("clear() removes the mounted component and destroys its ref", () => {
    const ref = outlet.load(GreetingCardComponent, { name: "Ada" });
    outlet.clear();
    fixture.detectChanges();

    expect(hostEl.querySelector(".greeting")).toBeNull();
    expect(ref.hostView.destroyed).toBe(true);
  });

  it("clear() on an empty outlet does not throw", () => {
    expect(() => outlet.clear()).not.toThrow();
  });
});

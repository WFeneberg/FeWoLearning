import { Component } from "@angular/core";
import { ComponentFixture, TestBed } from "@angular/core/testing";
import { GreetingComponent } from "./greeting.component";

/** A host that binds the input in a template, the way a real parent does. */
@Component({
  selector: "app-host",
  standalone: true,
  imports: [GreetingComponent],
  template: `<app-greeting name="Ada" salutation="Hi" />`,
})
class HostComponent {}

describe("GreetingComponent", () => {
  let fixture: ComponentFixture<GreetingComponent>;
  let component: GreetingComponent;

  const query = <T extends HTMLElement>(selector: string): T => {
    const found = fixture.nativeElement.querySelector(selector) as T | null;
    if (found === null) {
      throw new Error(`no element matched "${selector}" — is the template contract implemented?`);
    }
    return found;
  };

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [GreetingComponent, HostComponent],
    }).compileComponents();
    fixture = TestBed.createComponent(GreetingComponent);
    component = fixture.componentInstance;
  });

  it("runs the constructor before any input is set", () => {
    // createComponent() has run, detectChanges() has not.
    expect(component.log).toEqual(["constructor"]);
  });

  it("does not run ngOnInit until change detection", () => {
    expect(component.log).not.toContain("ngOnInit");

    fixture.detectChanges();

    expect(component.log).toEqual(["constructor", "ngOnInit"]);
  });

  it("runs ngOnInit only once", () => {
    fixture.detectChanges();
    fixture.detectChanges();
    fixture.detectChanges();

    expect(component.log).toEqual(["constructor", "ngOnInit"]);
  });

  it("cannot see a decorator input in the constructor", () => {
    fixture.componentRef.setInput("name", "Ada");
    fixture.detectChanges();

    // The classic trap: setup that reads an input from the constructor gets undefined.
    expect(component.nameAtConstruction).toBeUndefined();
    expect(component.nameAtInit).toBe("Ada");
  });

  it("builds the greeting from the bound input", () => {
    fixture.componentRef.setInput("name", "Ada");
    fixture.detectChanges();

    expect(component.greeting).toBe("Hello, Ada!");
  });

  it("uses the salutation it was given", () => {
    fixture.componentRef.setInput("name", "Ada");
    fixture.componentRef.setInput("salutation", "Hi");
    fixture.detectChanges();

    expect(component.greeting).toBe("Hi, Ada!");
  });

  it("falls back to guest with no name", () => {
    fixture.detectChanges();

    expect(component.greeting).toBe("Hello, guest!");
  });

  it("renders the greeting", () => {
    fixture.componentRef.setInput("name", "Ada");
    fixture.detectChanges();

    expect(query("p.greeting").textContent).toContain("Hello, Ada!");
  });

  it("renders the hook order", () => {
    fixture.detectChanges();

    expect(query("p.order").textContent).toContain("constructor > ngOnInit");
  });

  it("does not rebuild the greeting when the input changes later", () => {
    fixture.componentRef.setInput("name", "Ada");
    fixture.detectChanges();
    expect(component.greeting).toBe("Hello, Ada!");

    fixture.componentRef.setInput("name", "Grace");
    fixture.detectChanges();

    // ngOnInit ran once, so a value built there is a snapshot. Reacting to later changes
    // is what ngOnChanges — or a signal input — is for.
    expect(component.greeting).toBe("Hello, Ada!");
  });

  it("works the same when a parent template binds it", () => {
    const host = TestBed.createComponent(HostComponent);
    host.detectChanges();

    const greeting: GreetingComponent = host.debugElement.children[0].componentInstance;

    expect(greeting.nameAtConstruction).toBeUndefined();
    expect(greeting.nameAtInit).toBe("Ada");
    expect(greeting.greeting).toBe("Hi, Ada!");
  });
});

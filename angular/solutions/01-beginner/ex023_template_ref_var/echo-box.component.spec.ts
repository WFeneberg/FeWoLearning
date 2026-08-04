import { ComponentFixture, TestBed } from "@angular/core/testing";
import { EchoBoxComponent } from "./echo-box.component";

describe("EchoBoxComponent", () => {
  let fixture: ComponentFixture<EchoBoxComponent>;
  let component: EchoBoxComponent;

  const query = <T extends HTMLElement>(selector: string): T => {
    const found = fixture.nativeElement.querySelector(selector) as T | null;
    if (found === null) {
      throw new Error(`no element matched "${selector}" — is the template contract implemented?`);
    }
    return found;
  };

  const typeInto = (selector: string, value: string): void => {
    const input = query<HTMLInputElement>(selector);
    input.value = value;
    input.dispatchEvent(new Event("input"));
    fixture.detectChanges();
  };

  const click = (selector: string): void => {
    query<HTMLButtonElement>(selector).click();
    fixture.detectChanges();
  };

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [EchoBoxComponent],
    }).compileComponents();
    fixture = TestBed.createComponent(EchoBoxComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it("starts with nothing echoed", () => {
    expect(component.echo()).toBe("");
    expect(query("p.echo").textContent?.trim()).toBe("");
  });

  it("hands the element itself to the handler", () => {
    typeInto("input.name", "Ada");
    click("button.copy");

    expect(component.echo()).toBe("Ada");
    expect(query("p.echo").textContent).toContain("Ada");
  });

  it("trims what it copies", () => {
    typeInto("input.name", "  Ada  ");
    click("button.copy");

    expect(component.echo()).toBe("Ada");
  });

  it("counts the copies", () => {
    typeInto("input.name", "Ada");
    click("button.copy");
    click("button.copy");

    expect(component.copies()).toBe(2);
  });

  it("reads a DOM property straight from the expression", () => {
    typeInto("input.name", "Ada");

    // No class field involved: the template read nameBox.value.length itself.
    expect(query("p.length").textContent?.trim()).toBe("3");
  });

  it("calls a DOM method through the reference", () => {
    click("button.focus");

    // (click)="nameBox.focus()" — the handler is on the button, the target is the input.
    expect(document.activeElement).toBe(query("input.name"));
  });

  it("writes back to the element it was given", () => {
    typeInto("input.name", "Ada");
    click("button.copy");
    expect(component.echo()).toBe("Ada");

    click("button.clear");

    expect(query<HTMLInputElement>("input.name").value).toBe("");
    expect(component.echo()).toBe("");
  });

  it("leaves the copy count alone when clearing", () => {
    typeInto("input.name", "Ada");
    click("button.copy");
    click("button.clear");

    expect(component.copies()).toBe(1);
  });

  it("references a second element independently", () => {
    expect(query("p.flag-state").textContent?.trim()).toBe("off");

    const flag = query<HTMLInputElement>("input.flag");
    flag.checked = true;
    flag.dispatchEvent(new Event("change"));
    fixture.detectChanges();

    expect(query("p.flag-state").textContent?.trim()).toBe("on");
  });

  it("reads the DOM property at change-detection time, not continuously", () => {
    const flag = query<HTMLInputElement>("input.flag");

    // Changed behind Angular's back, with nothing to trigger a cycle.
    flag.checked = true;

    expect(query("p.flag-state").textContent?.trim()).toBe("off");

    fixture.detectChanges();

    expect(query("p.flag-state").textContent?.trim()).toBe("on");
  });

  it("keeps the reference out of the class", () => {
    // Template scope only — reaching it from TypeScript is what viewChild() is for.
    expect((component as unknown as Record<string, unknown>)["nameBox"]).toBeUndefined();

    typeInto("input.name", "Ada");
    click("button.copy");
    expect(component.echo()).toBe("Ada");
  });
});

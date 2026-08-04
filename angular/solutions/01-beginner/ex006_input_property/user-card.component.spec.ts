import { Component } from "@angular/core";
import { ComponentFixture, TestBed } from "@angular/core/testing";
import { UserCardComponent } from "./user-card.component";

/** A host that binds the inputs the way a template would, using their public names. */
@Component({
  selector: "app-host",
  standalone: true,
  imports: [UserCardComponent],
  template: `<app-user-card name="Ada" score="42" compact [admin]="true" />`,
})
class HostComponent {}

describe("UserCardComponent", () => {
  let fixture: ComponentFixture<UserCardComponent>;
  let component: UserCardComponent;

  const query = <T extends Element>(selector: string): T => {
    const found = fixture.nativeElement.querySelector(selector) as T | null;
    if (found === null) {
      throw new Error(`no element matched "${selector}" — is the template contract implemented?`);
    }
    return found;
  };

  const setInput = (name: string, value: unknown): void => {
    fixture.componentRef.setInput(name, value);
    fixture.detectChanges();
  };

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [UserCardComponent, HostComponent],
    }).compileComponents();
    fixture = TestBed.createComponent(UserCardComponent);
    component = fixture.componentInstance;
    fixture.componentRef.setInput("name", "Grace");
    fixture.detectChanges();
  });

  it("keeps the declared defaults", () => {
    expect(component.role).toBe("member");
    expect(component.score).toBe(0);
    expect(component.compact).toBe(false);
    expect(component.isAdmin).toBe(false);

    // The defaults have to survive all the way into the rendered card.
    expect(component.badge()).toBe("MEMBER");
    expect(query("p.badge").textContent?.trim()).toBe("MEMBER");
    expect(query("p.score").textContent).toContain("0");
    expect(query("p.mode").textContent?.trim()).toBe("full");
  });

  it("renders a bound name", () => {
    expect(query("h3.name").textContent?.trim()).toBe("Grace");
  });

  it("accepts a new name", () => {
    setInput("name", "Ada");

    expect(query("h3.name").textContent?.trim()).toBe("Ada");
  });

  it("builds the badge from the role", () => {
    setInput("role", "owner");

    expect(component.badge()).toBe("OWNER");
    expect(query("p.badge").textContent?.trim()).toBe("OWNER");
  });

  it("prefers ADMIN over the role", () => {
    setInput("role", "owner");
    setInput("admin", true);

    expect(component.badge()).toBe("ADMIN");
  });

  it("exposes isAdmin under its alias", () => {
    setInput("admin", true);

    // The public name is `admin`; the field it lands on is `isAdmin`.
    expect(component.isAdmin).toBe(true);
  });

  it("coerces a string score into a number", () => {
    setInput("score", "42");

    expect(component.score).toBe(42);
    expect(typeof component.score).toBe("number");
    expect(query("p.score").textContent).toContain("42");
  });

  it("coerces an unparseable score into NaN rather than a string", () => {
    setInput("score", "not-a-number");

    expect(Number.isNaN(component.score)).toBe(true);
  });

  it("treats an empty attribute value as true", () => {
    setInput("compact", "");

    expect(component.compact).toBe(true);
    expect(query("p.mode").textContent?.trim()).toBe("compact");
  });

  it('treats the literal string "false" as false', () => {
    setInput("compact", "false");

    expect(component.compact).toBe(false);
    expect(query("p.mode").textContent?.trim()).toBe("full");
  });

  it("treats a missing value as false", () => {
    setInput("compact", "");
    expect(component.compact).toBe(true);

    // null and undefined both fall back to false rather than staying truthy.
    setInput("compact", null);
    expect(component.compact).toBe(false);

    setInput("compact", undefined);
    expect(component.compact).toBe(false);
  });

  it("wires up from a host template", () => {
    const host = TestBed.createComponent(HostComponent);
    host.detectChanges();

    const card: UserCardComponent = host.debugElement.children[0].componentInstance;

    expect(card.name).toBe("Ada");
    // Attribute syntax passes strings; the transforms are what make these usable.
    expect(card.score).toBe(42);
    expect(card.compact).toBe(true);
    expect(card.isAdmin).toBe(true);
    expect(host.nativeElement.textContent).toContain("ADMIN");
  });
});

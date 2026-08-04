import { ComponentFixture, TestBed } from "@angular/core/testing";
import { ProfileFormComponent } from "./profile-form.component";

describe("ProfileFormComponent", () => {
  let fixture: ComponentFixture<ProfileFormComponent>;
  let component: ProfileFormComponent;

  const query = <T extends HTMLElement>(selector: string): T => {
    const found = fixture.nativeElement.querySelector(selector) as T | null;
    if (found === null) {
      throw new Error(`no element matched "${selector}" — is the template contract implemented?`);
    }
    return found;
  };

  /** Type into a control the way a keystroke does: set value, then fire "input". */
  const typeInto = async (selector: string, value: string): Promise<void> => {
    const input = query<HTMLInputElement>(selector);
    input.value = value;
    input.dispatchEvent(new Event("input"));
    fixture.detectChanges();
    await fixture.whenStable();
  };

  const check = async (selector: string, checked: boolean): Promise<void> => {
    const input = query<HTMLInputElement>(selector);
    input.checked = checked;
    input.dispatchEvent(new Event("change"));
    fixture.detectChanges();
    await fixture.whenStable();
  };

  const select = async (selector: string, value: string): Promise<void> => {
    const element = query<HTMLSelectElement>(selector);
    element.value = value;
    element.dispatchEvent(new Event("change"));
    fixture.detectChanges();
    await fixture.whenStable();
  };

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [ProfileFormComponent],
    }).compileComponents();
    fixture = TestBed.createComponent(ProfileFormComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
    await fixture.whenStable();
  });

  it("pushes a typed name into the model", async () => {
    await typeInto("input.name", "Ada");

    expect(component.name).toBe("Ada");
  });

  it("pulls a model change back into the control", async () => {
    component.name = "Grace";
    fixture.detectChanges();
    await fixture.whenStable();

    expect(query<HTMLInputElement>("input.name").value).toBe("Grace");
  });

  it("binds a number control", async () => {
    await typeInto("input.age", "34");

    // A number input hands ngModel a number, not the string "34".
    expect(component.age).toBe(34);
  });

  it("binds a checkbox", async () => {
    await check("input.subscribed", true);
    expect(component.subscribed).toBe(true);

    await check("input.subscribed", false);
    expect(component.subscribed).toBe(false);
  });

  it("pushes a checkbox change out to the control", async () => {
    component.subscribed = true;
    fixture.detectChanges();
    await fixture.whenStable();

    expect(query<HTMLInputElement>("input.subscribed").checked).toBe(true);
  });

  it("binds a select", async () => {
    await select("select.role", "admin");

    expect(component.role).toBe("admin");
  });

  it("pushes a select change out to the control", async () => {
    component.role = "admin";
    fixture.detectChanges();
    await fixture.whenStable();

    expect(query<HTMLSelectElement>("select.role").value).toBe("admin");
  });

  it("summarises a blank form", () => {
    expect(component.summary()).toBe("Anonymous (0, member, unsubscribed)");
  });

  it("summarises a filled form", () => {
    component.name = "Ada";
    component.age = 34;
    component.role = "admin";
    component.subscribed = true;

    expect(component.summary()).toBe("Ada (34, admin, subscribed)");
  });

  it("treats a whitespace-only name as anonymous", () => {
    component.name = "   ";

    expect(component.summary()).toBe("Anonymous (0, member, unsubscribed)");
  });

  it("renders the summary and keeps it current", async () => {
    expect(query("p.summary").textContent).toContain("Anonymous");

    await typeInto("input.name", "Ada");
    await typeInto("input.age", "34");
    await check("input.subscribed", true);

    expect(query("p.summary").textContent).toContain("Ada (34, member, subscribed)");
  });
});

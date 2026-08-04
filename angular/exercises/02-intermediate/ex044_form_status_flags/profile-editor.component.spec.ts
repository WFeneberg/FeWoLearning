import { ComponentFixture, TestBed } from "@angular/core/testing";
import { ProfileEditorComponent } from "./profile-editor.component";

const RECORD = { name: "Ada", email: "ada@example.com" };

describe("ProfileEditorComponent", () => {
  let fixture: ComponentFixture<ProfileEditorComponent>;
  let component: ProfileEditorComponent;

  const input = (selector: string): HTMLInputElement => {
    const found = fixture.nativeElement.querySelector(selector) as HTMLInputElement | null;
    if (found === null) {
      throw new Error(`no element matched "${selector}" — is the template implemented?`);
    }
    return found;
  };

  const text = (selector: string): string =>
    (fixture.nativeElement.querySelector(selector) as HTMLElement | null)?.textContent?.trim() ??
    `MISSING ${selector}`;

  /** Type into a control the way a user does, so the dirty flag is set for real. */
  const typeInto = (selector: string, value: string): void => {
    const element = input(selector);
    element.value = value;
    element.dispatchEvent(new Event("input"));
    fixture.detectChanges();
  };

  /** Focus and blur, which is what sets touched. */
  const blur = (selector: string): void => {
    input(selector).dispatchEvent(new Event("blur"));
    fixture.detectChanges();
  };

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [ProfileEditorComponent],
    }).compileComponents();
    fixture = TestBed.createComponent(ProfileEditorComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it("starts pristine and untouched, though invalid", () => {
    expect(component.form.pristine).toBe(true);
    expect(component.form.touched).toBe(false);
    expect(component.form.invalid).toBe(true);

    // Invalid but untouched: the user has not done anything wrong, they have done nothing.
    expect(component.showErrorsFor("name")).toBe(false);
    expect(component.stateLabel()).toBe("clean");
  });

  it("shows a control's errors once it is blurred", () => {
    blur("input.name");

    expect(component.controlFor("name").touched).toBe(true);
    expect(component.showErrorsFor("name")).toBe(true);
    // Only that control, not its neighbour.
    expect(component.showErrorsFor("email")).toBe(false);
  });

  it("shows errors for a control the user typed in but left invalid", () => {
    // Typed, then cleared again — dirty without ever being blurred, which is the case the
    // `touched` flag alone would miss.
    typeInto("input.email", "ada@example.com");
    typeInto("input.email", "");

    expect(component.controlFor("email").dirty).toBe(true);
    expect(component.controlFor("email").touched).toBe(false);
    expect(component.showErrorsFor("email")).toBe(true);
  });

  it("stays pristine when a record is loaded", () => {
    component.load(RECORD);

    expect(component.form.value).toEqual(RECORD);
    expect(component.form.valid).toBe(true);
    // Loading is not editing, so there is nothing unsaved.
    expect(component.form.pristine).toBe(true);
    expect(component.hasUnsavedChanges()).toBe(false);
    expect(component.canSave()).toBe(false);
  });

  it("does not dirty a control through setValue", () => {
    component.controlFor("name").setValue("Grace");

    // Deliberate: only user interaction dirties a control, which is what makes the flag
    // usable as "are there unsaved changes".
    expect(component.controlFor("name").pristine).toBe(true);
  });

  it("dirties a control the user types in", () => {
    typeInto("input.name", "Grace");

    expect(component.controlFor("name").dirty).toBe(true);
    expect(component.form.dirty).toBe(true);
  });

  it("reports unsaved changes after an edit", () => {
    component.load(RECORD);

    typeInto("input.name", "Grace");

    expect(component.hasUnsavedChanges()).toBe(true);
  });

  it("labels the three states", () => {
    expect(component.stateLabel()).toBe("clean");

    typeInto("input.name", "Ada");
    expect(component.stateLabel()).toBe("editing");

    typeInto("input.email", "ada@example.com");
    expect(component.stateLabel()).toBe("ready");
  });

  it("renders the state label", () => {
    typeInto("input.name", "Ada");
    typeInto("input.email", "ada@example.com");

    expect(text("p.state")).toBe("ready");
  });

  it("refuses to save a pristine form", () => {
    component.load(RECORD);

    component.save();

    expect(component.saveCount).toBe(0);
  });

  it("refuses to save an invalid form", () => {
    typeInto("input.name", "Ada");

    expect(component.canSave()).toBe(false);
    component.save();

    expect(component.saveCount).toBe(0);
  });

  it("saves a dirty valid form", () => {
    typeInto("input.name", "Ada");
    typeInto("input.email", "ada@example.com");

    expect(component.canSave()).toBe(true);
    component.save();

    expect(component.saveCount).toBe(1);
  });

  it("stops reporting unsaved changes after a save", () => {
    typeInto("input.name", "Ada");
    typeInto("input.email", "ada@example.com");

    component.save();

    // markAsPristine is the only thing that does this. Without it the form nags forever.
    expect(component.form.pristine).toBe(true);
    expect(component.hasUnsavedChanges()).toBe(false);
    expect(component.canSave()).toBe(false);
  });

  it("becomes saveable again after another edit", () => {
    typeInto("input.name", "Ada");
    typeInto("input.email", "ada@example.com");
    component.save();

    typeInto("input.name", "Grace");

    expect(component.canSave()).toBe(true);
    component.save();
    expect(component.saveCount).toBe(2);
  });

  it("gates the save button", () => {
    expect(input("input.name")).toBeDefined();
    expect(
      (fixture.nativeElement.querySelector("button.save") as HTMLButtonElement).disabled,
    ).toBe(true);

    typeInto("input.name", "Ada");
    typeInto("input.email", "ada@example.com");

    expect(
      (fixture.nativeElement.querySelector("button.save") as HTMLButtonElement).disabled,
    ).toBe(false);
  });

  it("reveals every error on a failed submit", () => {
    component.revealErrors();

    expect(component.showErrorsFor("name")).toBe(true);
    expect(component.showErrorsFor("email")).toBe(true);
    // markAllAsTouched touches, it does not dirty.
    expect(component.form.pristine).toBe(true);
  });
});

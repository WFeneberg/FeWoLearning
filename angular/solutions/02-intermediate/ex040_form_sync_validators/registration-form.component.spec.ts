import { ComponentFixture, TestBed } from "@angular/core/testing";
import { RegistrationFormComponent } from "./registration-form.component";

const VALID = {
  email: "ada@example.com",
  password: "correct-horse",
  age: 36,
  code: "ABC-1234",
};

describe("RegistrationFormComponent", () => {
  let fixture: ComponentFixture<RegistrationFormComponent>;
  let component: RegistrationFormComponent;

  const errors = (): string[] =>
    Array.from(fixture.nativeElement.querySelectorAll("li.error") as NodeListOf<HTMLElement>).map(
      (node) => node.textContent?.trim() ?? "",
    );

  const submitButton = (): HTMLButtonElement => {
    const found = fixture.nativeElement.querySelector("button.submit") as HTMLButtonElement | null;
    if (found === null) {
      throw new Error('no element matched "button.submit" — is the template implemented?');
    }
    return found;
  };

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [RegistrationFormComponent],
    }).compileComponents();
    fixture = TestBed.createComponent(RegistrationFormComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it("starts invalid, because the required fields are empty", () => {
    expect(component.form.invalid).toBe(true);
  });

  it("becomes valid when everything is filled in", () => {
    component.form.patchValue(VALID);

    expect(component.form.valid).toBe(true);
    expect(component.visibleErrors()).toEqual([]);
  });

  it("reports a missing required field", () => {
    expect(component.messagesFor("email")).toEqual(["email is required"]);
  });

  it("reports a malformed email", () => {
    component.controlFor("email").setValue("not-an-email");

    expect(component.messagesFor("email")).toEqual(["email must be an email address"]);
  });

  it("carries the lengths in the minlength payload", () => {
    component.controlFor("password").setValue("short");

    // Note the lower-case key: the factory is minLength, the error is minlength.
    expect(component.controlFor("password").errors).toEqual({
      minlength: { requiredLength: 8, actualLength: 5 },
    });
  });

  it("puts the required length into the message", () => {
    component.controlFor("password").setValue("short");

    expect(component.messagesFor("password")).toEqual([
      "password must be at least 8 characters",
    ]);
  });

  it("accepts a long enough password", () => {
    component.controlFor("password").setValue("long-enough");

    expect(component.messagesFor("password")).toEqual([]);
  });

  it("enforces a minimum age", () => {
    component.controlFor("age").setValue(12);

    expect(component.messagesFor("age")).toEqual(["age must be at least 18"]);
  });

  it("enforces a maximum age", () => {
    component.controlFor("age").setValue(500);

    expect(component.messagesFor("age")).toEqual(["age must be at most 120"]);
  });

  it("accepts an age in range", () => {
    component.controlFor("age").setValue(36);

    expect(component.messagesFor("age")).toEqual([]);
  });

  it("enforces the code pattern", () => {
    component.controlFor("code").setValue("abc-1234");

    expect(component.messagesFor("code")).toEqual(["code is not in the expected format"]);
  });

  it("accepts a well-formed code", () => {
    component.controlFor("code").setValue("XYZ-0001");

    expect(component.messagesFor("code")).toEqual([]);
  });

  it("leaves an optional field valid while empty", () => {
    // A pattern validator has nothing to complain about when there is no value.
    expect(component.messagesFor("code")).toEqual([]);
  });

  it("hides errors on a fresh form", () => {
    // Invalid, but nothing has been touched, so the user is not shouted at.
    expect(component.form.invalid).toBe(true);
    expect(component.visibleErrors()).toEqual([]);
    expect(errors()).toEqual([]);
  });

  it("shows a control's errors once it is touched", () => {
    component.controlFor("email").markAsTouched();
    fixture.detectChanges();

    expect(component.visibleErrors()).toEqual(["email is required"]);
    expect(errors()).toEqual(["email is required"]);
  });

  it("shows errors for an edited but untouched control", () => {
    component.controlFor("password").markAsDirty();
    component.controlFor("password").setValue("abc");
    fixture.detectChanges();

    expect(component.visibleErrors()).toEqual(["password must be at least 8 characters"]);
  });

  it("reveals everything on a submit attempt", () => {
    component.revealAllErrors();
    fixture.detectChanges();

    expect(component.visibleErrors()).toEqual([
      "email is required",
      "password is required",
      "age must be at least 18",
    ]);
    expect(errors()).toHaveLength(3);
  });

  it("clears a message as its field is fixed", () => {
    component.revealAllErrors();
    component.controlFor("email").setValue("ada@example.com");
    fixture.detectChanges();

    expect(component.visibleErrors()).toEqual([
      "password is required",
      "age must be at least 18",
    ]);
  });

  it("gates the submit button on validity", () => {
    expect(submitButton().disabled).toBe(true);

    component.form.patchValue(VALID);
    fixture.detectChanges();

    expect(submitButton().disabled).toBe(false);
  });
});

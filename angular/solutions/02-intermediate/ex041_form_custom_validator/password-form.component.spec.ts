import { ComponentFixture, TestBed } from "@angular/core/testing";
import { FormControl, FormGroup } from "@angular/forms";
import {
  fieldsMatch,
  forbiddenWords,
  PasswordFormComponent,
} from "./password-form.component";

describe("forbiddenWords", () => {
  const validator = forbiddenWords(["admin", "root"]);

  const check = (value: string) => validator(new FormControl(value));

  it("passes a clean value", () => {
    // null is the success signal — anything truthy means invalid.
    expect(check("ada")).toBeNull();
  });

  it("passes an empty value", () => {
    expect(check("")).toBeNull();
  });

  it("rejects a forbidden word", () => {
    expect(check("admin")).toEqual({ forbidden: { word: "admin" } });
  });

  it("rejects a value merely containing one", () => {
    expect(check("superadmin99")).toEqual({ forbidden: { word: "admin" } });
  });

  it("ignores case", () => {
    expect(check("ROOT")).toEqual({ forbidden: { word: "root" } });
  });

  it("names the first configured word that matches", () => {
    expect(check("root-admin")).toEqual({ forbidden: { word: "admin" } });
  });

  it("is configurable per instance", () => {
    const other = forbiddenWords(["guest"]);

    expect(other(new FormControl("admin"))).toBeNull();
    expect(other(new FormControl("guest"))).toEqual({ forbidden: { word: "guest" } });
  });
});

describe("fieldsMatch", () => {
  const build = (a: string, b: string): FormGroup =>
    new FormGroup(
      { password: new FormControl(a), confirm: new FormControl(b) },
      { validators: [fieldsMatch("password", "confirm")] },
    );

  it("passes when the two agree", () => {
    expect(build("secret", "secret").errors).toBeNull();
  });

  it("fails when they differ", () => {
    expect(build("secret", "different").errors).toEqual({
      mismatch: { first: "password", second: "confirm" },
    });
  });

  it("waits until both are filled in", () => {
    // Otherwise a form reports a mismatch before anything has been typed.
    expect(build("", "").errors).toBeNull();
    expect(build("secret", "").errors).toBeNull();
    expect(build("", "secret").errors).toBeNull();
  });

  it("puts the error on the group, not the controls", () => {
    const group = build("secret", "different");

    expect(group.invalid).toBe(true);
    // The rule belongs to neither field, so neither field carries it.
    expect(group.get("password")?.errors).toBeNull();
    expect(group.get("confirm")?.errors).toBeNull();
  });
});

describe("PasswordFormComponent", () => {
  let fixture: ComponentFixture<PasswordFormComponent>;
  let component: PasswordFormComponent;

  const text = (selector: string): string =>
    (fixture.nativeElement.querySelector(selector) as HTMLElement | null)?.textContent?.trim() ??
    `MISSING ${selector}`;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [PasswordFormComponent],
    }).compileComponents();
    fixture = TestBed.createComponent(PasswordFormComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it("wires the username validator", () => {
    component.controlFor("username").setValue("admin");

    expect(component.forbiddenWord()).toBe("admin");
    expect(component.form.invalid).toBe(true);
  });

  it("accepts an allowed username", () => {
    component.controlFor("username").setValue("ada");

    expect(component.forbiddenWord()).toBeNull();
  });

  it("wires the group validator", () => {
    component.form.patchValue({ password: "secret", confirm: "different" });

    expect(component.hasMismatch()).toBe(true);
    expect(component.form.hasError("mismatch")).toBe(true);
  });

  it("is happy once they match", () => {
    component.form.patchValue({ username: "ada", password: "secret", confirm: "secret" });

    expect(component.hasMismatch()).toBe(false);
    expect(component.form.valid).toBe(true);
  });

  it("renders the mismatch message", () => {
    component.form.patchValue({ password: "secret", confirm: "different" });
    fixture.detectChanges();

    expect(text("p.mismatch")).toBe("passwords must match");

    component.form.patchValue({ confirm: "secret" });
    fixture.detectChanges();

    expect(text("p.mismatch")).toBe("");
  });

  it("keeps both kinds of failure independent", () => {
    component.form.patchValue({ username: "root", password: "a", confirm: "b" });

    expect(component.forbiddenWord()).toBe("root");
    expect(component.hasMismatch()).toBe(true);

    component.form.patchValue({ username: "ada" });

    expect(component.forbiddenWord()).toBeNull();
    // Fixing the username does not fix the group-level problem.
    expect(component.hasMismatch()).toBe(true);
    expect(component.form.invalid).toBe(true);
  });

  it("reflects the group error onto the confirm control", () => {
    component.form.patchValue({ password: "secret", confirm: "different" });
    expect(component.controlFor("confirm").errors).toBeNull();

    component.reflectMismatchOntoConfirm();

    expect(component.controlFor("confirm").hasError("mismatch")).toBe(true);
  });

  it("clears the reflected error again", () => {
    component.form.patchValue({ password: "secret", confirm: "different" });
    component.reflectMismatchOntoConfirm();
    expect(component.controlFor("confirm").hasError("mismatch")).toBe(true);

    component.form.patchValue({ confirm: "secret" });
    component.reflectMismatchOntoConfirm();

    expect(component.controlFor("confirm").errors).toBeNull();
  });
});

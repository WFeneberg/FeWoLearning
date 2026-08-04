import { ComponentFixture, TestBed } from "@angular/core/testing";
import { FormArray, FormGroup } from "@angular/forms";
import { SignupFormComponent } from "./signup-form.component";

describe("SignupFormComponent", () => {
  let fixture: ComponentFixture<SignupFormComponent>;
  let component: SignupFormComponent;

  const input = (selector: string): HTMLInputElement => {
    const found = fixture.nativeElement.querySelector(selector) as HTMLInputElement | null;
    if (found === null) {
      throw new Error(`no element matched "${selector}" — is the template implemented?`);
    }
    return found;
  };

  /**
   * patchValue through a cast.
   *
   * The typed FormGroup the solution builds and the empty one the stub starts with have
   * different value types, so an untyped patch is what lets this spec compile against both.
   */
  const patch = (changes: Record<string, unknown>): void =>
    component.form.patchValue(changes as never);

  const valueOf = (path: string): unknown => component.form.get(path)?.value;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [SignupFormComponent],
    }).compileComponents();
    fixture = TestBed.createComponent(SignupFormComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it("builds the same shape a hand-written group would", () => {
    expect(component.form.value).toEqual({
      email: "",
      profile: { firstName: "", lastName: "" },
      tags: [],
    });
  });

  it("produces real FormGroup and FormArray instances", () => {
    // Sugar, not a different kind of object.
    expect(component.form).toBeInstanceOf(FormGroup);
    expect(component.profileGroup()).toBeInstanceOf(FormGroup);
    expect(component.tagArray()).toBeInstanceOf(FormArray);
  });

  it("makes non-nullable controls", () => {
    patch({ email: "ada@example.com" });
    expect(valueOf("email")).toBe("ada@example.com");

    component.form.reset();

    // The non-nullable builder resets to the initial value; plain fb would give null here.
    expect(valueOf("email")).toBe("");
  });

  it("patches the nested group", () => {
    patch({ profile: { firstName: "Ada" } });

    expect(component.profileGroup().value).toEqual({ firstName: "Ada", lastName: "" });
  });

  it("starts with no tags", () => {
    expect(component.tagList()).toEqual([]);
    expect(component.tagArray().length).toBe(0);
  });

  it("appends tags", () => {
    component.addTag("angular");
    component.addTag("forms");

    expect(component.tagList()).toEqual(["angular", "forms"]);
    expect(component.form.value).toEqual({
      email: "",
      profile: { firstName: "", lastName: "" },
      tags: ["angular", "forms"],
    });
  });

  it("refuses a blank tag", () => {
    expect(() => component.addTag("  ")).toThrow(RangeError);
    expect(component.tagList()).toEqual([]);
  });

  it("removes a tag by index", () => {
    component.addTag("a");
    component.addTag("b");
    component.addTag("c");

    component.removeTag(1);

    expect(component.tagList()).toEqual(["a", "c"]);
  });

  it("refuses an out-of-range removal", () => {
    component.addTag("a");

    expect(() => component.removeTag(3)).toThrow(RangeError);
    expect(() => component.removeTag(-1)).toThrow(RangeError);
  });

  it("renders the tag count", () => {
    component.addTag("a");
    component.addTag("b");
    fixture.detectChanges();

    expect(
      (fixture.nativeElement.querySelector("p.tags") as HTMLElement).textContent?.trim(),
    ).toBe("2");
  });

  it("keeps an array value as an array", () => {
    // The trap: written as fb.group({pair: ["a", "b"]}) this would have become "a" with
    // "b" mistaken for a validator list.
    expect(component.pairControl().value).toEqual(["a", "b"]);
  });

  it("pushes values into the inputs", () => {
    patch({
      email: "ada@example.com",
      profile: { firstName: "Ada", lastName: "Lovelace" },
    });
    fixture.detectChanges();

    expect(input("input.email").value).toBe("ada@example.com");
    expect(input("input.first").value).toBe("Ada");
    expect(input("input.last").value).toBe("Lovelace");
  });

  it("takes typed values back", () => {
    const element = input("input.last");
    element.value = "Hopper";
    element.dispatchEvent(new Event("input"));

    expect(component.profileGroup().value.lastName).toBe("Hopper");
  });

  it("describes the filled form", () => {
    patch({
      email: "ada@example.com",
      profile: { firstName: "Ada", lastName: "Lovelace" },
    });

    expect(component.describe()).toBe("Ada Lovelace ada@example.com");
  });

  it("squeezes blanks out of the description", () => {
    patch({ profile: { firstName: "Ada" } });

    expect(component.describe()).toBe("Ada");
  });
});

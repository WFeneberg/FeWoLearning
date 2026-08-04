import { ComponentFixture, TestBed } from "@angular/core/testing";
import { AddressFormComponent } from "./address-form.component";

const FULL = {
  name: "Ada",
  address: { street: "1 Main St", city: "Springfield", zip: "12345" },
};

describe("AddressFormComponent", () => {
  let fixture: ComponentFixture<AddressFormComponent>;
  let component: AddressFormComponent;

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

  const typeInto = (selector: string, value: string): void => {
    const element = input(selector);
    element.value = value;
    element.dispatchEvent(new Event("input"));
    fixture.detectChanges();
  };

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [AddressFormComponent],
    }).compileComponents();
    fixture = TestBed.createComponent(AddressFormComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it("has a value shaped like the group", () => {
    expect(component.form.value).toEqual({
      name: "",
      address: { street: "", city: "", zip: "" },
    });
  });

  it("reaches the nested group by name", () => {
    expect(Object.keys(component.addressGroup().controls).sort()).toEqual([
      "city",
      "street",
      "zip",
    ]);
  });

  it("reaches a nested control by path", () => {
    component.controlAt("address.city").setValue("Springfield");

    expect(component.form.value).toEqual({
      name: "",
      address: { street: "", city: "Springfield", zip: "" },
    });
  });

  it("replaces every value at once", () => {
    component.replaceAll(FULL);

    expect(component.form.value).toEqual(FULL);
  });

  it("rejects an incomplete setValue", () => {
    // A complete one is accepted...
    component.replaceAll(FULL);
    expect(component.form.value).toEqual(FULL);

    // ...and setValue is strict on purpose: a missing key is a mistake, not a partial update.
    expect(() =>
      component.replaceAll({ name: "Ada" } as unknown as typeof FULL),
    ).toThrow();
  });

  it("patches a single top-level field", () => {
    component.replaceAll(FULL);
    component.applyPatch({ name: "Grace" });

    expect(component.form.value).toEqual({ ...FULL, name: "Grace" });
  });

  it("patches a single nested field", () => {
    component.replaceAll(FULL);
    component.applyPatch({ address: { city: "Shelbyville" } });

    expect(component.form.value).toEqual({
      name: "Ada",
      address: { street: "1 Main St", city: "Shelbyville", zip: "12345" },
    });
  });

  it("ignores an unknown key rather than throwing", () => {
    component.replaceAll(FULL);

    component.applyPatch({ nope: "x" } as unknown as { name?: string });

    // Lenient by design — and the reason a typo in a key name goes unnoticed.
    expect(component.form.value).toEqual(FULL);
  });

  it("pushes values into the inputs", () => {
    component.replaceAll(FULL);
    fixture.detectChanges();

    expect(input("input.name").value).toBe("Ada");
    expect(input("input.city").value).toBe("Springfield");
  });

  it("takes a typed nested value", () => {
    typeInto("input.zip", "99999");

    expect(component.controlAt("address.zip").value).toBe("99999");
  });

  it("summarises the filled form", () => {
    component.replaceAll(FULL);

    expect(component.summary()).toBe("Ada, 1 Main St, 12345 Springfield");
    fixture.detectChanges();
    expect(text("p.summary")).toBe("Ada, 1 Main St, 12345 Springfield");
  });

  it("leaves blank parts out of the summary", () => {
    component.applyPatch({ name: "Ada", address: { city: "Springfield" } });

    expect(component.summary()).toBe("Ada, Springfield");
  });

  it("summarises an empty form as nothing", () => {
    expect(component.summary()).toBe("");
  });

  it("drops a disabled control from value", () => {
    component.replaceAll(FULL);

    component.controlAt("address.zip").disable();

    // Absent, not empty. A submit handler reading `value` would silently lose the zip.
    expect(component.form.value).toEqual({
      name: "Ada",
      address: { street: "1 Main St", city: "Springfield" },
    });
  });

  it("keeps a disabled control in the raw value", () => {
    component.replaceAll(FULL);
    component.controlAt("address.zip").disable();

    expect(component.payload()).toEqual(FULL);
  });

  it("drops a whole disabled group from value", () => {
    component.replaceAll(FULL);

    component.addressGroup().disable();

    expect(component.form.value).toEqual({ name: "Ada" });
    expect(component.payload()).toEqual(FULL);
  });

  it("reports validity across the tree", () => {
    expect(component.form.valid).toBe(true);

    component.controlAt("address.city").setErrors({ required: true });

    // A child's problem is the whole form's problem.
    expect(component.form.valid).toBe(false);
    expect(component.addressGroup().valid).toBe(false);
  });
});

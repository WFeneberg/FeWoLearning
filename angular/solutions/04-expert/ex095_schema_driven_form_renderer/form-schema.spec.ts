import { FormBuilder } from "@angular/forms";
import { buildFormFromSchema, FieldSchema } from "./form-schema";

describe("buildFormFromSchema", () => {
  let fb: FormBuilder;

  beforeEach(() => {
    fb = new FormBuilder();
  });

  it("builds one control per field, with per-type default values when none is given", () => {
    const schema: FieldSchema[] = [
      { name: "title", type: "text" },
      { name: "quantity", type: "number" },
      { name: "subscribed", type: "checkbox" },
    ];

    const form = buildFormFromSchema(fb, schema);

    expect(Object.keys(form.controls).sort()).toEqual(["quantity", "subscribed", "title"]);
    expect(form.get("title")?.value).toBe("");
    expect(form.get("quantity")?.value).toBe(0);
    expect(form.get("subscribed")?.value).toBe(false);
  });

  it("uses an explicit defaultValue over the per-type default", () => {
    const schema: FieldSchema[] = [{ name: "title", type: "text", defaultValue: "Untitled" }];

    const form = buildFormFromSchema(fb, schema);

    expect(form.get("title")?.value).toBe("Untitled");
  });

  it("keeps an explicit true/false defaultValue instead of falling back (falsy is not 'missing')", () => {
    const schema: FieldSchema[] = [{ name: "subscribed", type: "checkbox", defaultValue: true }];

    const form = buildFormFromSchema(fb, schema);

    expect(form.get("subscribed")?.value).toBe(true);
  });

  it("applies a required validator", () => {
    const schema: FieldSchema[] = [{ name: "title", type: "text", validators: { required: true } }];

    const form = buildFormFromSchema(fb, schema);

    expect(form.get("title")?.valid).toBe(false);
    expect(form.get("title")?.errors).toEqual({ required: true });

    form.get("title")?.setValue("something");
    expect(form.get("title")?.valid).toBe(true);
  });

  it("applies minLength, min and max validators", () => {
    const schema: FieldSchema[] = [
      { name: "code", type: "text", validators: { minLength: 4 } },
      { name: "age", type: "number", validators: { min: 18, max: 65 } },
    ];

    const form = buildFormFromSchema(fb, schema);

    form.get("code")?.setValue("abc");
    expect(form.get("code")?.valid).toBe(false);
    form.get("code")?.setValue("abcd");
    expect(form.get("code")?.valid).toBe(true);

    form.get("age")?.setValue(10);
    expect(form.get("age")?.valid).toBe(false);
    form.get("age")?.setValue(70);
    expect(form.get("age")?.valid).toBe(false);
    form.get("age")?.setValue(30);
    expect(form.get("age")?.valid).toBe(true);
  });

  it("gives a field with no validators spec no validators at all", () => {
    const schema: FieldSchema[] = [{ name: "nickname", type: "text" }];

    const form = buildFormFromSchema(fb, schema);

    expect(form.get("nickname")?.valid).toBe(true);
  });

  it("builds two fully independent forms from two schemas — no shared state between calls", () => {
    const formA = buildFormFromSchema(fb, [{ name: "title", type: "text", defaultValue: "A" }]);
    const formB = buildFormFromSchema(fb, [{ name: "title", type: "text", defaultValue: "B" }]);

    formA.get("title")?.setValue("changed");

    expect(formA.get("title")?.value).toBe("changed");
    expect(formB.get("title")?.value).toBe("B");
  });
});

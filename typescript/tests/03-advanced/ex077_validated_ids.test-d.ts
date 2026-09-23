import { expectTypeOf, test } from "vitest";
import { assertEmail } from "@ex/03-advanced/ex077_validated_ids/index";
import type { Email } from "@ex/03-advanced/ex077_validated_ids/index";

// This row has ONE type fact. Three others were written and dropped after
// measuring them green on the untouched tree: the brands and both
// constructor signatures are given in the stub, so "a checked value flows
// where a raw one does not", "toEmail reports the branded type" and
// "describeQuota refuses raw values" are all satisfied before any work is
// done. ex076 grades the brand mechanism; what is left here is the
// validation logic, which the runtime facts carry.
//
// The assertion's narrowing is the exception, because the stub returns
// `void`.
test("assertEmail narrows to the branded type", () => {
  const value = "ada@example.com" as string;
  assertEmail(value);
  expectTypeOf(value).toEqualTypeOf<Email>();
});

import { expectTypeOf, test } from "vitest";
import { HardSecret } from "@ex/01-beginner/ex025_private_hash_fields/index";

test("isHardSecret narrows its argument", () => {
  const candidate: unknown = new HardSecret("x");
  if (HardSecret.isHardSecret(candidate)) {
    expectTypeOf(candidate).toEqualTypeOf<HardSecret>();
  }
});

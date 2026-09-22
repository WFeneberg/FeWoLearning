import { expectTypeOf, test } from "vitest";
import type {
  OmittableKeys,
  Settings,
} from "@ex/01-beginner/ex005_optional_and_readonly/index";

// toEqualTypeOf compares readonly and optional modifiers, not just the
// property types, so this one assertion grades all three declarations.
test("Settings declares readonly, optional and present-but-undefined", () => {
  expectTypeOf<Settings>().toEqualTypeOf<{
    readonly id: string;
    theme?: "light" | "dark";
    nickname: string | undefined;
  }>();
});

// The discriminating fact: `nickname` is also inhabited by undefined, so any
// derivation that merely asks "can this be undefined?" would name it too.
test("only the optional property may be omitted", () => {
  expectTypeOf<OmittableKeys>().toEqualTypeOf<"theme">();
});

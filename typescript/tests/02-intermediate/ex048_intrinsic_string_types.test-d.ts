import { expectTypeOf, test } from "vitest";
import type { EnvVar, HandlerName, Handlers } from "@ex/02-intermediate/ex048_intrinsic_string_types/index";
import { handlerName } from "@ex/02-intermediate/ex048_intrinsic_string_types/index";

test("HandlerName capitalises inside the template", () => {
  expectTypeOf<HandlerName<"click">>().toEqualTypeOf<"onClick">();
});

test("HandlerName touches only the first character", () => {
  expectTypeOf<HandlerName<"mouseDown">>().toEqualTypeOf<"onMouseDown">();
});

test("EnvVar uppercases the whole key", () => {
  expectTypeOf<EnvVar<"port">>().toEqualTypeOf<"APP_PORT">();
  expectTypeOf<EnvVar<"logLevel">>().toEqualTypeOf<"APP_LOGLEVEL">();
});

// Composing an intrinsic, a template and a key remap: the handler type
// cannot drift from the data type, because it is derived from it.
test("Handlers renames every key and types each callback from its value", () => {
  expectTypeOf<Handlers<{ click: number; focus: string }>>().toEqualTypeOf<{
    onClick: (value: number) => void;
    onFocus: (value: string) => void;
  }>();
});

test("handlerName reports the exact literal", () => {
  expectTypeOf(handlerName("click")).toEqualTypeOf<"onClick">();
});

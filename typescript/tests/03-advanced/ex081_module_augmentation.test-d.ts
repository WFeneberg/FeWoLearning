import { expectTypeOf, test } from "vitest";
import type { Request } from "@ex/03-advanced/ex081_module_augmentation/library";

// Imported from ./library, which declares neither member. They are there
// only because ./index augmented that module.
test("Request carries the augmented members", () => {
  expectTypeOf<Request["traceId"]>().toEqualTypeOf<string | undefined>();
  expectTypeOf<Request["startedAt"]>().toEqualTypeOf<number | undefined>();
});

test("the library's own members survive the augmentation", () => {
  expectTypeOf<keyof Request>().toEqualTypeOf<"url" | "headers" | "traceId" | "startedAt">();
});

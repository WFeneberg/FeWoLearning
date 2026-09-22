import { expectTypeOf, test } from "vitest";
import type { Config, Id } from "@ex/01-beginner/ex002_type_vs_interface/index";

test("the two declarations merge into one Config", () => {
  expectTypeOf<keyof Config>().toEqualTypeOf<"host" | "port">();
});

test("the merged-in property keeps its own type", () => {
  expectTypeOf<Config["port"]>().toEqualTypeOf<number>();
});

// Deliberately the whole shape, not `Config["host"]` on its own: the stub
// already declares `host: string`, so that narrower fact would be green
// before the merge was written — it grades the stub's signature, not the work.
test("the first declaration survives alongside the second", () => {
  expectTypeOf<Config>().toEqualTypeOf<{ host: string; port: number }>();
});

test("Id is a union, which no interface could express", () => {
  expectTypeOf<Id>().toEqualTypeOf<string | number>();
});

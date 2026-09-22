import { expectTypeOf, test } from "vitest";
import { Account } from "@ex/01-beginner/ex021_class_basics/index";

test("id is a string", () => {
  expectTypeOf<Account["id"]>().toEqualTypeOf<string>();
});

// Red while the stub's `id` is writable: the assignment compiles, which
// makes the expect-error unused, which is itself an error.
test("id cannot be reassigned", () => {
  const account = new Account("a-1", 100);
  // @ts-expect-error — id is readonly
  account.id = "a-2";
});

// `keyof` yields only the public keys, so a private field drops out of it
// entirely. On the stub, `balance` is public and this resolves to true.
test("balance is not part of the public type", () => {
  expectTypeOf<"balance" extends keyof Account ? true : false>().toEqualTypeOf<false>();
});

test("the public surface is exactly id and the three operations", () => {
  expectTypeOf<keyof Account>().toEqualTypeOf<
    "id" | "getBalance" | "deposit" | "withdraw"
  >();
});

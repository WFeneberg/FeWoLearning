import { expectTypeOf, test } from "vitest";
import { Container, token } from "@ex/03-advanced/ex085_tiny_di_container/index";

interface Clock {
  now(): number;
}

const CLOCK = token<Clock>("Clock");
const GREETING = token<string>("Greeting");

// Three further facts were written and dropped after measuring them
// green on the untouched tree: `Token<T>`, `token<T>` and `register`'s
// `this` return are all given in the stub, because the runtime tests
// cannot compile without them. What is left is the part the stub gets
// wrong — register, registerSingleton and resolve all take `unknown`.

// No cast at the call site: the return type is read out of the token.
test("resolve reports the token's own type", () => {
  const container = new Container();
  expectTypeOf(container.resolve(CLOCK)).toEqualTypeOf<Clock>();
  expectTypeOf(container.resolve(GREETING)).toEqualTypeOf<string>();
});

test("a factory must produce what its token promises", () => {
  // @ts-expect-error — CLOCK is a Token<Clock>, not a Token<string>
  new Container().register(CLOCK, () => "not a clock");
});

test("a singleton factory is checked the same way", () => {
  // @ts-expect-error — GREETING is a Token<string>
  new Container().registerSingleton(GREETING, () => 42);
});

// Exercise 082 — mixins (advanced).
// Goal:   add behaviour to a class from outside, composably, with the
//         types following.
// Drills: a constructor type as a constraint, a class expression as a
//         return value, inferring the combined instance type.
// Passes: each mixin adds its own members, two of them compose, and the
//         base class's own members survive.
//
// This row replaces the class-decorator one the catalog originally
// planned. Standard TC39 decorators do not survive this track's
// transform — measured: Vite 8's Rolldown/oxc pipeline leaves the `@`
// syntax in the output and the module fails to load with a SyntaxError,
// with no configuration found that changes it. The mixin pattern is what
// TypeScript codebases use for the same job anyway, and unlike a
// decorator it is entirely ordinary code.
//
// The pattern is a function taking a class and returning a subclass:
//
//   type Constructor<T = object> = new (...args: any[]) => T;
//   function Timestamped<TBase extends Constructor>(Base: TBase) {
//     return class extends Base { … };
//   }
//
// Two details carry it. The constraint must be a CONSTRUCTOR type, since
// `extends Base` needs something constructible. And the return type is
// left to inference: writing it out means naming an anonymous class
// expression, which cannot be done — so the mixin's signature has no
// return annotation, and the combined instance type is inferred at each
// call.
//
// The constructor parameter list is the one place this track allows a
// loose type. A mixin must forward whatever its base takes, and
// `...args: any[]` is what the language requires here — `never[]` and
// `unknown[]` both make `super(...args)` fail.

/** A constructible thing. */
export type Constructor<T = object> = new (...args: any[]) => T;

export class Entity {
  constructor(public readonly id: string) {}

  describe(): string {
    return `entity:${this.id}`;
  }
}

/** TODO: add a `createdAt` set from `Date.now` at construction, and a
 *  `age(now)` returning the difference. */
export function Timestamped<TBase extends Constructor>(_Base: TBase) {
  throw new Error("TODO: implement Timestamped");
}

/** TODO: add a `tags` array, an `addTag(tag)` returning `this` for
 *  chaining, and a `hasTag(tag)`. */
export function Taggable<TBase extends Constructor>(_Base: TBase) {
  throw new Error("TODO: implement Taggable");
}

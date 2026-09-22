// Exercise 044 — rebuilding ReturnType and Parameters (intermediate).
// Goal:   read a function's signature apart with `infer`.
// Drills: inferring a return type, inferring the parameter tuple, and
//         composing two extractions for something the library lacks.
// Passes: all three answer for functions of any arity and fall back to
//         never for what is not a function.
//
// Both built-ins are one conditional each. ReturnType is
// `T extends (...args: any) => infer R ? R : never`, and Parameters infers
// the whole parameter list as a TUPLE — which is how it keeps one type per
// position (ex006) rather than flattening to a union.
//
// The pattern uses `(...args: never[])` here rather than the library's
// `any`. `never` is the correct variance for a parameter position you do
// not care about, and this track has no `any` in it.
//
// AsyncReturnType is the one the standard library does not provide, and the
// one worth having: for `() => Promise<User>` you almost never want
// `Promise<User>`, you want `User`. Compose your own ReturnType with
// ex040's Resolved idea.
//
// Type-level only; there is nothing to run.

/** TODO: what T returns; never if T is not a function. */
export type MyReturnType<T> = unknown;

/** TODO: T's parameter list as a tuple; never if T is not a function. */
export type MyParameters<T> = unknown;

/** TODO: what T's returned promise resolves to. For a function that does
 *  not return a promise, its plain return type. never if not a function. */
export type AsyncReturnType<T> = unknown;

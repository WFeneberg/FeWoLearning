// Exercise 010 — overload signatures (beginner).
// Goal:   let one function report a different result type per argument type.
// Drills: overload signatures, the implementation signature, why the latter
//         is not part of the public contract.
// Passes: parse("a,b") is string[] and parse(407) is number[] — not the union
//         of both — and the implementation signature is not callable.
//
// Unlike C#, there is one function object here: the overloads are declarations
// stacked above a single implementation whose signature must be compatible
// with all of them. The implementation signature is deliberately invisible to
// callers, which is why passing a `string | number` matches no overload even
// though the body handles it.

/**
 * TODO: declare two overload signatures directly above the implementation —
 * a string argument yields string[], a number argument yields number[] —
 * then implement the body.
 */
export function parse(_input: string | number): string[] | number[] {
  throw new Error("TODO: implement parse");
}

// Exercise 017 — scope and hoisting (beginner).
// Goal:   know what exists before the line that declares it.
// Drills: var is function-scoped, let/const are block-scoped with a
//         temporal dead zone, function declarations hoist whole, `typeof`
//         on a name that was never declared.
// Passes: four functions that each report one of those behaviours, none of
//         which is a bug.

/**
 * Declares `message` with `var` INSIDE an if-block and returns it from
 * outside that block. `var` has no block scope, so this works.
 *
 * Return the string "from block".
 *
 * TODO: implement.
 */
export function varSurvivesBlock() {
  throw new Error("TODO: implement varSurvivesBlock");
}

/**
 * Reads a `let` binding one line BEFORE its declaration, inside a
 * try/catch, and returns the caught error's `name`.
 *
 * The binding exists from the top of the block but cannot be touched until
 * its declaration runs — the temporal dead zone. Return "ReferenceError".
 *
 * TODO: implement.
 */
export function tdzErrorName() {
  throw new Error("TODO: implement tdzErrorName");
}

/**
 * Calls a function declaration that appears LATER in its own body and
 * returns the result ("hoisted"). A function declaration is hoisted with
 * its body; `const helper = () => …` would not be.
 *
 * TODO: implement.
 */
export function callsBeforeDeclaration() {
  throw new Error("TODO: implement callsBeforeDeclaration");
}

/**
 * Returns `typeof` a name that is not declared anywhere. This is the one
 * expression allowed to touch an undeclared name without throwing — the
 * answer is the string "undefined".
 *
 * TODO: implement without try/catch.
 */
export function typeofUndeclared() {
  throw new Error("TODO: implement typeofUndeclared");
}

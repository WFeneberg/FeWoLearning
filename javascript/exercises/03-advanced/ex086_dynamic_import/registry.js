// Support module for exercise 086 — NOT a TODO, identical in both trees.
// A module with state, to show that a module is evaluated ONCE however
// many times it is imported.

export const evaluatedAt = [Date.now()];

let calls = 0;

export function bump() {
  calls += 1;
  return calls;
}

export function callCount() {
  return calls;
}

// Reference solution — exercise 026.
import type { Unit } from "./units";

export type { Unit };

// A renamed re-export: no wrapper function, no second place for the
// arithmetic to drift.
export { toMillimetres as mmFrom } from "./units";

export function formatLength(value: number, unit: Unit): string {
  return `${value}${unit}`;
}

// Module state. Evaluated once; every importer sees the same counter.
let idCounter = 0;

export function nextId(): string {
  idCounter += 1;
  return `id-${idCounter}`;
}

export function resetIds(): void {
  idCounter = 0;
}

export default formatLength;

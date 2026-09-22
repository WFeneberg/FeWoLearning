// Given, for ex026 to consume. Do not change.
export type Unit = "mm" | "cm" | "in";

export const MM_PER: Readonly<Record<Unit, number>> = {
  mm: 1,
  cm: 10,
  in: 25.4,
};

export function toMillimetres(value: number, unit: Unit): number {
  return value * MM_PER[unit];
}

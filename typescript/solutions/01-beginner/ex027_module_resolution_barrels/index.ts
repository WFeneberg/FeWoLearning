// Reference solution — exercise 027, the barrel.
export * from "./shapes";

// Explicit rather than a second `export *`, so the clashing name can be
// renamed instead of silently dropping out of the barrel.
export { describe as describeColour, isWarm } from "./colours";
export type { Colour } from "./colours";

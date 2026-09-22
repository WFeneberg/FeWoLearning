// Given, for ex027's barrel to re-export. Do not change.
//
// Note that this module also exports a `describe`. That collision is the
// point of the exercise: two `export *` lines cannot both contribute it.
export type Colour = "red" | "green" | "blue";

export function describe(colour: Colour): string {
  return `colour:${colour}`;
}

export function isWarm(colour: Colour): boolean {
  return colour === "red";
}

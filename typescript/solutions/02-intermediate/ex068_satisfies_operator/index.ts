// Reference solution — exercise 068.
export type Palette = Record<string, string | readonly [number, number, number]>;

// Checked against Palette, typed as what it is.
export const palette = {
  red: [255, 0, 0],
  green: "#00ff00",
} satisfies Palette;

export function toCss(name: keyof typeof palette): string {
  const value = palette[name];
  if (typeof value === "string") {
    return value;
  }
  return `rgb(${value[0]}, ${value[1]}, ${value[2]})`;
}

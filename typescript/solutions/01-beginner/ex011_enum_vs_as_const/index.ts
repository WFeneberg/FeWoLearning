// Reference solution — exercise 011.
export enum Direction {
  Up,
  Down,
  Left,
  Right,
}

export function enumNames(enumObject: Record<string, string | number>): string[] {
  // The reverse-mapping keys are the numeric ones; a member name never parses
  // as a number.
  return Object.keys(enumObject).filter((key) => Number.isNaN(Number(key)));
}

export const Compass = {
  Up: "up",
  Down: "down",
  Left: "left",
  Right: "right",
} as const;

export type CompassPoint = (typeof Compass)[keyof typeof Compass];

export function opposite(point: CompassPoint): CompassPoint {
  switch (point) {
    case "up":
      return "down";
    case "down":
      return "up";
    case "left":
      return "right";
    case "right":
      return "left";
  }
}

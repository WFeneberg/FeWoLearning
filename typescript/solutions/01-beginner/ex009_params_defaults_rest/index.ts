// Reference solution — exercise 009.
export function css(value: number, unit = "px", ...extras: string[]): string {
  return [`${value}${unit}`, ...extras].join(" ");
}

export function labelAll(prefix: string, ...items: string[]): string[] {
  return items.map((item) => `${prefix}${item}`);
}

// Reference solution — exercise 015.
export type AppEvent =
  | { type: "click"; x: number; y: number }
  | { type: "key"; key: string }
  | { type: "scroll"; delta: number };

export function render(event: AppEvent): string {
  switch (event.type) {
    case "click":
      return `click@${event.x},${event.y}`;
    case "key":
      return `key:${event.key}`;
    case "scroll":
      return `scroll${event.delta >= 0 ? "+" : ""}${event.delta}`;
  }
}

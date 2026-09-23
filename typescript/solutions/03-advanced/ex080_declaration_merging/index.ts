// Reference solution — exercise 080.
let next = 1;

export function counter(): number {
  const value = next;
  next += 1;
  return value;
}

// Merged into the function above: its exports become properties of it.
export namespace counter {
  export const start = 1;

  export function reset(): void {
    next = start;
  }
}

export class Widget {
  constructor(public readonly name: string) {}

  describe(): string {
    return `<${this.name}>`;
  }
}

// Merged into the class: this adds `render` to Widget's TYPE. Nothing
// checks that an implementation exists, so the assignment below is the
// other half of the promise.
export interface Widget {
  render(): string;
}

Widget.prototype.render = function render(this: Widget): string {
  return `[${this.name}]`;
};

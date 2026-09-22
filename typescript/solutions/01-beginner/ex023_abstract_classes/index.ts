// Reference solution — exercise 023.
export abstract class Shape {
  abstract area(): number;

  protected abstract name(): string;

  // Written once here; both subclasses inherit it.
  describe(): string {
    return `${this.name()}: ${this.area().toFixed(2)}`;
  }
}

export class Circle extends Shape {
  constructor(public readonly radius: number) {
    super();
  }

  area(): number {
    return Math.PI * this.radius ** 2;
  }

  protected name(): string {
    return "circle";
  }
}

export class Rect extends Shape {
  constructor(
    public readonly width: number,
    public readonly height: number,
  ) {
    super();
  }

  area(): number {
    return this.width * this.height;
  }

  protected name(): string {
    return "rect";
  }
}

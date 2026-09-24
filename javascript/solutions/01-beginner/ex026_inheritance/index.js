// Reference solution — exercise 026.

export class Shape {
  constructor(name) {
    this.name = name;
  }

  area() {
    return 0;
  }

  describe() {
    // this.area() is looked up on the instance's prototype chain at call
    // time, so a subclass's override wins even from here.
    return `${this.name} has area ${this.area()}`;
  }

  static describeAll(shapes) {
    return [...shapes].map((shape) => shape.describe());
  }
}

export class Rectangle extends Shape {
  constructor(width, height) {
    super("rectangle");
    this.width = width;
    this.height = height;
  }

  area() {
    return this.width * this.height;
  }
}

export class Square extends Rectangle {
  constructor(side) {
    super(side, side);
    this.name = "square";
  }

  describe() {
    return `[sq] ${super.describe()}`;
  }
}

export function errorFromTouchingThisFirst() {
  class Broken extends Shape {
    constructor() {
      this.tooEarly = true;
      super("broken");
    }
  }
  try {
    // eslint-disable-next-line no-new
    new Broken();
    return "no error";
  } catch (error) {
    return error.name;
  }
}

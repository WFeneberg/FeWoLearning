// Reference solution — exercise 046.

export function Point(x, y) {
  // NOT new.target: Point3D calls this function with Point.call(this, …),
  // where new.target is undefined although the call is a real construction.
  // `this instanceof Point` is true there, and false for a bare call —
  // where `this` is undefined, this being strict-mode code.
  if (!(this instanceof Point)) throw new TypeError("Point requires new");
  this.x = x;
  this.y = y;
}

Point.prototype.toString = function toString() {
  return `(${this.x}, ${this.y})`;
};

Point.prototype.distanceTo = function distanceTo(other) {
  return Math.hypot(other.x - this.x, other.y - this.y);
};

export function inherit(Child, Parent) {
  Child.prototype = Object.create(Parent.prototype);
  // The built-in `constructor` is non-enumerable; match it.
  Object.defineProperty(Child.prototype, "constructor", {
    value: Child,
    writable: true,
    enumerable: false,
    configurable: true,
  });
}

export function Point3D(x, y, z) {
  if (!(this instanceof Point3D)) throw new TypeError("Point3D requires new");
  Point.call(this, x, y);
  this.z = z;
}

inherit(Point3D, Point);

Point3D.prototype.toString = function toString() {
  return `(${this.x}, ${this.y}, ${this.z})`;
};

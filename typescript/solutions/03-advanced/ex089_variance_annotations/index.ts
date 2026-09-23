// Reference solution — exercise 089.
export interface Animal {
  name: string;
}

export interface Dog extends Animal {
  name: string;
  breed: string;
}

// Output position only: covariant.
export interface Producer<out T> {
  get(): T;
}

// Input position only: contravariant. A property of function type, not a
// method — method syntax is bivariant (ex066) and would break this.
export interface Consumer<in T> {
  accept: (value: T) => void;
}

// Both positions: invariant.
export interface Box<in out T> {
  get(): T;
  set: (value: T) => void;
}

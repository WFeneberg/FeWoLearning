// Reference solution — exercise 073.
export type DeepFreeze<T> = T extends (...args: never[]) => unknown
  ? T
  : T extends object
    ? // Homomorphic and array-aware: a tuple stays a tuple of the same
      // length, an array stays an array, an object stays an object. No
      // separate array branch is needed, and adding one is what loses
      // the arity.
      { readonly [K in keyof T]: DeepFreeze<T[K]> }
    : T;

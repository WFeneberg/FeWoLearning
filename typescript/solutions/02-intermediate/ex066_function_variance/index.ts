// Reference solution — exercise 066.
export interface Animal {
  name: string;
}

export interface Dog extends Animal {
  name: string;
  breed: string;
}

export type HandlerAssignable<A, B> = ((a: A) => void) extends (b: B) => void
  ? true
  : false;

export type PropertyAssignable<A, B> = { handle: (a: A) => void } extends {
  handle: (b: B) => void;
}
  ? true
  : false;

// Method syntax on BOTH sides. This is the bivariant hole: it answers true
// in the direction the property form rejects.
export type MethodAssignable<A, B> = { handle(a: A): void } extends {
  handle(b: B): void;
}
  ? true
  : false;

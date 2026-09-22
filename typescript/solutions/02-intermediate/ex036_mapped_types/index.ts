// Reference solution — exercise 036.
export interface User {
  id: string;
  age: number;
  active: boolean;
}

export interface Box<V> {
  value: V;
}

export type Stringify<T> = { [K in keyof T]: string };

// T[K] keeps each key's own type, so the boxes differ per property.
export type Boxed<T> = { [K in keyof T]: Box<T[K]> };

export function stringifyAll(user: User): Stringify<User> {
  return {
    id: String(user.id),
    age: String(user.age),
    active: String(user.active),
  };
}

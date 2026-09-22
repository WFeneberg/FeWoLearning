// Reference solution — exercise 001.
export interface Named {
  name: string;
}

export function greet(target: Named): string {
  return `Hello, ${target.name}!`;
}

export type IsNamed<S> = S extends Named ? true : false;

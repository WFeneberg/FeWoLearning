// Reference solution — exercise 079.
export interface AppEvents {
  click: { x: number; y: number };
  key: { code: string };
  close: void;
}

export class Emitter<E> {
  private readonly handlers = new Map<keyof E, ((payload: never) => void)[]>();

  on<K extends keyof E>(name: K, handler: (payload: E[K]) => void): void {
    const existing = this.handlers.get(name) ?? [];
    existing.push(handler as (payload: never) => void);
    this.handlers.set(name, existing);
  }

  // The variadic tuple is what makes a void event take no second
  // argument while every other event requires one.
  emit<K extends keyof E>(
    name: K,
    ...args: E[K] extends void ? [] : [payload: E[K]]
  ): void {
    for (const handler of this.handlers.get(name) ?? []) {
      (handler as (payload: unknown) => void)(args[0]);
    }
  }

  countFor<K extends keyof E>(name: K): number {
    return (this.handlers.get(name) ?? []).length;
  }
}

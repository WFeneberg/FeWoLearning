// Reference solution — exercise 048.
export type HandlerName<K extends string> = `on${Capitalize<K>}`;

export type EnvVar<K extends string> = `APP_${Uppercase<K>}`;

export type Handlers<T> = {
  [K in keyof T as HandlerName<string & K>]: (value: T[K]) => void;
};

export function handlerName<K extends string>(key: K): HandlerName<K> {
  // The intrinsics exist only in the type system; the runtime has to do
  // the same work by hand.
  return `on${key.charAt(0).toUpperCase()}${key.slice(1)}` as HandlerName<K>;
}

// Reference solution — exercise 011.

export function makeUser(name, age, source) {
  return { name, age, meta: { source } };
}

export function tag(prefix, id, value) {
  return { [`${prefix}:${id}`]: value };
}

export function orderedKeys(object) {
  return Object.keys(object);
}

export function makeConfig(env, port) {
  return { env, server: { port, host: "localhost" }, features: [] };
}

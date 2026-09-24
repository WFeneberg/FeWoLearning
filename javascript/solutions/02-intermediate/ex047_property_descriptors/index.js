// Reference solution — exercise 047.

export function defineConstant(object, key, value) {
  return Object.defineProperty(object, key, {
    value,
    writable: false,
    enumerable: true,
    configurable: false,
  });
}

export function defineHidden(object, key, value) {
  return Object.defineProperty(object, key, {
    value,
    writable: true,
    enumerable: false,
    configurable: true,
  });
}

export function flagsOf(object, key) {
  const descriptor = Object.getOwnPropertyDescriptor(object, key);
  if (descriptor === undefined) return null;
  const { writable, enumerable, configurable } = descriptor;
  return { writable, enumerable, configurable };
}

export function definedVsAssigned() {
  const definedOn = Object.defineProperty({}, "key", { value: 1 });
  const assignedOn = { key: 1 };
  return { defined: flagsOf(definedOn, "key"), assigned: flagsOf(assignedOn, "key") };
}

export function writeToReadOnly(object, key, value) {
  try {
    object[key] = value;
    return "no error";
  } catch (error) {
    return error.name;
  }
}

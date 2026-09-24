// Reference solution — exercise 049.

export function clone(value) {
  return structuredClone(value);
}

export function cloneWithCycle() {
  const original = { name: "self" };
  original.self = original;
  return structuredClone(original);
}

export function cloneUncloneable(value) {
  try {
    structuredClone(value);
    return "no error";
  } catch (error) {
    return error.name;
  }
}

export function cloneLosesPrototype() {
  class Tagged {
    constructor(value) {
      this.value = value;
    }

    describe() {
      return `tagged ${this.value}`;
    }
  }
  const copy = structuredClone(new Tagged(7));
  return {
    isTagged: copy instanceof Tagged,
    hasDescribe: typeof copy.describe === "function",
    value: copy.value,
  };
}

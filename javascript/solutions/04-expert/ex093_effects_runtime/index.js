// Reference solution — exercise 093.

export function effect(type, payload) {
  return { type, payload };
}

export async function run(program, handlers) {
  let input = { value: undefined, kind: "next" };
  while (true) {
    // The generator is resumed either with a result or with an error; both
    // paths go through its own try/catch and finally blocks.
    const step =
      input.kind === "next" ? program.next(input.value) : program.throw(input.value);
    if (step.done) return step.value;

    const { type, payload } = step.value;
    const handle = handlers[type];
    if (handle === undefined) {
      // Let the generator clean up, then fail the run.
      program.return(undefined);
      throw new RangeError(`unknown effect: ${type}`);
    }
    try {
      input = { kind: "next", value: await handle(payload) };
    } catch (error) {
      input = { kind: "throw", value: error };
    }
  }
}

export function* describeUser(id) {
  const user = yield effect("readUser", id);
  let orders;
  try {
    orders = yield effect("readOrders", id);
  } catch {
    return `${user.name} has unknown orders`;
  }
  return `${user.name} has ${orders.length} orders`;
}

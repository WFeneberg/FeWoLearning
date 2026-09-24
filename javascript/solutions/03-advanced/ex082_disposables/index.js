// Reference solution — exercise 082.

export function makeResource(name, log) {
  return {
    name,
    disposed: false,
    [Symbol.dispose]() {
      this.disposed = true;
      log.push(name);
    },
  };
}

export function withResources(resources, body) {
  const stack = new DisposableStack();
  try {
    for (const resource of resources) stack.use(resource);
    return body(stack);
  } finally {
    stack.dispose(); // LIFO, and it runs on the throwing path too
  }
}

export function buildStack(value, onRelease, log) {
  const stack = new DisposableStack();
  stack.adopt(value, (held) => onRelease(held));
  stack.defer(() => log.push("deferred"));
  return stack;
}

export function transferOwnership(stack) {
  const moved = stack.move();
  stack.dispose();
  return { moved, oldDisposed: stack.disposed };
}

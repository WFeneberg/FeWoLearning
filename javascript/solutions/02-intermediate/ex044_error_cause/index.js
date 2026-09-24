// Reference solution — exercise 044.

export function wrapErrors(fn, message) {
  try {
    return fn();
  } catch (error) {
    throw new Error(message, { cause: error });
  }
}

export function rootCause(error) {
  let current = error;
  while (current?.cause !== undefined) current = current.cause;
  return current;
}

export function causeMessages(error) {
  const messages = [];
  for (let current = error; current !== undefined; current = current.cause) {
    messages.push(current.message);
  }
  return messages;
}

export function runAll(tasks, message) {
  const results = [];
  const errors = [];
  for (const task of tasks) {
    try {
      results.push(task());
    } catch (error) {
      errors.push(error);
    }
  }
  if (errors.length > 0) throw new AggregateError(errors, message);
  return results;
}

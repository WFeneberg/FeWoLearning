// Reference solution — exercise 083.

export function* cleaningProducer(values, log) {
  try {
    for (const value of values) yield value;
  } finally {
    // Runs on normal completion AND on return()/throw() from outside.
    log.push("cleanup");
  }
}

export function pullThenReturn(generator, count) {
  const values = [];
  for (let i = 0; i < count; i++) {
    const { value, done } = generator.next();
    if (done) break;
    values.push(value);
  }
  return { values, result: generator.return("stopped") };
}

export function* resilient() {
  let n = 1;
  while (true) {
    try {
      yield n++;
    } catch (error) {
      yield `caught: ${error.message}`;
    }
  }
}

export function throwInto(generator, error) {
  try {
    return { yielded: generator.throw(error).value };
  } catch {
    return "propagated";
  }
}

// Reference solution — exercise 063.

export function* turnstile() {
  let state = "locked";
  while (true) {
    // The state is this generator's local variable; suspending at the yield
    // is what keeps it.
    const event = yield state;
    if (state === "locked" && event === "coin") state = "open";
    else if (state === "open" && event === "push") state = "locked";
  }
}

export function drive(events) {
  const machine = turnstile();
  const states = [machine.next().value];
  for (const event of events) states.push(machine.next(event).value);
  return states;
}

export function countTransitions(events) {
  const states = drive(events);
  return states.filter((state, index) => index > 0 && state !== states[index - 1]).length;
}

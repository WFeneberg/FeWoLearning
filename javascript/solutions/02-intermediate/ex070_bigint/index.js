// Reference solution — exercise 070.

export function factorial(n) {
  let result = 1n;
  for (let i = 2n; i <= n; i++) result *= i;
  return result;
}

export function divide(a, b) {
  return a / b; // BigInt division truncates towards zero
}

export function mixingError() {
  try {
    const mixed = 1n + 1;
    return "no error";
  } catch (error) {
    return error.name;
  }
}

export function comparisons() {
  return { loose: 1n == 1, strict: 1n === 1, greater: 2n > 1 };
}

export function serializeBig(value) {
  return JSON.stringify(value, (_key, current) =>
    typeof current === "bigint" ? current.toString() : current,
  );
}

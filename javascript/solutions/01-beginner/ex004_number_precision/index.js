// Reference solution — exercise 004.

export function nearlyEqual(a, b, epsilon = Number.EPSILON) {
  if (a === b) return true; // also the only way Infinity compares equal
  // NaN and a mismatched infinity are out: |Inf - -Inf| <= Inf is true, so
  // the tolerance test below would call those two "nearly equal".
  if (!Number.isFinite(a) || !Number.isFinite(b)) return false;
  // Scale the tolerance with the magnitude: one ulp near 1e16 is 2.0, not
  // 2.2e-16, so a fixed epsilon is useless away from 1.0.
  return Math.abs(a - b) <= epsilon * Math.max(1, Math.abs(a), Math.abs(b));
}

export function formatMoney(value) {
  return value.toFixed(2);
}

export function sumAmounts(amounts) {
  const cents = amounts.reduce((total, amount) => total + Math.round(amount * 100), 0);
  return cents / 100;
}

export function isExactInteger(value) {
  return Number.isSafeInteger(value);
}

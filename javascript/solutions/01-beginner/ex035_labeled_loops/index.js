// Reference solution — exercise 035.

export function findInGrid(grid, target) {
  let found = null;
  search: for (let row = 0; row < grid.length; row++) {
    for (let col = 0; col < grid[row].length; col++) {
      if (grid[row][col] === target) {
        found = { row, col };
        break search; // leaves BOTH loops
      }
    }
  }
  return found;
}

export function sumCleanRows(grid) {
  let total = 0;
  rows: for (const row of grid) {
    let rowTotal = 0;
    for (const value of row) {
      if (value < 0) continue rows; // abandon the row, keep nothing of it
      rowTotal += value;
    }
    total += rowTotal;
  }
  return total;
}

export function firstCommonValue(lists) {
  const [first = [], ...rest] = lists;
  candidates: for (const candidate of first) {
    for (const list of rest) {
      if (!list.includes(candidate)) continue candidates;
    }
    return candidate;
  }
  return null;
}

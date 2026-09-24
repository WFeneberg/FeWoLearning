// Flips catalog rows ⬜ -> ✅ and refreshes the Status line.
// Usage: node tools/flip.mjs 001 005
import { readFileSync, writeFileSync } from "node:fs";

const [from, to] = process.argv.slice(2);
const path = new URL("../catalog.md", import.meta.url);
let text = readFileSync(path, "utf8");

for (let n = Number(from); n <= Number(to); n++) {
  const id = String(n).padStart(3, "0");
  const before = text;
  text = text.replace(new RegExp(`^(\\| ${id} \\|.*)⬜(.*)$`, "m"), "$1✅$2");
  if (text === before) throw new Error(`row ${id} was not ⬜`);
}

const done = (text.match(/✅ \|/g) ?? []).length;
text = text.replace(
  /^\*\*Status:.*$/m,
  done === 100
    ? "**Status: 100 ✅ / 0 ⬜** — complete."
    : `**Status: ${done} ✅ / ${100 - done} ⬜**`,
);
writeFileSync(path, text);
console.log(`catalog: ${done} done`);

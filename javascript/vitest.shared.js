// Shared config factory. `tests/` is written once and imports every exercise
// through the `@ex` alias; this file is what points that alias at either
// content tree. `vitest.config.js` targets exercises/ (the red run),
// `vitest.solutions.config.js` targets solutions/ (the green run) — the same
// mechanism `typescript/` uses, and for the same reason: one suite grades
// both, so `solutions/` cannot drift silently.
//
// Two thin configs rather than a USE_SOLUTIONS env var, because an inline
// `VAR=value` prefix in an npm script does not work on Windows without
// cross-env, and this track adds no dependency for that.
import { fileURLToPath } from "node:url";
import { defineConfig } from "vitest/config";

export function makeConfig(target) {
  return defineConfig({
    resolve: {
      alias: {
        "@ex": fileURLToPath(new URL(`./${target}`, import.meta.url)),
      },
    },
    test: {
      include: ["tests/**/*.test.js"],
    },
  });
}

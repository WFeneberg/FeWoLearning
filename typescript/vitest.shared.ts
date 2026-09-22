// Shared config factory. `tests/` is written once and imports every exercise
// through the `@ex` alias; this file is what points that alias at either
// content tree. `vitest.config.ts` targets exercises/ (the red run),
// `vitest.solutions.config.ts` targets solutions/ (the green run).
//
// Two thin configs rather than a USE_SOLUTIONS env var, because an inline
// `VAR=value` prefix in an npm script does not work on Windows without
// cross-env, and this track adds no dependency for that.
import { fileURLToPath } from "node:url";
import { defineConfig } from "vitest/config";

export type Target = "exercises" | "solutions";

export function makeConfig(target: Target) {
  return defineConfig({
    resolve: {
      alias: {
        "@ex": fileURLToPath(new URL(`./${target}`, import.meta.url)),
      },
    },
    test: {
      include: ["tests/**/*.test.ts"],
      // Type-level facts live in *.test-d.ts and are graded by tsc. Enabled
      // here rather than behind the --typecheck flag, so a plain `npm test`
      // runs runtime and type facts together.
      typecheck: {
        enabled: true,
        include: ["tests/**/*.test-d.ts"],
        tsconfig: `./tsconfig.${target}.json`,
      },
    },
  });
}

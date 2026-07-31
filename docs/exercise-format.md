# Exercise Format & Conventions

Every exercise across every language follows the same conventions so the
experience is uniform and tooling stays predictable.

## Numbering & difficulty tiers

Exercises are numbered `001`–`100` per language and grouped into four tiers:

| Tier folder        | Numbers   | Focus                                              |
|--------------------|-----------|----------------------------------------------------|
| `01-beginner`      | 001–035   | Syntax, types, control flow, collections, I/O      |
| `02-intermediate`  | 036–070   | Idioms, error handling, generics, std-lib depth    |
| `03-advanced`      | 071–090   | Concurrency, performance, memory, advanced patterns|
| `04-expert`        | 091–100   | Architecture, DSLs, metaprogramming, systems design|

The full per-language list lives in each folder's `catalog.md`.

## Anatomy of one exercise

Each exercise `NNN-slug` exists in two mirrored places:

- `exercises/<tier>/NNN-slug` — the **stub** you edit (contains `TODO`s and a
  failing test).
- `solutions/<tier>/NNN-slug` — the **reference** implementation.

Each stub carries a header comment/README stating: the goal, the concepts it
drills, and the acceptance criteria (what the test verifies).

## Test-driven workflow

1. Read the exercise header for the goal and constraints.
2. Run the test — watch it fail (red).
3. Implement until the test passes (green).
4. Compare against the reference solution and note idiomatic differences.

Run commands per language are in each folder's `README.md` and summarised in the
root `CLAUDE.md`.

## Adding new exercises

Keep the stub/solution pair in sync, register the exercise in the language's
`catalog.md`, and make sure the stub's test **fails** before the solution is
applied and **passes** after.

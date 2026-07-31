# Vue 3 Track

Vue 3 exercises (Composition API + TypeScript), tested with **Vitest** and
`@vue/test-utils` in a jsdom environment.

## Setup (once)

```powershell
cd vue
npm install
```

## Commands

| Action                | Command                                  |
|-----------------------|------------------------------------------|
| Run all tests         | `npm test`                               |
| Watch mode            | `npm run test:watch`                     |
| Run tests by name     | `npm run test:one -- "increments"`       |
| Type-check            | `npm run typecheck`                      |

## Layout

- `exercises/<tier>/exNNN_slug/` — a composable (`.ts`) or component (`.vue`) stub
  plus its `*.test.ts`.
- `solutions/<tier>/exNNN_slug/` — reference implementation.

Stubs throw so tests start red. Composable exercises drill reactivity
(`ref`/`computed`/`watch`); component exercises drill props, events, slots, and
rendering via `mount()`.

See [`catalog.md`](catalog.md) for the full 100-exercise roadmap.

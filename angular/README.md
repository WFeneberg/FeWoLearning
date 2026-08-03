# Angular Track

Angular exercises (standalone components + services), tested headlessly with
**Jest** via `jest-preset-angular`.

## Setup (once)

```powershell
cd angular
npm install
```

## Commands

| Action              | Command                          |
|---------------------|----------------------------------|
| Run all tests       | `npm test`                       |
| Watch mode          | `npm run test:watch`             |
| Run tests by name   | `npm run test:one -- "increments"` |

## Layout

- `exercises/<tier>/exNNN_slug/` — a service or standalone component stub plus its
  `*.spec.ts` (Angular `TestBed`).
- `solutions/<tier>/exNNN_slug/` — reference implementation.

Service exercises drill DI and pure logic; component exercises drill standalone
components, signals, inputs/outputs, and template binding via `ComponentFixture`.

See [`catalog.md`](catalog.md) — the 100-row progress ledger. Currently **2 / 100**; the ⬜ rows are the work queue.

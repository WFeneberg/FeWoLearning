# Python Track

Test-driven Python exercises graded across four difficulty tiers.

## Setup (once)

```powershell
cd python
python -m venv .venv
.\.venv\Scripts\Activate.ps1
python -m pip install -e ".[dev]"
```

## Commands

| Action                     | Command                                              |
|----------------------------|------------------------------------------------------|
| Run all tests              | `pytest`                                             |
| Run one tier               | `pytest exercises/01-beginner`                       |
| Run one exercise's test    | `pytest exercises/01-beginner/test_ex001_temperature.py` |
| Lint                       | `ruff check .`                                        |
| Type-check                 | `mypy exercises`                                      |

## Layout

- `exercises/<tier>/exNNN_slug.py` — stub you implement (raises `NotImplementedError`).
- `exercises/<tier>/test_exNNN_slug.py` — the test that must pass.
- `solutions/<tier>/exNNN_slug.py` — reference implementation.

Module names are prefixed `exNNN_` so they are importable (Python modules cannot
start with a digit). Tier folders are on the pytest `pythonpath`, so tests import
the stub directly: `from ex001_temperature import celsius_to_fahrenheit`.

See [`catalog.md`](catalog.md) — the 100-row progress ledger. Currently **76 / 100**; the ⬜ rows are the work queue.

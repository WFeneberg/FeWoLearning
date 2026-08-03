# Go Track

Test-driven Go exercises. Requires Go 1.22+ — see
[`../docs/requirements.md`](../docs/requirements.md) to install it.

## Commands

| Action                  | Command                                             |
|-------------------------|-----------------------------------------------------|
| Run all tests           | `go test ./...`                                      |
| Run one exercise        | `go test ./exercises/01-beginner/ex001_fizzbuzz/`   |
| Verbose                 | `go test -v ./...`                                   |
| Vet / static checks     | `go vet ./...`                                       |
| Format                  | `gofmt -w .`                                         |

## Layout

Each exercise is its own package under `exercises/<tier>/exNNN_slug/`:

- `slug.go`      — stub you implement (contains `panic("TODO: ...")` so it compiles).
- `slug_test.go` — the test that must pass.

Reference implementations live under `solutions/<tier>/exNNN_slug/`. Stubs panic
rather than returning zero values so an unfinished exercise fails loudly instead
of silently passing.

See [`catalog.md`](catalog.md) — the 100-row progress ledger. This track is **complete: 100 / 100**.

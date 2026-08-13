// Exercise 092 — a typed data access layer: DTO mapping and a typed error envelope (expert).
// Goal:   turn "whatever the network handed back" (untyped, possibly malformed, possibly absent)
//         into either a trustworthy domain object or a specific, typed reason it isn't one — never
//         a thrown exception the caller has to remember to catch.
// Drills: mapping an external DTO shape to an internal domain type at the boundary (never leaking the
//         wire format past the repository), and a discriminated-union `Result` as the return type
//         instead of `throw`, so every failure mode is enumerated and switch-exhaustive at compile time.
// Passes: when `npx jest exercises/04-expert/ex092_typed_data_access_layer` is green.
//
// `UserFetcher` stands in for "however this repository actually reaches the network" — an injected
// function rather than a concrete `HttpClient` dependency, so a test can hand it a fake that returns
// or throws exactly what a scenario needs without any `HttpTestingController` machinery (that's
// exercise 046's drill; this one is about what happens to the response once it arrives, not about
// making the request). A real app would inject one backed by `HttpClient.get(...)`; the repository's
// own logic below does not, and should not, care which.
//
// The wire format (`full_name`, a loose `status` string) is deliberately NOT the same shape as the
// domain `User` type (`displayName`, a boolean `isActive`). That mismatch is normal and expected at
// a real API boundary, and `getUser` is the one place it gets resolved — nothing above this
// repository should ever see a `full_name` or a raw `status` string. Every distinct way the raw
// response can fail to become a valid `User` gets its own `UserRepositoryError` variant rather than
// being flattened into one generic "something went wrong": a caller can `switch` on `error.type` and
// handle "the user doesn't exist" completely differently from "the network failed" or "the server
// sent us garbage" — TypeScript will even warn if a switch over `error.type` stops being exhaustive
// once a new variant is added later.
//
// `getUser` must never let `fetchUser` throw (or the promise it returns reject) propagate out of this
// method uncaught — that defeats the entire point of returning a typed `Result` in the first place.
// A caller that always gets a `Result` back can handle every case through the same shape; a caller
// that sometimes gets a `Result` and sometimes has to wrap the call in `try`/`catch` has to remember
// which is which, and will eventually forget.

export interface User {
  readonly id: string;
  readonly displayName: string;
  readonly isActive: boolean;
}

export type Result<T, E> =
  | { readonly kind: "ok"; readonly value: T }
  | { readonly kind: "error"; readonly error: E };

export type UserRepositoryError =
  | { readonly type: "not-found"; readonly id: string }
  | { readonly type: "invalid-response"; readonly reason: string }
  | { readonly type: "network"; readonly message: string };

/** The raw shape a real backend actually sends - never exposed past this file. */
interface RawUserDto {
  readonly id: string;
  readonly full_name: string;
  readonly status: string;
}

export type UserFetcher = (id: string) => Promise<unknown>;

export class UserRepository {
  constructor(private readonly fetchUser: UserFetcher) {}

  /**
   * TODO: implement getUser.
   *   - Call `this.fetchUser(id)`. If it throws or the returned promise rejects, catch it and return
   *     `{ kind: "error", error: { type: "network", message } }`, where `message` is
   *     `err.message` if `err instanceof Error`, otherwise `String(err)`.
   *   - If the resolved value is `null` or `undefined`, return
   *     `{ kind: "error", error: { type: "not-found", id } }`.
   *   - Otherwise validate it looks like a `RawUserDto`: an object with a string `id`, a string
   *     `full_name`, and a string `status`. If any of those are missing or the wrong type, return
   *     `{ kind: "error", error: { type: "invalid-response", reason: "malformed user payload" } }`.
   *   - `status` must be exactly `"active"` or `"inactive"`. Any other string is
   *     `{ kind: "error", error: { type: "invalid-response", reason: \`unrecognized status: ${status}\` } }`.
   *   - Otherwise, map it to the domain type and return
   *     `{ kind: "ok", value: { id, displayName: full_name, isActive: status === "active" } }`.
   */
  async getUser(id: string): Promise<Result<User, UserRepositoryError>> {
    throw new Error("TODO: implement getUser");
  }
}

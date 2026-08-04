import { Injectable } from "@angular/core";
import { Observable } from "rxjs";

// Exercise 045 — typed HttpClient.get (intermediate).
// Goal:   fetch data with a real return type instead of `any`.
// Drills: inject(HttpClient), get<T>, building HttpParams conditionally, transforming the
//         response with map, and where the type safety actually stops.
// Passes: when `npx jest exercises/02-intermediate/ex045_http_get_typed` is green.
//
// `http.get<User>(url)` gives an `Observable<User>` — but be clear about what that is worth. It
// is an *assertion*, not a check: nothing at runtime verifies the server sent a User, so a
// renamed field arrives as undefined and TypeScript is none the wiser. The generic buys you
// editor support and a single place to change the shape, not validation. Genuine validation is a
// parse step (exercise 092's territory).
//
// HttpParams is immutable: `params.set(...)` returns a *new* HttpParams and leaves the original
// alone, so `params.set("page", "1")` on its own does nothing. Reassign, or chain.
//
// Leaving an empty parameter out matters more than it looks. `?search=` is a different request
// from no search at all — different cache key, and plenty of back ends treat it as "match the
// empty string" rather than "no filter".
//
// The whole service is testable without a browser: HttpClient is injected, so a test provides a
// fake HTTP backend and asserts on the request that would have gone out (exercise 046).

export interface User {
  readonly id: number;
  readonly name: string;
  readonly email: string;
}

export interface UserQuery {
  readonly page?: number;
  readonly search?: string;
  readonly activeOnly?: boolean;
}

@Injectable({ providedIn: "root" })
export class UserApi {
  /** The base every URL is built from. */
  static readonly base = "/api/users";

  /** TODO: inject HttpClient. */

  /** GET /api/users/<id> — one user. */
  getUser(id: number): Observable<User> {
    throw new Error("TODO: implement getUser");
  }

  /**
   * GET /api/users — a list, with query parameters.
   *
   * Include `page` only when it is given, `search` only when it is a non-blank string (trimmed),
   * and `activeOnly` only when true. Everything absent means no parameters at all.
   */
  listUsers(query?: UserQuery): Observable<readonly User[]> {
    throw new Error("TODO: implement listUsers");
  }

  /** GET one user, then keep only the name — a transformation on the way out. */
  getUserName(id: number): Observable<string> {
    throw new Error("TODO: implement getUserName");
  }

  /**
   * GET the list, then keep only the email addresses, sorted.
   *
   * Proof that mapping happens in the service, so callers never see the wire shape.
   */
  listEmails(query?: UserQuery): Observable<readonly string[]> {
    throw new Error("TODO: implement listEmails");
  }
}

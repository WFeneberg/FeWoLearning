import { Injectable } from "@angular/core";
import { Observable } from "rxjs";

// Exercise 046 — HttpTestingController (intermediate).
// Goal:   assert on the requests a service *would* have sent, and control the responses.
// Drills: expectOne by URL / by predicate / by {method, url}, match() for several, expectNone
//         for "nothing should have gone out", verify(), and flush with a body or a status.
// Passes: when `npx jest exercises/02-intermediate/ex046_http_testing_controller` is green.
//
// The testing backend replaces HttpClient's transport, so nothing leaves the process. Every
// request is queued until a test picks it up and answers it. That makes two things testable that
// otherwise are not: exactly what was sent (method, URL, body, headers), and what happens on any
// response you care to invent.
//
// `verify()` in an afterEach is the part people leave out and should not. Without it, a request
// nobody expected simply goes unnoticed — including the "extra fetch on every keystroke" class of
// bug, which is invisible unless something fails on an unaccounted-for request.
//
// `expectNone` is its mirror: an assertion that a code path did *not* reach the network. That is
// the only way to test a cache, which is why this service has one.
//
// A cache also brings the invalidation question, and the tests below pin it down: a write must
// drop the cache, or the UI keeps showing what it fetched before the write.

export interface Todo {
  readonly id: number;
  readonly title: string;
  readonly done: boolean;
}

@Injectable({ providedIn: "root" })
export class TodoApi {
  static readonly base = "/api/todos";

  /** TODO: inject HttpClient. */

  /** Whether a list result is currently cached — the spec reads this directly. */
  get cached(): boolean {
    throw new Error("TODO: implement the cached getter");
  }

  /**
   * GET /api/todos, cached.
   *
   * The first subscribe fetches and remembers. Later ones must resolve from memory without
   * touching the network at all.
   */
  list(): Observable<readonly Todo[]> {
    throw new Error("TODO: implement list");
  }

  /** GET /api/todos, ignoring and replacing the cache. */
  refresh(): Observable<readonly Todo[]> {
    throw new Error("TODO: implement refresh");
  }

  /**
   * POST /api/todos with `{title, done: false}` as the body.
   *
   * Invalidates the cache — the list is stale the moment this succeeds. A blank title is a
   * RangeError, thrown before any request is made.
   */
  create(title: string): Observable<Todo> {
    throw new Error("TODO: implement create");
  }

  /** DELETE /api/todos/<id>. Invalidates the cache. */
  remove(id: number): Observable<void> {
    throw new Error("TODO: implement remove");
  }

  /** PATCH /api/todos/<id> with `{done}`. Invalidates the cache. */
  setDone(id: number, done: boolean): Observable<Todo> {
    throw new Error("TODO: implement setDone");
  }
}

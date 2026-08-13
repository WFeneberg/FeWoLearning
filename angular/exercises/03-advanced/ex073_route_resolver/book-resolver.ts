import { Injectable, inject } from "@angular/core";
import { ResolveFn, Router, UrlTree } from "@angular/router";
import { Observable, delay, of } from "rxjs";

// Exercise 073 — a functional route resolver (advanced).
// Goal:   have the router fetch a book before the route activates, so the component that renders
//         it never has to deal with a "loading" state at all.
// Drills: ResolveFn, inject() inside a resolver, returning an Observable that the router itself
//         subscribes to and waits on, and returning a UrlTree to redirect when resolution fails.
// Passes: when `npx jest exercises/03-advanced/ex073_route_resolver` is green.
//
// A resolver runs during navigation, before the target route's component is created. The router
// subscribes to whatever the resolver returns (a value, a Promise, or an Observable) and only
// finishes navigating once it emits — so the component that eventually mounts can read the data
// straight off `route.snapshot.data`, synchronously, on its very first change-detection pass. That
// is the whole point: it moves "what if the data isn't here yet" out of the component and into the
// route configuration, where it only has to be handled once.
//
// Like a guard, a resolver can also return a UrlTree instead of the data it was asked for — the
// router treats that exactly like a guard's redirect: the navigation in progress is abandoned and
// a new one to the UrlTree starts instead. That is the idiomatic way to handle "the id in the URL
// does not exist" without the component ever rendering with a missing book.

export interface Book {
  id: string;
  title: string;
  author: string;
}

@Injectable({ providedIn: "root" })
export class BookCatalog {
  private readonly books = new Map<string, Book>([
    ["1", { id: "1", title: "Clean Code", author: "Robert C. Martin" }],
    ["2", { id: "2", title: "Refactoring", author: "Martin Fowler" }],
  ]);

  /** A stand-in for a network call — still asynchronous, so the resolver has to wait on it. */
  findById(id: string): Observable<Book | undefined> {
    return of(this.books.get(id)).pipe(delay(0));
  }
}

/**
 * TODO: implement the resolver.
 *
 * Read the `id` route param, ask BookCatalog for that book, and return the resulting Observable.
 * If the book does not exist, resolve to a UrlTree redirecting to "/books" instead of the missing
 * book — never let `undefined` reach the route's data.
 */
export const bookResolver: ResolveFn<Book | UrlTree> = (_route, _state) => {
  throw new Error("TODO: implement bookResolver");
};

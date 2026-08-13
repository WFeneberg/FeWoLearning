import { Injectable, inject } from "@angular/core";
import { ResolveFn, Router, UrlTree } from "@angular/router";
import { Observable, delay, map, of } from "rxjs";

// Exercise 073 — a functional route resolver (reference solution).

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

  findById(id: string): Observable<Book | undefined> {
    return of(this.books.get(id)).pipe(delay(0));
  }
}

export const bookResolver: ResolveFn<Book | UrlTree> = (route) => {
  const catalog = inject(BookCatalog);
  const router = inject(Router);
  const id = route.paramMap.get("id") ?? "";

  // The router subscribes to this Observable itself and waits for it before activating the
  // route — no separate loading state needed in the component that eventually mounts.
  return catalog.findById(id).pipe(
    map((book) => book ?? router.createUrlTree(["/books"])),
  );
};

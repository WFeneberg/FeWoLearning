import { TestBed } from "@angular/core/testing";
import {
  ActivatedRouteSnapshot,
  RouterStateSnapshot,
  UrlTree,
  convertToParamMap,
  provideRouter,
} from "@angular/router";
import { Router } from "@angular/router";
import { Observable, firstValueFrom } from "rxjs";
import { Book, bookResolver } from "./book-resolver";

const routeWithParam = (id: string): ActivatedRouteSnapshot =>
  ({ paramMap: convertToParamMap({ id }) }) as ActivatedRouteSnapshot;
const state = {} as RouterStateSnapshot;

describe("bookResolver", () => {
  let router: Router;

  beforeEach(() => {
    TestBed.configureTestingModule({ providers: [provideRouter([])] });
    router = TestBed.inject(Router);
  });

  const runResolver = (id: string) =>
    TestBed.runInInjectionContext(
      () => bookResolver(routeWithParam(id), state) as Observable<Book | UrlTree>,
    );

  it("resolves the book matching the route's id param", async () => {
    const result = await firstValueFrom(runResolver("1"));

    expect(result).toEqual({ id: "1", title: "Clean Code", author: "Robert C. Martin" });
  });

  it("resolves a different book for a different id", async () => {
    const result = (await firstValueFrom(runResolver("2"))) as Book;

    expect(result.title).toBe("Refactoring");
  });

  it("redirects to /books when the id does not match any book", async () => {
    const result = await firstValueFrom(runResolver("does-not-exist"));

    expect(result).toBeInstanceOf(UrlTree);
    expect(router.serializeUrl(result as UrlTree)).toBe("/books");
  });
});

import { ComponentFixture, TestBed } from "@angular/core/testing";
import { ActivatedRoute, ParamMap, convertToParamMap } from "@angular/router";
import { BehaviorSubject } from "rxjs";
import { ArticleLoader } from "./article-loader.service";
import { ArticleComponent } from "./article.component";

describe("ArticleComponent", () => {
  let fixture: ComponentFixture<ArticleComponent>;
  let component: ArticleComponent;
  let loader: ArticleLoader;
  let paramMap: BehaviorSubject<ParamMap>;

  const navigateTo = (id: string): void => {
    paramMap.next(convertToParamMap({ id }));
    fixture.detectChanges();
  };

  const text = (selector: string): string => {
    const found = fixture.nativeElement.querySelector(selector) as HTMLElement | null;
    if (found === null) {
      throw new Error(`no element matched "${selector}" — is the template implemented?`);
    }
    return found.textContent?.trim() ?? "";
  };

  beforeEach(async () => {
    paramMap = new BehaviorSubject<ParamMap>(convertToParamMap({ id: "1" }));
    await TestBed.configureTestingModule({
      imports: [ArticleComponent],
      providers: [
        {
          provide: ActivatedRoute,
          // Both halves of the same route: a live stream and a snapshot that never moves.
          useValue: {
            paramMap,
            snapshot: { paramMap: convertToParamMap({ id: "1" }) },
          },
        },
      ],
    }).compileComponents();
    loader = TestBed.inject(ArticleLoader);
    fixture = TestBed.createComponent(ArticleComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it("loads the first article", () => {
    component.start();

    expect(loader.requested).toEqual(["1"]);
    loader.respond("1", "First");

    expect(component.title()).toBe("First");
    expect(component.loadCount()).toBe(1);
  });

  it("renders the loaded title", () => {
    component.start();
    loader.respond("1", "First");
    fixture.detectChanges();

    expect(text("h2.title")).toBe("First");
    expect(text("p.loads")).toBe("1");
  });

  it("reloads when the route changes", () => {
    component.start();
    loader.respond("1", "First");

    navigateTo("2");

    expect(loader.requested).toEqual(["1", "2"]);
    loader.respond("2", "Second");

    // Same component instance throughout — no constructor, no ngOnInit, and still correct.
    expect(component.title()).toBe("Second");
    expect(component.loadCount()).toBe(2);
  });

  it("records every id it saw", () => {
    component.start();
    loader.respond("1", "First");
    navigateTo("2");
    loader.respond("2", "Second");
    navigateTo("3");
    loader.respond("3", "Third");

    expect(component.seenIds()).toEqual(["1", "2", "3"]);
  });

  it("abandons a load the user navigated away from", () => {
    component.start();
    navigateTo("2");

    // switchMap: the first load is cancelled rather than allowed to land last.
    expect(loader.cancelled).toEqual(["1"]);
    expect(loader.isPending("1")).toBe(true);

    loader.respond("2", "Second");
    expect(component.title()).toBe("Second");
    expect(component.loadCount()).toBe(1);
  });

  it("handles a route with no id", () => {
    component.start();
    loader.respond("1", "First");

    paramMap.next(convertToParamMap({}));
    fixture.detectChanges();

    expect(component.seenIds()).toEqual(["1", ""]);
    expect(loader.requested).toEqual(["1"]);
  });

  it("goes stale when the snapshot is read once", () => {
    component.startFromSnapshot();
    loader.respond("1", "First");
    expect(component.title()).toBe("First");

    navigateTo("2");

    // The bug: the snapshot still says "1", so nothing reloads and the page is wrong.
    expect(loader.requested).toEqual(["1"]);
    expect(component.title()).toBe("First");
    expect(component.loadCount()).toBe(1);
  });

  it("stops following once destroyed", () => {
    component.start();
    loader.respond("1", "First");

    fixture.destroy();
    paramMap.next(convertToParamMap({ id: "2" }));

    // takeUntilDestroyed: no request, and nothing writing to a dead component.
    expect(loader.requested).toEqual(["1"]);
    expect(component.title()).toBe("First");
  });
});

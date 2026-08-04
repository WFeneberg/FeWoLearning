import { ComponentFixture, TestBed } from "@angular/core/testing";
import { ClockComponent, Ticker } from "./clock.component";

describe("ClockComponent", () => {
  let fixture: ComponentFixture<ClockComponent>;
  let component: ClockComponent;
  let ticker: Ticker;

  const query = <T extends HTMLElement>(selector: string): T => {
    const found = fixture.nativeElement.querySelector(selector) as T | null;
    if (found === null) {
      throw new Error(`no element matched "${selector}" — is the template contract implemented?`);
    }
    return found;
  };

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [ClockComponent],
    }).compileComponents();
    ticker = TestBed.inject(Ticker);
    fixture = TestBed.createComponent(ClockComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it("subscribes once it is initialised", () => {
    expect(ticker.listenerCount()).toBe(1);
  });

  it("follows the ticker", () => {
    ticker.emit(7);

    expect(component.ticks()).toBe(7);
  });

  it("renders the current tick", () => {
    ticker.emit(7);
    fixture.detectChanges();

    expect(query("p.ticks").textContent).toContain("7");
  });

  it("keeps following while it is alive", () => {
    ticker.emit(1);
    ticker.emit(2);
    ticker.emit(3);

    expect(component.ticks()).toBe(3);
  });

  it("has not cleaned up anything yet", () => {
    expect(component.log).toEqual([]);
  });

  it("unsubscribes when destroyed", () => {
    expect(ticker.listenerCount()).toBe(1);

    fixture.destroy();

    // The leak test: a listener still registered here would keep firing forever.
    expect(ticker.listenerCount()).toBe(0);
  });

  it("stops following once destroyed", () => {
    ticker.emit(5);
    fixture.destroy();

    ticker.emit(99);

    // Worse than wasted memory: a live callback writing to a dead component.
    expect(component.ticks()).toBe(5);
  });

  it("runs both cleanups", () => {
    fixture.destroy();

    expect(component.log.slice().sort()).toEqual(["destroyRef", "ngOnDestroy"]);
  });

  it("cleans up only once", () => {
    fixture.destroy();
    fixture.destroy();

    expect(component.log.slice().sort()).toEqual(["destroyRef", "ngOnDestroy"]);
  });

  it("leaves the ticker usable for whoever is left", () => {
    const other: number[] = [];
    ticker.subscribe((tick) => other.push(tick));

    fixture.destroy();
    ticker.emit(42);

    expect(ticker.listenerCount()).toBe(1);
    expect(other).toEqual([42]);
  });

  it("does not leak across many instances", () => {
    for (let i = 0; i < 5; i += 1) {
      const extra = TestBed.createComponent(ClockComponent);
      extra.detectChanges();
      extra.destroy();
    }

    // Only the original fixture's listener is left.
    expect(ticker.listenerCount()).toBe(1);
  });
});

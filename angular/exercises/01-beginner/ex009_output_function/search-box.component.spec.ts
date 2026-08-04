import { Component } from "@angular/core";
import { ComponentFixture, TestBed } from "@angular/core/testing";
import { SearchBoxComponent } from "./search-box.component";

/** A parent listening by the outputs' public names. */
@Component({
  selector: "app-host",
  standalone: true,
  imports: [SearchBoxComponent],
  template: `
    <app-search-box
      (submitted)="searches.push($event)"
      (cleared)="clears = clears + 1"
      (changed)="transitions.push($event.from + '->' + $event.to)"
    />
  `,
})
class HostComponent {
  readonly searches: string[] = [];
  readonly transitions: string[] = [];
  clears = 0;
}

describe("SearchBoxComponent", () => {
  let fixture: ComponentFixture<SearchBoxComponent>;
  let component: SearchBoxComponent;
  let searches: string[];
  let transitions: string[];
  let clears: number;

  const query = <T extends Element>(selector: string): T => {
    const found = fixture.nativeElement.querySelector(selector) as T | null;
    if (found === null) {
      throw new Error(`no element matched "${selector}" — is the template contract implemented?`);
    }
    return found;
  };

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [SearchBoxComponent, HostComponent],
    }).compileComponents();
    fixture = TestBed.createComponent(SearchBoxComponent);
    component = fixture.componentInstance;
    searches = [];
    transitions = [];
    clears = 0;
    // subscribe() is all an OutputEmitterRef offers — no pipe(), no operators.
    component.submitted.subscribe((term) => searches.push(term));
    component.termChanged.subscribe(({ from, to }) => transitions.push(`${from}->${to}`));
    component.cleared.subscribe(() => (clears += 1));
    fixture.detectChanges();
  });

  it("records the typed term", () => {
    component.type("ang");
    fixture.detectChanges();

    expect(component.term()).toBe("ang");
    expect(query("p.term").textContent).toContain("ang");
  });

  it("announces the transition with both ends", () => {
    component.type("a");
    component.type("ab");

    expect(transitions).toEqual(["->a", "a->ab"]);
  });

  it("stays quiet when the term does not actually change", () => {
    component.type("ab");
    transitions.length = 0;

    component.type("ab");

    expect(transitions).toEqual([]);
  });

  it("emits the trimmed term on submit", () => {
    component.type("  angular  ");
    component.submit();

    expect(searches).toEqual(["angular"]);
  });

  it("refuses to search for nothing", () => {
    component.submit();
    component.type("   ");
    component.submit();

    expect(searches).toEqual([]);
  });

  it("emits a payload-free cleared event", () => {
    component.type("angular");
    component.clear();

    expect(component.term()).toBe("");
    expect(clears).toBe(1);
  });

  it("does not announce clearing an empty box", () => {
    component.clear();

    expect(clears).toBe(0);
  });

  it("submits from the button", () => {
    component.type("signals");
    query<HTMLButtonElement>("button.submit").click();

    expect(searches).toEqual(["signals"]);
  });

  it("clears from the button", () => {
    component.type("signals");
    query<HTMLButtonElement>("button.clear").click();

    expect(component.term()).toBe("");
    expect(clears).toBe(1);
  });

  it("is an event, not a signal", () => {
    component.type("x");
    component.submit();
    expect(searches).toEqual(["x"]);

    // Outputs carry notifications, not state — there is nothing to read back.
    expect(typeof (component.submitted as unknown as () => unknown)).not.toBe("function");
    expect(typeof component.submitted.emit).toBe("function");
    expect(typeof component.submitted.subscribe).toBe("function");
  });

  it("reaches a parent listening in a template", () => {
    const host = TestBed.createComponent(HostComponent);
    host.detectChanges();

    const box: SearchBoxComponent = host.debugElement.children[0].componentInstance;
    box.type("routing");
    box.submit();
    box.clear();
    host.detectChanges();

    expect(host.componentInstance.searches).toEqual(["routing"]);
    expect(host.componentInstance.clears).toBe(1);
    // The parent binds (changed), not (termChanged).
    expect(host.componentInstance.transitions).toEqual(["->routing", "routing->"]);
  });
});

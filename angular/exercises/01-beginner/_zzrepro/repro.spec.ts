import { Component } from "@angular/core";
import { TestBed } from "@angular/core/testing";

@Component({
  selector: "app-repro2",
  standalone: true,
  template: `<h2>{{ getLabel() }}</h2>`,
})
class ReproComponent2 {
  count = 0;
  getLabel() { this.count++; return "call#" + this.count; }
}

describe("repro2", () => {
  it("method call re-invoked each detectChanges", () => {
    TestBed.configureTestingModule({ imports: [ReproComponent2] });
    const fixture = TestBed.createComponent(ReproComponent2);
    fixture.detectChanges();
    console.log("after 1st detectChanges:", fixture.nativeElement.outerHTML);
    fixture.detectChanges();
    console.log("after 2nd detectChanges:", fixture.nativeElement.outerHTML);
    fixture.detectChanges();
    console.log("after 3rd detectChanges:", fixture.nativeElement.outerHTML);
  });
});

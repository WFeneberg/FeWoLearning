import { ComponentFixture, DeferBlockState, TestBed } from "@angular/core/testing";
import { DetailsPanelComponent } from "./details-panel.component";

describe("DetailsPanelComponent (@defer)", () => {
  let fixture: ComponentFixture<DetailsPanelComponent>;
  let panel: DetailsPanelComponent;

  beforeEach(() => {
    TestBed.configureTestingModule({ imports: [DetailsPanelComponent] });
    fixture = TestBed.createComponent(DetailsPanelComponent);
    fixture.detectChanges();
    panel = fixture.componentInstance;
  });

  it("reveal() flips the trigger signal the @defer block's `when` condition reads", () => {
    panel.reveal();

    expect(panel.shouldLoad()).toBe(true);
  });

  it("shows the placeholder block before anything has triggered", async () => {
    const [deferBlock] = await fixture.getDeferBlocks();
    await deferBlock.render(DeferBlockState.Placeholder);

    expect((fixture.nativeElement as HTMLElement).textContent).toContain("Details hidden");
  });

  it("shows the loading block while the deferred chunk is being fetched", async () => {
    const [deferBlock] = await fixture.getDeferBlocks();
    await deferBlock.render(DeferBlockState.Loading);

    expect((fixture.nativeElement as HTMLElement).textContent).toContain("Loading details");
  });

  it("shows the error block if loading the deferred chunk fails", async () => {
    const [deferBlock] = await fixture.getDeferBlocks();
    await deferBlock.render(DeferBlockState.Error);

    expect((fixture.nativeElement as HTMLElement).textContent).toContain("Couldn't load details");
  });

  it("renders the heavy panel with its input once the block completes", async () => {
    const [deferBlock] = await fixture.getDeferBlocks();
    await deferBlock.render(DeferBlockState.Complete);

    expect((fixture.nativeElement as HTMLElement).textContent).toContain("Heavy panel for Room 204");
  });

  it("clicking the reveal button calls reveal(), flipping the trigger", () => {
    const button = (fixture.nativeElement as HTMLElement).querySelector<HTMLButtonElement>(".reveal")!;

    button.click();
    fixture.detectChanges();

    expect(panel.shouldLoad()).toBe(true);
  });
});

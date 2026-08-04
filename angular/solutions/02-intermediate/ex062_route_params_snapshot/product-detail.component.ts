import { Component, inject } from "@angular/core";
import { ActivatedRoute } from "@angular/router";

// Exercise 062 — reading route parameters from the snapshot (reference solution).
@Component({
  selector: "app-product-detail",
  standalone: true,
  template: `
    <h2 class="title">{{ title() }}</h2>
    <p class="id">{{ rawId() }}</p>
    <p class="page">{{ page() }}</p>
    <p class="tags">{{ tags().join(",") }}</p>
  `,
})
export class ProductDetailComponent {
  private readonly route = inject(ActivatedRoute);

  productId(): number {
    const raw = this.rawId();
    if (raw === null) {
      throw new RangeError("no id in the route");
    }
    const parsed = Number(raw);
    // Checked rather than returned: Number("banana") is NaN, which would spread silently.
    if (!Number.isFinite(parsed)) {
      throw new RangeError(`id "${raw}" is not a number`);
    }
    return parsed;
  }

  rawId(): string | null {
    // null, not undefined, for an absent key — the ParamMap contract.
    return this.route.snapshot.paramMap.get("id");
  }

  page(): number {
    const raw = this.route.snapshot.queryParamMap.get("page");
    const parsed = Number(raw ?? "");
    return raw === null || !Number.isFinite(parsed) ? 1 : parsed;
  }

  tags(): readonly string[] {
    // getAll, because a query parameter can repeat and `get` would keep only the first.
    return this.route.snapshot.queryParamMap.getAll("tag");
  }

  hasQueryParam(name: string): boolean {
    return this.route.snapshot.queryParamMap.has(name);
  }

  title(): string {
    // The template renders unconditionally, so this has to absorb what productId() rejects.
    try {
      return `Product ${this.productId()}`;
    } catch {
      return "Unknown product";
    }
  }
}

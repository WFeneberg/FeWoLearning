import { TestBed } from "@angular/core/testing";
import { REPORT_EXPORTERS, ReportExporter, ReportExportService } from "./report-export.service";

const csvExporter: ReportExporter = {
  format: "csv",
  export: (data) => data.join(","),
};

const jsonExporter: ReportExporter = {
  format: "json",
  export: (data) => JSON.stringify(data),
};

const upperExporter: ReportExporter = {
  format: "upper",
  export: (data) => data.join(" ").toUpperCase(),
};

describe("ReportExportService (multi-provider plugin extension point)", () => {
  it("sees every registered exporter, not just the last one registered", () => {
    TestBed.configureTestingModule({
      providers: [
        { provide: REPORT_EXPORTERS, useValue: csvExporter, multi: true },
        { provide: REPORT_EXPORTERS, useValue: jsonExporter, multi: true },
        { provide: REPORT_EXPORTERS, useValue: upperExporter, multi: true },
      ],
    });

    const service = TestBed.inject(ReportExportService);

    expect(service.availableFormats()).toEqual(["csv", "json", "upper"]);
  });

  it("exports using the exporter whose format matches", () => {
    TestBed.configureTestingModule({
      providers: [
        { provide: REPORT_EXPORTERS, useValue: csvExporter, multi: true },
        { provide: REPORT_EXPORTERS, useValue: jsonExporter, multi: true },
      ],
    });

    const service = TestBed.inject(ReportExportService);

    expect(service.exportAs("csv", ["a", "b"])).toBe("a,b");
    expect(service.exportAs("json", ["a", "b"])).toBe(JSON.stringify(["a", "b"]));
  });

  it("throws a RangeError for a format nothing registered", () => {
    TestBed.configureTestingModule({
      providers: [{ provide: REPORT_EXPORTERS, useValue: csvExporter, multi: true }],
    });

    const service = TestBed.inject(ReportExportService);

    expect(() => service.exportAs("xml", ["a"])).toThrow(RangeError);
  });

  it("works with only a single plugin registered", () => {
    TestBed.configureTestingModule({
      providers: [{ provide: REPORT_EXPORTERS, useValue: upperExporter, multi: true }],
    });

    const service = TestBed.inject(ReportExportService);

    expect(service.availableFormats()).toEqual(["upper"]);
    expect(service.exportAs("upper", ["go", "team"])).toBe("GO TEAM");
  });

  it("does not throw when NO plugin is registered at all — an empty extension point is valid", () => {
    TestBed.configureTestingModule({ providers: [] });

    const service = TestBed.inject(ReportExportService);

    expect(service.availableFormats()).toEqual([]);
    expect(() => service.exportAs("csv", ["a"])).toThrow(RangeError);
  });
});

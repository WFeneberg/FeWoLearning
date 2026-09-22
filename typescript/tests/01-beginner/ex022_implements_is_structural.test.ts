import { describe, expect, it } from "vitest";
import {
  CollectingLogger,
  SilentLogger,
  logAll,
} from "@ex/01-beginner/ex022_implements_is_structural/index";

describe("ex022 CollectingLogger", () => {
  it("prefixes each message", () => {
    const logger = new CollectingLogger();
    logger.log("started");
    expect(logger.lines).toEqual(["collect: started"]);
  });
});

describe("ex022 logAll", () => {
  it("logs through a class that declares the interface", () => {
    const logger = new CollectingLogger();
    logAll(logger, ["one", "two"]);
    expect(logger.lines).toEqual(["collect: one", "collect: two"]);
  });

  it("logs through a class that never mentions the interface", () => {
    const logger = new SilentLogger();
    logAll(logger, ["one", "two"]);
    expect(logger.seen).toEqual(["one", "two"]);
  });

  it("does nothing for an empty message list", () => {
    const logger = new CollectingLogger();
    logAll(logger, []);
    expect(logger.lines).toEqual([]);
  });
});

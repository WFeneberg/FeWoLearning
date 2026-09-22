import { describe, expect, it } from "vitest";
import { Account } from "@ex/01-beginner/ex021_class_basics/index";

describe("ex021 Account", () => {
  it("keeps the id it was constructed with", () => {
    expect(new Account("a-1", 100).id).toBe("a-1");
  });

  it("starts at the opening balance", () => {
    expect(new Account("a-1", 100).getBalance()).toBe(100);
  });

  it("adds a deposit", () => {
    const account = new Account("a-1", 100);
    account.deposit(50);
    expect(account.getBalance()).toBe(150);
  });

  it("withdraws when there is enough", () => {
    const account = new Account("a-1", 100);
    expect(account.withdraw(40)).toBe(true);
    expect(account.getBalance()).toBe(60);
  });

  it("refuses a withdrawal that would overdraw, and changes nothing", () => {
    const account = new Account("a-1", 100);
    expect(account.withdraw(140)).toBe(false);
    expect(account.getBalance()).toBe(100);
  });

  it("allows withdrawing the whole balance", () => {
    const account = new Account("a-1", 100);
    expect(account.withdraw(100)).toBe(true);
    expect(account.getBalance()).toBe(0);
  });
});

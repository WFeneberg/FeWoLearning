// Reference solution — exercise 021.
export class Account {
  // Parameter properties: declared, accepted and assigned in one place.
  constructor(
    public readonly id: string,
    private balance: number,
  ) {}

  getBalance(): number {
    return this.balance;
  }

  deposit(amount: number): void {
    this.balance += amount;
  }

  withdraw(amount: number): boolean {
    if (amount > this.balance) {
      return false;
    }
    this.balance -= amount;
    return true;
  }
}

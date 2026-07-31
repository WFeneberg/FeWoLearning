// Package deadlockfreetransfer — Exercise 081 (reference solution).
package deadlockfreetransfer

import "sync"

// Account is a bank account guarded by its own mutex.
type Account struct {
	ID      int
	mu      sync.Mutex
	balance int
}

// NewAccount creates an account with the given id and starting balance.
func NewAccount(id, balance int) *Account {
	return &Account{ID: id, balance: balance}
}

// Balance returns the current balance, synchronized.
func (a *Account) Balance() int {
	a.mu.Lock()
	defer a.mu.Unlock()
	return a.balance
}

// Transfer moves amount from a to b, always locking the two accounts'
// mutexes in ascending order of ID. Because every caller — no matter
// which order it names the accounts in — acquires the locks in the same
// global order, two goroutines transferring in opposite directions
// between the same pair of accounts can never form a lock cycle.
func Transfer(a, b *Account, amount int) {
	if a == b {
		return
	}

	first, second := a, b
	if first.ID > second.ID {
		first, second = second, first
	}

	first.mu.Lock()
	defer first.mu.Unlock()
	second.mu.Lock()
	defer second.mu.Unlock()

	a.balance -= amount
	b.balance += amount
}

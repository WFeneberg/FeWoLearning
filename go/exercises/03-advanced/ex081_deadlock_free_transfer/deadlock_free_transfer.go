// Package deadlockfreetransfer — Exercise 081 (advanced).
// Goal:   Transfer funds between two accounts that each have their own
//         mutex, without ever deadlocking, by always acquiring the two
//         locks in a consistent global order.
// Drills: deadlock avoidance, lock ordering, sync.Mutex.
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
	panic("TODO: implement Balance")
}

// Transfer moves amount from a to b. It must lock both accounts' mutexes
// in a consistent global order (regardless of the order a and b are
// passed in) so that two concurrent transfers moving money in opposite
// directions between the same pair of accounts can never deadlock.
func Transfer(a, b *Account, amount int) {
	panic("TODO: implement Transfer")
}

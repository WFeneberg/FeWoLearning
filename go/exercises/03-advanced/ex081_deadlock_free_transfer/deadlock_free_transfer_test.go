package deadlockfreetransfer

import (
	"sync"
	"testing"
	"time"
)

// TestTransferMovesFunds checks a single transfer updates both balances
// correctly and exactly.
func TestTransferMovesFunds(t *testing.T) {
	a := NewAccount(1, 100)
	b := NewAccount(2, 50)

	Transfer(a, b, 30)

	if got := a.Balance(); got != 70 {
		t.Errorf("a.Balance() = %d, want 70", got)
	}
	if got := b.Balance(); got != 80 {
		t.Errorf("b.Balance() = %d, want 80", got)
	}
}

// TestTransferOrderIndependentLocking checks that Transfer locks the two
// accounts in the same global order no matter which order they are
// passed in, by running the same transfer with arguments swapped.
func TestTransferOrderIndependentLocking(t *testing.T) {
	a := NewAccount(5, 100)
	b := NewAccount(9, 100)

	Transfer(a, b, 10) // a (lower ID) -> b
	Transfer(b, a, 40) // b -> a, arguments swapped relative to IDs

	if got := a.Balance(); got != 130 {
		t.Errorf("a.Balance() = %d, want 130", got)
	}
	if got := b.Balance(); got != 70 {
		t.Errorf("b.Balance() = %d, want 70", got)
	}
}

// TestConcurrentOppositeTransfersDoNotDeadlock hammers a fixed pair of
// accounts with many concurrent transfers running in both directions
// simultaneously. If Transfer locked its two arguments in call order
// instead of a consistent global order, goroutine G1 doing
// Transfer(a, b, ...) while G2 does Transfer(b, a, ...) could each grab
// one lock and then block forever waiting for the other: classic
// deadlock. This test must complete quickly and preserve the total
// balance across the pair.
func TestConcurrentOppositeTransfersDoNotDeadlock(t *testing.T) {
	a := NewAccount(1, 1_000_000)
	b := NewAccount(2, 1_000_000)
	const total = 2_000_000
	const rounds = 2000

	done := make(chan struct{})
	go func() {
		var wg sync.WaitGroup
		for i := 0; i < rounds; i++ {
			wg.Add(2)
			go func(i int) {
				defer wg.Done()
				Transfer(a, b, (i%7)+1)
			}(i)
			go func(i int) {
				defer wg.Done()
				Transfer(b, a, (i%5)+1)
			}(i)
		}
		wg.Wait()
		close(done)
	}()

	select {
	case <-done:
	case <-time.After(5 * time.Second):
		t.Fatal("transfers did not complete within 5s: suspected deadlock")
	}

	if got := a.Balance() + b.Balance(); got != total {
		t.Errorf("total balance = %d, want %d (funds not conserved)", got, total)
	}
}

// TestConcurrentTransfersAmongManyAccountsConserveTotal runs many
// deterministic transfers among a ring of accounts, each transfer
// launched from its own goroutine, exercising the general N-account
// case (not just a single pair). It must finish promptly and the sum
// of all balances must be unchanged.
func TestConcurrentTransfersAmongManyAccountsConserveTotal(t *testing.T) {
	const numAccounts = 8
	const numTransfers = 4000
	const startBalance = 10_000

	accounts := make([]*Account, numAccounts)
	for i := range accounts {
		accounts[i] = NewAccount(i, startBalance)
	}

	done := make(chan struct{})
	go func() {
		var wg sync.WaitGroup
		wg.Add(numTransfers)
		for i := 0; i < numTransfers; i++ {
			from := i % numAccounts
			to := (i*7 + 3) % numAccounts
			if to == from {
				to = (to + 1) % numAccounts
			}
			amount := (i % 11) + 1
			go func(from, to, amount int) {
				defer wg.Done()
				Transfer(accounts[from], accounts[to], amount)
			}(from, to, amount)
		}
		wg.Wait()
		close(done)
	}()

	select {
	case <-done:
	case <-time.After(5 * time.Second):
		t.Fatal("transfers did not complete within 5s: suspected deadlock")
	}

	total := 0
	for _, acc := range accounts {
		total += acc.Balance()
	}
	want := numAccounts * startBalance
	if total != want {
		t.Errorf("total balance = %d, want %d (funds not conserved)", total, want)
	}
}

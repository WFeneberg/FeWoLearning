package pluginarchitectureinterfaces

import (
	"errors"
	"fmt"
	"sort"
	"strings"
	"sync"
	"testing"
)

// upperPlugin uppercases its input.
type upperPlugin struct{}

func (upperPlugin) Name() string { return "upper" }
func (upperPlugin) Execute(input string) (string, error) {
	return strings.ToUpper(input), nil
}

// reversePlugin reverses its input.
type reversePlugin struct{}

func (reversePlugin) Name() string { return "reverse" }
func (reversePlugin) Execute(input string) (string, error) {
	runes := []rune(input)
	for i, j := 0, len(runes)-1; i < j; i, j = i+1, j-1 {
		runes[i], runes[j] = runes[j], runes[i]
	}
	return string(runes), nil
}

// failingPlugin always reports a domain-specific error.
type failingPlugin struct{}

func (failingPlugin) Name() string { return "boom" }
func (failingPlugin) Execute(input string) (string, error) {
	return "", fmt.Errorf("boom: cannot process %q", input)
}

func TestRunDispatchesToRegisteredPlugin(t *testing.T) {
	cases := []struct {
		plugin Plugin
		input  string
		want   string
	}{
		{upperPlugin{}, "hello", "HELLO"},
		{reversePlugin{}, "hello", "olleh"},
	}
	for _, tc := range cases {
		r := NewRegistry()
		if err := r.Register(tc.plugin); err != nil {
			t.Fatalf("Register(%s) unexpected error: %v", tc.plugin.Name(), err)
		}
		got, err := r.Run(tc.plugin.Name(), tc.input)
		if err != nil {
			t.Fatalf("Run(%s, %q) unexpected error: %v", tc.plugin.Name(), tc.input, err)
		}
		if got != tc.want {
			t.Errorf("Run(%s, %q) = %q, want %q", tc.plugin.Name(), tc.input, got, tc.want)
		}
	}
}

func TestRunUnknownNameReturnsError(t *testing.T) {
	r := NewRegistry()
	if err := r.Register(upperPlugin{}); err != nil {
		t.Fatalf("Register unexpected error: %v", err)
	}
	_, err := r.Run("does-not-exist", "hi")
	if err == nil {
		t.Fatal("Run(unregistered name) expected error, got nil")
	}
	if !errors.Is(err, ErrNotFound) {
		t.Errorf("Run(unregistered name) error = %v, want wrapping ErrNotFound", err)
	}
}

func TestRunPropagatesPluginError(t *testing.T) {
	r := NewRegistry()
	if err := r.Register(failingPlugin{}); err != nil {
		t.Fatalf("Register unexpected error: %v", err)
	}
	_, err := r.Run("boom", "payload")
	if err == nil {
		t.Fatal("Run(boom) expected error from plugin, got nil")
	}
	if !strings.Contains(err.Error(), `cannot process "payload"`) {
		t.Errorf("Run(boom) error = %v, want it to contain the plugin's message", err)
	}
}

func TestRegisterRejectsDuplicateName(t *testing.T) {
	r := NewRegistry()
	if err := r.Register(upperPlugin{}); err != nil {
		t.Fatalf("first Register unexpected error: %v", err)
	}
	err := r.Register(upperPlugin{})
	if err == nil {
		t.Fatal("second Register(upper) expected error, got nil")
	}
	if !errors.Is(err, ErrAlreadyRegistered) {
		t.Errorf("second Register(upper) error = %v, want wrapping ErrAlreadyRegistered", err)
	}
}

func TestRegisterRejectsEmptyName(t *testing.T) {
	r := NewRegistry()
	err := r.Register(namedPlugin{name: ""})
	if err == nil {
		t.Fatal("Register(empty name) expected error, got nil")
	}
}

func TestNamesReturnsSortedRegisteredPlugins(t *testing.T) {
	r := NewRegistry()
	for _, p := range []Plugin{reversePlugin{}, upperPlugin{}, failingPlugin{}} {
		if err := r.Register(p); err != nil {
			t.Fatalf("Register(%s) unexpected error: %v", p.Name(), err)
		}
	}
	got := r.Names()
	want := []string{"boom", "reverse", "upper"}
	if !sort.StringsAreSorted(got) || fmt.Sprint(got) != fmt.Sprint(want) {
		t.Errorf("Names() = %v, want %v", got, want)
	}
}

func TestRegistryIsSafeForConcurrentRegisterAndRun(t *testing.T) {
	r := NewRegistry()
	const n = 20
	var wg sync.WaitGroup
	wg.Add(n)
	for i := 0; i < n; i++ {
		go func(i int) {
			defer wg.Done()
			_ = r.Register(namedPlugin{name: fmt.Sprintf("p%02d", i)})
		}(i)
	}
	wg.Wait()

	if got := len(r.Names()); got != n {
		t.Fatalf("Names() length = %d, want %d", got, n)
	}

	wg.Add(n)
	for i := 0; i < n; i++ {
		go func(i int) {
			defer wg.Done()
			name := fmt.Sprintf("p%02d", i)
			got, err := r.Run(name, "x")
			if err != nil {
				t.Errorf("Run(%s) unexpected error: %v", name, err)
				return
			}
			want := name + ":x"
			if got != want {
				t.Errorf("Run(%s) = %q, want %q", name, got, want)
			}
		}(i)
	}
	wg.Wait()
}

// namedPlugin is a minimal Plugin whose Execute echoes "name:input", used to
// probe registry edge cases (empty names, bulk concurrent registration).
type namedPlugin struct{ name string }

func (p namedPlugin) Name() string { return p.name }
func (p namedPlugin) Execute(input string) (string, error) {
	return p.name + ":" + input, nil
}

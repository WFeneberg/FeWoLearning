// Package pluginarchitectureinterfaces — Exercise 099 (reference solution).
package pluginarchitectureinterfaces

import (
	"errors"
	"fmt"
	"sort"
	"sync"
)

// ErrNotFound is returned by Run when no plugin is registered under the
// requested name.
var ErrNotFound = errors.New("plugin: not found")

// ErrAlreadyRegistered is returned by Register when a plugin with the same
// name has already been registered.
var ErrAlreadyRegistered = errors.New("plugin: already registered")

// Plugin is the common interface every plugin implementation must satisfy.
type Plugin interface {
	// Name returns the unique identifier the plugin is registered under.
	Name() string
	// Execute runs the plugin's behavior against input and returns its result.
	Execute(input string) (string, error)
}

// Registry holds named Plugin implementations and dispatches calls to them.
type Registry struct {
	mu      sync.RWMutex
	plugins map[string]Plugin
}

// NewRegistry creates an empty, ready-to-use Registry.
func NewRegistry() *Registry {
	return &Registry{plugins: make(map[string]Plugin)}
}

// Register adds p to the registry under p.Name(). It returns
// ErrAlreadyRegistered if a plugin with that name is already registered, or
// an error if p.Name() is empty.
func (r *Registry) Register(p Plugin) error {
	name := p.Name()
	if name == "" {
		return errors.New("plugin: name must not be empty")
	}

	r.mu.Lock()
	defer r.mu.Unlock()

	if _, exists := r.plugins[name]; exists {
		return fmt.Errorf("%w: %q", ErrAlreadyRegistered, name)
	}
	r.plugins[name] = p
	return nil
}

// Run looks up the plugin registered under name and invokes its Execute
// method with input, returning its result. It returns an error wrapping
// ErrNotFound if no plugin is registered under name.
func (r *Registry) Run(name string, input string) (string, error) {
	r.mu.RLock()
	p, ok := r.plugins[name]
	r.mu.RUnlock()

	if !ok {
		return "", fmt.Errorf("%w: %q", ErrNotFound, name)
	}
	return p.Execute(input)
}

// Names returns the sorted names of all currently registered plugins.
func (r *Registry) Names() []string {
	r.mu.RLock()
	defer r.mu.RUnlock()

	names := make([]string, 0, len(r.plugins))
	for name := range r.plugins {
		names = append(names, name)
	}
	sort.Strings(names)
	return names
}

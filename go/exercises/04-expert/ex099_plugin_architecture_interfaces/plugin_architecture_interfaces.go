// Package pluginarchitectureinterfaces — Exercise 099 (expert).
// Goal:   A plugin registry that registers named Plugin implementations of a
//         common interface and dispatches calls to them by name.
// Drills: interfaces as extension points, dynamic dispatch, error wrapping,
//         concurrency-safe registration/lookup.
package pluginarchitectureinterfaces

import "errors"

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
	// TODO: add fields (e.g. a mutex-protected map of name -> Plugin)
}

// NewRegistry creates an empty, ready-to-use Registry.
func NewRegistry() *Registry {
	panic("TODO: implement NewRegistry")
}

// Register adds p to the registry under p.Name(). It returns
// ErrAlreadyRegistered if a plugin with that name is already registered, or
// an error if p.Name() is empty.
func (r *Registry) Register(p Plugin) error {
	panic("TODO: implement Register")
}

// Run looks up the plugin registered under name and invokes its Execute
// method with input, returning its result. It returns an error wrapping
// ErrNotFound if no plugin is registered under name.
func (r *Registry) Run(name string, input string) (string, error) {
	panic("TODO: implement Run")
}

// Names returns the sorted names of all currently registered plugins.
func (r *Registry) Names() []string {
	panic("TODO: implement Names")
}

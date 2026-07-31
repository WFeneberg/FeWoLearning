<!-- Exercise 083 — ErrorBoundaryCapture component (advanced).
     Goal:   use `onErrorCaptured` to catch a render/lifecycle error thrown by
             a descendant in the default slot, stop it from propagating
             further up the component tree, and show a fallback message
             instead of letting the whole app crash.
     Drills: onErrorCaptured, returning false to stop propagation, error
             state, conditional rendering of slot vs. fallback UI. -->
<script setup lang="ts">
withDefaults(defineProps<{ fallbackMessage?: string }>(), {
  fallbackMessage: "Something went wrong.",
});

throw new Error("TODO: implement ErrorBoundaryCapture");
</script>

<template>
  <div>TODO: render the fallback message when an error was captured, else render the default slot</div>
</template>

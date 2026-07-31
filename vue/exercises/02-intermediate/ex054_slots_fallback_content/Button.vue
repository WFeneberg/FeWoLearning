<!--
  Exercise 054 — Button component with slot fallback content (intermediate).
  Goal:   render a default slot, falling back to "Click me" when no
          slot content is provided by the parent.
  Drills: <slot> fallback content, rendering a native <button>.
-->
<script setup lang="ts">
// No props are required for this exercise; the component only needs to
// render its default slot (with fallback content) inside a <button>.
</script>

<template>
  <button type="button">
    <!-- TODO: implement fallback content for the default slot. -->
    <span>{{ (() => { throw new Error("TODO: implement Button slot fallback") })() }}</span>
  </button>
</template>

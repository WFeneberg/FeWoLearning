<!--
  Exercise 054 — Button component with slot fallback content (reference solution).
-->
<script setup lang="ts">
// No props are required for this exercise; the component only needs to
// render its default slot (with fallback content) inside a <button>.
</script>

<template>
  <button type="button">
    <slot>Click me</slot>
  </button>
</template>

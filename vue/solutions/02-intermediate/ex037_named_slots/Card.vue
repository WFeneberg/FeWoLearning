<!--
Exercise 037 — Card component (reference solution).
Named `header` and `footer` slots wrap the default slot content.
-->
<template>
  <div class="card">
    <header v-if="$slots.header" class="card-header">
      <slot name="header" />
    </header>
    <div class="card-body">
      <slot />
    </div>
    <footer v-if="$slots.footer" class="card-footer">
      <slot name="footer" />
    </footer>
  </div>
</template>

<script setup lang="ts">
// No script logic needed — this component is purely slot-driven composition.
</script>

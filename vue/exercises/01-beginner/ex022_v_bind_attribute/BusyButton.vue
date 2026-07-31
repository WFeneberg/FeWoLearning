<!--
Exercise 022 — BusyButton component (beginner).
Goal:   bind the `disabled` attribute of a <button> to an `isBusy` prop using v-bind.
Drills: v-bind dynamic attribute binding, boolean attribute rendering.
-->
<script setup lang="ts">
defineProps<{
  isBusy: boolean;
}>();
</script>

<template>
  <button>TODO: bind disabled to isBusy</button>
</template>

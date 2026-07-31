<!--
Exercise 022 — BusyButton component (reference solution).
-->
<script setup lang="ts">
defineProps<{
  isBusy: boolean;
}>();
</script>

<template>
  <button v-bind:disabled="isBusy">Submit</button>
</template>

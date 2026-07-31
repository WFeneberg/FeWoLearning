<!-- Exercise 040 — LabeledInput component (reference solution). -->
<script setup lang="ts">
defineProps<{ label: string }>();

const model = defineModel<string>({ default: "" });
</script>

<template>
  <label>
    <span data-testid="label">{{ label }}</span>
    <input type="text" data-testid="input" v-model="model" />
  </label>
</template>

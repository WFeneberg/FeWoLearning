<!-- Exercise 057 — CapitalizeInput component (reference solution). -->
<script setup lang="ts">
defineProps<{ label: string }>();

const [model, modifiers] = defineModel<string, "capitalize">({ default: "" });

function capitalize(value: string): string {
  if (!modifiers.capitalize || value.length === 0) return value;
  return value.charAt(0).toUpperCase() + value.slice(1);
}
</script>

<template>
  <label>
    <span data-testid="label">{{ label }}</span>
    <input
      type="text"
      data-testid="input"
      :value="model"
      @input="model = capitalize(($event.target as HTMLInputElement).value)"
    />
  </label>
</template>

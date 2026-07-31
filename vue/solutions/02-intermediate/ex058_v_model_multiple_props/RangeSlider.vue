<!-- Exercise 058 — RangeSlider component (reference solution). -->
<script setup lang="ts">
const props = defineProps<{ min: number; max: number }>();
const emit = defineEmits<{ "update:min": [value: number]; "update:max": [value: number] }>();
</script>

<template>
  <div>
    <label>
      Min
      <input
        type="range"
        :value="props.min"
        @input="emit('update:min', Number(($event.target as HTMLInputElement).value))"
      />
    </label>
    <label>
      Max
      <input
        type="range"
        :value="props.max"
        @input="emit('update:max', Number(($event.target as HTMLInputElement).value))"
      />
    </label>
  </div>
</template>

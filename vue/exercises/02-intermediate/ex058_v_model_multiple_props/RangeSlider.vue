<!-- Exercise 058 — RangeSlider component (intermediate).
     Goal:   support two independent named v-models, `v-model:min` and
             `v-model:max`, each backed by its own prop/emit pair.
     Drills: multiple v-model bindings, defineProps/defineEmits with
             `update:propName` events, numeric <input type="range">. -->
<script setup lang="ts">
defineProps<{ min: number; max: number }>();
defineEmits<{ "update:min": [value: number]; "update:max": [value: number] }>();
</script>

<template>
  <div>TODO: render two range inputs bound to min/max and emit update:min/update:max</div>
</template>

<!--
Exercise 041 — Rating component (intermediate).
Goal:   support v-model on a custom component via a `modelValue` prop
        and an `update:modelValue` emit for a star-click rating widget.
Drills: props/emits contracts, custom v-model, computed derived from props.
-->
<script setup lang="ts">
import { computed } from "vue";

const props = defineProps<{
  modelValue: number;
  max?: number;
}>();

const emit = defineEmits<{
  (e: "update:modelValue", value: number): void;
}>();

const stars = computed(() => Array.from({ length: props.max ?? 5 }, (_, i) => i + 1));

function select(_star: number) {
  throw new Error("TODO: implement select (emit update:modelValue)");
}
</script>

<template>
  <div class="rating">
    <button
      v-for="star in stars"
      :key="star"
      type="button"
      :data-testid="`star-${star}`"
      :class="{ filled: star <= modelValue }"
      @click="select(star)"
    >
      ★
    </button>
  </div>
</template>

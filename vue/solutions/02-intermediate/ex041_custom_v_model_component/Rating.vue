<!--
Exercise 041 — Rating component (reference solution).
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

function select(star: number) {
  emit("update:modelValue", star);
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

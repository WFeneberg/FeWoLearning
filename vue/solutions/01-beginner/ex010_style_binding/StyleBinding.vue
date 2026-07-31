<!-- Exercise 010 — StyleBinding component (reference solution). -->
<script setup lang="ts">
import { computed } from "vue";

interface Props {
  color: string;
  fontSize: number;
}

const props = defineProps<Props>();

const textStyle = computed(() => ({
  color: props.color,
  fontSize: `${props.fontSize}px`,
}));
</script>

<template>
  <p data-testid="text" :style="textStyle">Styled text</p>
</template>

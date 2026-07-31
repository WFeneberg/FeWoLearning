<!-- Exercise 010 — StyleBinding component (beginner). -->
<!-- Goal:   bind `color` and `fontSize` props to inline style via :style. -->
<!-- Drills: dynamic inline style binding, computed style objects. -->
<script setup lang="ts">
import { computed } from "vue";

interface Props {
  color: string;
  fontSize: number;
}

const props = defineProps<Props>();

const textStyle = computed(() => {
  throw new Error("TODO: implement textStyle");
});
</script>

<template>
  <p data-testid="text" :style="textStyle">Styled text</p>
</template>

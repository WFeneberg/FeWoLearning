<!-- Exercise 081 — VirtualizedListBasic component (reference solution). -->
<script setup lang="ts">
import { computed, ref } from "vue";

const props = withDefaults(
  defineProps<{
    items: string[];
    itemHeight: number;
    viewportHeight: number;
    overscan?: number;
  }>(),
  { overscan: 0 },
);

const scrollTop = ref(0);

function onScroll(event: Event): void {
  scrollTop.value = (event.target as HTMLElement).scrollTop;
}

const startIndex = computed<number>(() => {
  const raw = Math.floor(scrollTop.value / props.itemHeight) - props.overscan;
  return Math.max(0, raw);
});

const endIndex = computed<number>(() => {
  const lastIndex = props.items.length - 1;
  const raw =
    Math.ceil((scrollTop.value + props.viewportHeight) / props.itemHeight) -
    1 +
    props.overscan;
  return Math.min(lastIndex, raw);
});

const visibleIndices = computed<number[]>(() => {
  const result: number[] = [];
  for (let i = startIndex.value; i <= endIndex.value; i += 1) {
    result.push(i);
  }
  return result;
});

const totalHeight = computed<number>(() => props.items.length * props.itemHeight);
</script>

<template>
  <div
    data-testid="viewport"
    :style="{ height: viewportHeight + 'px', overflow: 'auto', position: 'relative' }"
    @scroll="onScroll"
  >
    <div :style="{ height: totalHeight + 'px', position: 'relative' }">
      <div
        v-for="index in visibleIndices"
        :key="index"
        :data-index="index"
        :style="{
          position: 'absolute',
          top: index * itemHeight + 'px',
          height: itemHeight + 'px',
        }"
      >
        {{ items[index] }}
      </div>
    </div>
  </div>
</template>

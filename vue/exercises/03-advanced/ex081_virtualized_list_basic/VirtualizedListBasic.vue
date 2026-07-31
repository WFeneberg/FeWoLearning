<!-- Exercise 081 — VirtualizedListBasic component (advanced).
     Goal:   render only the slice of `items` whose index falls inside the
             visible scroll window, derived from `scrollTop` and `itemHeight`,
             instead of rendering the whole (potentially huge) list.
     Drills: computed index-window math, scroll event handling, absolute
             positioning against a full-height spacer, overscan padding. -->
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

// TODO: replace with real index-window computations.
const startIndex = computed<number>(() => {
  throw new Error("TODO: implement startIndex");
});
const endIndex = computed<number>(() => {
  throw new Error("TODO: implement endIndex");
});
const visibleIndices = computed<number[]>(() => {
  throw new Error("TODO: implement visibleIndices");
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

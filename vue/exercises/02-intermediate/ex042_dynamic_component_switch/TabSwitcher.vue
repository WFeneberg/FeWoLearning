<!--
  Exercise 042 — TabSwitcher component (intermediate).
  Goal:   render one of three named tab components with `<component :is="...">`.
  Drills: dynamic components, resolving a component by name, exposing reactive
          state from a component instance for testing.
-->
<script setup lang="ts">
import { ref, computed } from "vue";

const AlphaPanel = {
  name: "AlphaPanel",
  template: `<section>Alpha content</section>`,
};

const BetaPanel = {
  name: "BetaPanel",
  template: `<section>Beta content</section>`,
};

const GammaPanel = {
  name: "GammaPanel",
  template: `<section>Gamma content</section>`,
};

export type TabName = "alpha" | "beta" | "gamma";

const tabs: Record<TabName, unknown> = {
  alpha: AlphaPanel,
  beta: BetaPanel,
  gamma: GammaPanel,
};

const currentTab = ref<TabName>("alpha");

const currentComponent = computed(() => {
  throw new Error("TODO: resolve tabs[currentTab.value]");
});

defineExpose({ currentTab });
</script>

<template>
  <div class="tab-switcher">
    <component :is="currentComponent" />
  </div>
</template>

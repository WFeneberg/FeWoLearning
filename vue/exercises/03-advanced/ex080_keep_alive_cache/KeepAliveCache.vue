<!-- Exercise 080 — KeepAliveCache component (advanced).
     Goal:   a tab view that switches between a "Notes" tab (holding a text
             input) and an "Info" tab via <component :is>, wrapped in
             <KeepAlive> so the Notes tab's typed text survives switching
             away to Info and back — instead of being reset because the
             component instance was destroyed and recreated.
     Drills: <KeepAlive>, dynamic components (<component :is>), component
             instance lifecycle vs. local reactive state. -->
<script setup lang="ts">
import { defineComponent, shallowRef } from "vue";

// Local state lives inside each tab component, not in the parent — that is
// exactly what makes this a meaningful test of <KeepAlive>: only caching the
// component instance (not lifting state up) preserves it across switches.
const NotesTab = defineComponent({
  name: "NotesTab",
  setup() {
    const text = shallowRef("");
    return { text };
  },
  template: `<textarea data-testid="notes-input" v-model="text" />`,
});

const InfoTab = defineComponent({
  name: "InfoTab",
  template: `<p data-testid="info-panel">Info panel — nothing to persist here.</p>`,
});

const tabs = {
  notes: NotesTab,
  info: InfoTab,
} as const;

type TabKey = keyof typeof tabs;

const activeTab = shallowRef<TabKey>("notes");

function selectTab(_key: TabKey): void {
  throw new Error("TODO: implement selectTab");
}
</script>

<template>
  <div>
    <button type="button" data-testid="tab-btn-notes" @click="selectTab('notes')">
      Notes
    </button>
    <button type="button" data-testid="tab-btn-info" @click="selectTab('info')">
      Info
    </button>

    <!-- TODO: wrap the dynamic component in <KeepAlive> so switching tabs
         does not destroy (and thus reset) the previously active tab's
         component instance and its local state. -->
    <component :is="tabs[activeTab]" />
  </div>
</template>

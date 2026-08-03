// Exercise 100 — Accessibility-audited tabs widget (expert).
// Goal:   a tabs widget that follows the WAI-ARIA Tabs pattern, not just one that
//         looks right: correct roles, the aria-* wiring that links a tab to its
//         panel, a roving tabindex so Tab enters the tablist once, and arrow-key
//         navigation that wraps.
// Drills: ARIA roles and relationships, roving tabindex, keyboard event handling,
//         programmatic focus management, rendering only the active panel.
import { defineComponent } from "vue";

export interface TabDefinition {
  id: string;
  label: string;
  content: string;
  disabled?: boolean;
}

/**
 * Renders:
 *
 *   <div>
 *     <div role="tablist" aria-label="…">
 *       <button role="tab" id="tab-<id>" aria-controls="panel-<id>"
 *               aria-selected="true|false" tabindex="0|-1"
 *               [aria-disabled="true"]>label</button>
 *       …
 *     </div>
 *     <div role="tabpanel" id="panel-<id>" aria-labelledby="tab-<id>" tabindex="0">
 *       content
 *     </div>
 *   </div>
 *
 * Requirements:
 *  - only the **active** panel is rendered at all;
 *  - exactly one tab has `aria-selected="true"`, and it is the only one with
 *    `tabindex="0"` — every other tab gets `tabindex="-1"` (roving tabindex), so
 *    the whole tablist is a single tab stop;
 *  - clicking a tab activates it; clicking a disabled tab does nothing;
 *  - ArrowRight / ArrowLeft move to the next / previous **enabled** tab and wrap
 *    around; Home / End jump to the first / last enabled tab;
 *  - keyboard navigation also moves DOM focus to the newly active tab, and calls
 *    `preventDefault()` on keys it handles;
 *  - the initial active tab is the first enabled one, or `defaultTabId` when given
 *    and enabled;
 *  - emit `change` with the newly active tab id whenever it changes.
 */
export const Tabs = defineComponent({
  name: "Tabs",
  props: {
    tabs: { type: Array as () => TabDefinition[], required: true },
    /** Value for the tablist's aria-label. */
    label: { type: String, default: "Tabs" },
    defaultTabId: { type: String, default: undefined },
  },
  emits: {
    change: (id: string) => typeof id === "string",
  },
  setup(_props, _ctx) {
    throw new Error("TODO: implement the Tabs widget");
  },
});

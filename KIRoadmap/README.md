# KI-Roadmap für Softwareentwickler

Ein mehrstufiger Einarbeitungsplan — zugeschnitten auf einen **Senior .NET-Architekten**,
der KI (a) im eigenen Arbeitsalltag nutzen und (b) in eigene Software einbauen und
verantworten will.

> Stand: 2026-09-04 (Vorversion: 2026-08-03). Kurse und Repos ändern sich schnell; die
> verlinkten Anlaufstellen (`anthropic.com/learn`, `huggingface.co/learn`,
> `deeplearning.ai/courses`, `learn.microsoft.com/dotnet/ai`) sind stabil, einzelne
> Kurstitel darunter nicht. Alles hier Verlinkte ist **kostenlos** nutzbar (Zertifikate
> teils kostenpflichtig); API-Nutzung kostet Geld — dazu Stufe 6.

<details>
<summary><strong>Was sich seit der Fassung vom 2026-08-03 geändert hat</strong></summary>

Die Achsen-Struktur und die acht Stufen sind unverändert — die tragen. Faktisch nachgezogen:

1. **EU AI Act, Digital Omnibus** (Stufe 8): Die Hochrisiko-Pflichten sind **nicht** am
   2026-08-02 in Kraft getreten. Verschiebung auf 2027-12-02 (Anhang III) bzw. 2028-08-02
   (Anhang I). Die **Transparenzpflichten aus Art. 50 gelten seit 2026-08-02 unverändert**.
2. **Microsoft Agent Framework 1.0 ist GA** (April 2026), Semantic Kernel im
   Wartungsmodus (Stufe 3). Die alte Formulierung „weiterhin gepflegt" war zu freundlich.
3. **MCP-Spezifikation 2026-07-28** (Stufe 5): größte Revision seit Start, Protokoll jetzt
   **zustandslos**, mit Breaking Changes. MCP, AGENTS.md und Agent Skills stehen inzwischen
   unter der **Agentic AI Foundation** (Linux Foundation) — für dich als Architekt das
   eigentlich wichtige Signal.
4. **Agent Skills / `SKILL.md`** als neue Ebene neben MCP (Stufen 1 und 5) — das gab es in
   der Vorversion überhaupt nicht.
5. **Sampling-Parameter sind weg** (Stufe 2): `temperature`/`top_p` werden von den aktuellen
   Anthropic-Modellen mit 400 abgelehnt, `budget_tokens` ebenso. Steuerung läuft über
   `effort`. Die alte Praxisübung „dieselbe Frage 10× bei temperature=0" lief so ins Leere.
6. **OWASP LLM Top 10 2026** (Stufe 6): Excessive Agency von Platz 6 auf 3.
7. **Neu aufgenommen**: `Microsoft.Extensions.AI.Evaluation` (Evals in xUnit — dein
   Heimvorteil), Agent Memory als eigenes Thema, agentische Suche als RAG-Alternative,
   Small Language Models / Modell-Routing, Zahlen zur Produktionsreife von Agenten.

</details>

---

## 0. Das Zielbild — zwei Achsen, nicht eine

Die meisten „KI lernen"-Pläne werfen zwei völlig verschiedene Kompetenzen in einen Topf.
Trenne sie bewusst:

| Achse | Frage | Wer braucht das |
|-------|-------|-----------------|
| **A — KI benutzen** | Wie mache ich *mich* mit LLMs 2–5× schneller? | jeder Entwickler, sofort |
| **B — KI bauen** | Wie baue ich LLM-Funktionen in Produktivsoftware ein, die nicht peinlich sind? | du als Architekt |

Achse A ist in ~4 Wochen auf gutem Niveau. Achse B ist die eigentliche Reise.
**Was du nicht brauchst:** Modelle trainieren. Das ist ein anderer Beruf.
Fundamentwissen (Stufe 7) brauchst du, um Grenzen einzuschätzen — nicht, um GPUs zu mieten.

**Gesamtplan:** 8 Stufen, ca. **5 h/Woche über ~6 Monate**. Jede Stufe hat ein
Abnahmekriterium („Fertig, wenn…"). Nicht weitergehen, bevor das erfüllt ist —
LLM-Wissen ohne gebauten Code verdampft.

```
Stufe 1  KI im Alltag              ── 3 Wo ─┐
Stufe 2  Mentales Modell LLM       ── 2 Wo ─┤ Achse A
Stufe 3  Erste eigene Integration  ── 3 Wo ─┤
Stufe 4  RAG / eigene Daten        ── 4 Wo ─┤
Stufe 5  Agenten, Tools, MCP       ── 4 Wo ─┤ Achse B
Stufe 6  Produktionsreife          ── 4 Wo ─┤
Stufe 7  Fundament (optional-tief) ── 4 Wo ─┤
Stufe 8  Architekten-Ebene         ── 2 Wo ─┘
```

**Die eine Verschiebung, die 2026 alles andere überlagert:** Die Frontier-Modelle sind
konvergiert. Der Unterschied zwischen einem guten und einem schlechten KI-Feature liegt
seit ca. einem Jahr nicht mehr am Modell, sondern am **Harness** drumherum — was im
Kontext landet, welche Tools es gibt, wie gemessen wird. Das ist genau die Sorte Arbeit,
die ein Architekt gut kann. Deshalb liegen die Stufen 4–6 im Zentrum dieses Plans.

---

## Stufe 1 — KI als Werkzeug im eigenen Alltag (3 Wochen)

**Ziel:** LLMs sind ab jetzt Teil deines Werkzeugkastens, nicht Spielerei.
Du weißt, wann sie helfen und wann sie dich verlangsamen.

### Inhalte
- Chat-Interfaces vs. **agentische Coding-Tools** (Claude Code, GitHub Copilot,
  OpenAI Codex, Cursor, Gemini CLI, JetBrains AI) — der Unterschied ist gewaltig.
  Seit 2026 gilt: die Modelle darin sind weitgehend austauschbar, **das Harness
  entscheidet** über die Erfahrung. Vergleiche also Werkzeuge, nicht Modellnamen.
- Prompting als Handwerk: Kontext liefern, Rolle/Format vorgeben, Beispiele geben
  (Few-Shot), Aufgaben zerlegen, das Modell erst planen lassen.
- **Context Engineering**: Was das Modell sieht, entscheidet über die Qualität —
  mehr als jede Prompt-Formulierung. Bei Coding-Agents: Anweisungsdatei pflegen,
  gezieltes Öffnen von Dateien, kurze Sessions statt endloser Threads, Subagenten für
  lesende Recherche, damit der Hauptkontext sauber bleibt.
- **Die drei Dateiformate, die du kennen musst** — alle drei sind inzwischen offene
  Standards unter der Linux Foundation (siehe Stufe 5), kein Herstellerkram:

  | Datei | Zweck | Wer liest sie |
  |-------|-------|---------------|
  | **`AGENTS.md`** | Projektwissen für *jeden* Coding-Agent: Build, Test, Konventionen | Codex, Cursor, Copilot, Gemini CLI, Aider, Windsurf, Zed, … (60 000+ Repos) |
  | **`CLAUDE.md`** | dasselbe für Claude Code | Claude Code (liest `AGENTS.md` **nicht** automatisch — verweise mit `@AGENTS.md` in der ersten Zeile darauf) |
  | **`SKILL.md`** | eine *wiederverwendbare Fähigkeit*: Markdown + YAML-Frontmatter, optional mit Skripten | Claude, Codex, Copilot, VS Code, Cursor, Gemini CLI, Goose u. a. (~40 Produkte) |

  Die Unterscheidung lohnt sich: `AGENTS.md`/`CLAUDE.md` beschreiben **dieses Repo**,
  `SKILL.md` beschreibt **eine Arbeitsweise**, die du über Repos hinweg mitnimmst
  („so schreiben wir hier ADRs", „so ziehst du ein EF-Core-Migrationsskript"). Skills
  werden erst geladen, wenn sie gebraucht werden — das ist Context Engineering als
  Dateiformat.
- Wo LLMs zuverlässig sind (Boilerplate, Tests, Migrationen, Reviews, Doku,
  fremde Sprachen lernen — genau dein FeWoLearning-Fall) und wo nicht
  (exakte Zahlen, aktuelle API-Signaturen, alles ohne Verifikation).

### Kostenlose Quellen
- **[Anthropic Academy](https://www.anthropic.com/learn)** (Kurse auf
  [anthropic.skilljar.com](https://anthropic.skilljar.com)) — seit März 2026 stark
  ausgebaut: inzwischen gut 20 Kurse in mehreren Tracks, mit Zertifikat, kostenlos.
  Für dich: *Claude Code 101* / *Claude Code in Action*, *Claude with the API*, sowie
  die neueren Einheiten zu **MCP** und **Agent Skills**. (Für die praktischen Übungen
  in *Claude Code 101* brauchst du einen Pro/Max-Zugang oder einen API-Key.)
- **[anthropics/courses](https://github.com/anthropics/courses)** — Jupyter-Notebooks:
  API Fundamentals, **Prompt Engineering Tutorial**, Real World Prompting,
  Prompt Evaluations, Tool Use. Weiterhin der beste kostenlose Prompting-Kurs.
- **[agentskills.io](https://agentskills.io/)** — die offene Spezifikation für `SKILL.md`
  samt Beispielen; **[agents.md](https://agents.md/)** analog für `AGENTS.md`.
- **[Google „Prompt Engineering" Whitepaper](https://www.kaggle.com/whitepaper-prompt-engineering)**
  (Lee Boonstra) — 60 Seiten, dicht, kein Marketing.
- **[Claude Code Docs](https://docs.claude.com/en/docs/claude-code/overview)** /
  **[GitHub Copilot Docs](https://docs.github.com/en/copilot)**.

### Praxis
1. Führe **eine ganze Arbeitswoche** ausschließlich mit einem Coding-Agent im
   Beifahrersitz. Nicht Autocomplete — echte Aufgaben delegieren.
2. Schreib dir eine persönliche `prompts.md` mit 10 wiederkehrenden Mustern
   („Review dieses Diffs auf Nebenwirkungen", „Portiere diese Klasse nach Go", …).
   Wenn ein Muster zum dritten Mal auftaucht: mach ein `SKILL.md` daraus.
3. Lege in einem echten Repo eine `AGENTS.md` an (plus `CLAUDE.md` mit `@AGENTS.md`)
   und miss, ob die Ergebnisse besser werden. (Dein FeWoLearning-Repo hat schon eine
   `CLAUDE.md` — gutes Beispiel, und ein guter Kandidat für den Split.)

**Fertig, wenn:** du bei einer neuen Aufgabe *automatisch* abwägst „selbst tippen
oder delegieren?" und in mindestens zwei Fällen begründet **selbst** tippst.

---

## Stufe 2 — Das mentale Modell: Was ein LLM wirklich tut (2 Wochen)

**Ziel:** Du kannst erklären, warum ein Modell halluziniert, warum es rechnen nicht kann,
warum derselbe Prompt zweimal Verschiedenes liefert — ohne Mystik, ohne Mathe-Tiefe.

### Inhalte
- **Tokens** (und warum „Zähle die r in strawberry" scheitert), Tokenizer, Kosten pro Token.
- **Context Window**, „Lost in the middle", Context Rot bei langen Sessions.
  **1 Mio. Token Kontext sind 2026 bei allen großen Anbietern Standard** — das ändert
  die Frage von „passt es rein?" zu „was *soll* rein?". Mehr Kontext ist nicht besser;
  ab einer gewissen Füllung sinkt die Qualität messbar. Genau deshalb heißt die
  Disziplin heute Context Engineering und nicht Prompt Engineering.
- **Nichtdeterminismus** — und wie man ihn heute steuert. Achtung, hier ist die
  Vorversion dieses Dokuments überholt: bei den aktuellen Anthropic-Modellen
  (Opus 5, Sonnet 5, Fable 5.x) sind `temperature`, `top_p` und `top_k` **entfernt** —
  ein Request damit endet in einem 400. Gesteuert wird über `output_config.effort`
  (`low` … `max`). Das ist ein Qualitäts-/Kosten-Regler, kein Zufallsregler:
  Nichtdeterminismus bleibt, du kannst ihn nicht mehr per Parameter wegdrehen.
  Für Tests heißt das: Zusicherungen über *Eigenschaften* der Antwort, nie über
  exakte Strings (→ Stufe 6).
- **Reasoning ist der Normalfall geworden.** „Extended Thinking" mit fixem
  Token-Budget (`budget_tokens`) ist abgelöst durch *adaptives* Denken: das Modell
  entscheidet selbst, wie lange es nachdenkt, du gibst nur die Effort-Stufe vor.
  Die rohe Gedankenkette bekommst du nicht zu sehen, höchstens eine Zusammenfassung.
- **Embeddings & Vektorähnlichkeit** — die Grundlage von Stufe 4.
- Modellklassen und was sie 2026 kosten (Anthropic-Listenpreise, $ pro 1 Mio. Token,
  als Größenordnung — die konkreten Zahlen veralten, das Verhältnis nicht):

  | Klasse | Beispiel | Kontext | Input | Output | wofür |
  |--------|----------|---------|-------|--------|-------|
  | Frontier | Claude Opus 5 | 1 M | $5 | $25 | schwierige Aufgaben, Agenten |
  | Arbeitspferd | Claude Sonnet 5 | 1 M | $2 | $10 | der Default für Produktcode |
  | schnell/günstig | Claude Haiku 4.5 | 200 K | $1 | $5 | Klassifikation, Subagenten, Massenlast |

  Dazu: multimodal ist Standard, lokal vs. Cloud siehe Stufe 7. Der Faktor 5 zwischen
  den Zeilen ist der Grund, warum Modell-Routing (Stufe 6) sich lohnt.
- Grenzen: Wissensstichtag, Konfidenz ≠ Korrektheit, Prompt Injection (Vorschau auf Stufe 6).

### Kostenlose Quellen
- **[microsoft/generative-ai-for-beginners](https://github.com/microsoft/generative-ai-for-beginners)**
  — 21 Lektionen, Lektion 1–6 reichen hier. Beste strukturierte Gratis-Basis.
- **[Hugging Face LLM Course](https://huggingface.co/learn/llm-course)** — Kapitel 1–2
  (Transformers, Tokenizer) für das konzeptionelle Fundament; inzwischen 12 Kapitel
  bis hin zu Reasoning-Modellen.
- **[Andrej Karpathy: „Deep Dive into LLMs like ChatGPT"](https://www.youtube.com/watch?v=7xTGNNLPyMI)**
  (~3,5 h) und **[„Intro to LLMs"](https://www.youtube.com/watch?v=zjkBMFhNj_g)** (1 h) —
  wenn du nur *eine* Sache aus dieser Stufe machst, dann diese.
- **[Tiktokenizer](https://tiktokenizer.vercel.app/)** — Tokens live sehen. 10 Minuten, großer Aha-Effekt.
- **[The Illustrated Transformer](https://jalammar.github.io/illustrated-transformer/)** (Jay Alammar)
  — der Klassiker, falls du visuell lernst.

### Praxis
- Nimm einen 10-Seiten-Text, tokenisiere ihn, rechne die Kosten für drei Modelle aus.
  Nutze dafür den `count_tokens`-Endpunkt des Anbieters, nicht `tiktoken` — die
  Tokenizer unterscheiden sich zwischen Modellfamilien.
- Stelle dieselbe Frage 10× **bei identischem Request** und dokumentiere die Streuung.
  Dann dieselbe Frage je 5× bei `effort: low` und `effort: high` — und vergleiche
  Streuung, Latenz und Tokenkosten. Das ist die 2026-Variante des alten
  Temperature-Experiments und lehrt mehr.
- Erkläre einem Kollegen in 5 Minuten, was ein Embedding ist. Wenn du ins Stocken kommst → nochmal.

**Fertig, wenn:** du bei einem Fehlverhalten des Modells eine *Hypothese* hast
(Tokenisierung? Kontext zu voll? Effort zu niedrig? Wissensstichtag?) statt „KI halt".

---

## Stufe 3 — Erste eigene Integration: API, Structured Output, Tool Use (3 Wochen)

**Ziel:** Du hast LLM-Funktionalität aus **eigenem C#-Code** aufgerufen, mit typisierten
Ergebnissen und Werkzeugaufrufen. Ab hier ist es normale Softwareentwicklung.

### Inhalte
- Messages-API: system/user/assistant, Multi-Turn, Streaming, Stop-Reasons, Token-Limits.
- **Structured Output / JSON-Schema** — der wichtigste Hebel für produktiven Einsatz:
  LLM-Output wird zu einem `record`, nicht zu einem String, den du parsen musst.
  Wichtig: das ist heute ein **erzwungenes API-Feature** (Grammar-constrained decoding),
  kein Prompt-Trick mehr. Ebenso `strict: true` auf Tool-Definitionen — damit sind die
  Argumente eines Tool-Calls schemavalide, nicht nur meistens.
- **Tool Use / Function Calling** — das Modell fordert Funktionsaufrufe an, *dein* Code
  führt sie aus. Kern jeder späteren Agentenarchitektur. Auch das Umgekehrte kennen:
  serverseitige Tools (Websuche, Code-Ausführung), die der Anbieter selbst ausführt.
- **Prompt Caching** — 5–10× Kostenhebel bei wiederholtem System-Kontext. Die Regel, die
  in der Praxis alles entscheidet: Caching ist **Präfix-Matching**. Ein `DateTime.Now`
  im System-Prompt invalidiert alles dahinter. Verifiziere über
  `usage.cache_read_input_tokens`, statt zu glauben, es funktioniere.
- **Batch-API** (~50 % günstiger) für alles, was nicht interaktiv ist.
- Fehlerbehandlung: Rate Limits, Retries mit Backoff, Timeouts, Idempotenz.

### Der .NET-Stack (dein Heimvorteil)

Hier hat sich seit Anfang 2026 substanziell etwas geändert — die Konsolidierung ist durch:

| Baustein | Was es ist | Stand 2026-09 |
|----------|-----------|---------------|
| **[`Microsoft.Extensions.AI`](https://learn.microsoft.com/dotnet/ai/microsoft-extensions-ai)** | die Abstraktionsschicht (`IChatClient`, `IEmbeddingGenerator`) — DI-, Logging-, OTel-, Caching-fähig | stabil, 10.9.0 (Aug 2026), im .NET-10-Zyklus. **Hier anfangen.** |
| **[Microsoft Agent Framework](https://learn.microsoft.com/agent-framework/)** (`Microsoft.Agents.AI`) | Konvergenz aus Semantic Kernel + AutoGen; Agenten, Sessions, Workflows, Multi-Agent | **1.0 GA seit April 2026**; C# und Python, Go angekündigt |
| **[Semantic Kernel](https://github.com/microsoft/semantic-kernel)** | der Vorgänger | **Wartungsmodus**: kritische Bugfixes für mind. 1 Jahr nach dem Agent-Framework-GA, aber **keine neuen Features mehr**. Für Neues nicht mehr wählen; es gibt einen offiziellen [Migrationsleitfaden](https://learn.microsoft.com/agent-framework/migration-guide/from-semantic-kernel/) und eine Kompatibilitätsbrücke (`KernelFunction.as_agent_framework_tool`) |
| **[`Microsoft.Extensions.AI.Evaluation`](https://learn.microsoft.com/dotnet/ai/evaluation/libraries)** | Evals als xUnit/MSTest/NUnit-Tests | siehe Stufe 6 — der Grund, warum du Stufe 6 in .NET *nicht* überspringen musst |
| **[dotnet/ai-samples](https://github.com/dotnet/ai-samples)** | offizielle, lauffähige Beispiele | |

Denk in bekannten Mustern: `IChatClient` ist ein Interface wie `HttpClient` —
registriere es in DI, dekoriere es (Logging, Caching, Telemetrie), mocke es im Test.
Das ist bewusst so gebaut, damit du kein neues Paradigma lernen musst. Das Agent
Framework setzt genau darauf auf: ein `AIAgent` entsteht aus einem `IChatClient` per
`chatClient.AsAIAgent(...)`, Tools sind schlichte Methoden ohne Attribute.

### Kostenlose Quellen
- **[microsoft/Generative-AI-for-beginners-dotnet](https://github.com/microsoft/Generative-AI-for-beginners-dotnet)**
  — Lektionen mit kurzen Videos und lauffähigem .NET-Code. Exakt dein Einstiegspunkt.
- **[Microsoft Learn: AI for .NET developers](https://learn.microsoft.com/dotnet/ai/)** — Referenzdoku.
- **[anthropics/anthropic-cookbook](https://github.com/anthropics/anthropic-cookbook)**
  — Rezepte für Tool Use, Structured Output, Vision, Batch. Python, aber 1:1 übertragbar.
- **[openai/openai-cookbook](https://github.com/openai/openai-cookbook)** — dasselbe für OpenAI-APIs.
- **[Anthropic Docs: Tool Use](https://docs.claude.com/en/docs/agents-and-tools/tool-use/overview)**
  und **[Prompt Caching](https://docs.claude.com/en/docs/build-with-claude/prompt-caching)**.
- **[google-gemini/cookbook](https://github.com/google-gemini/cookbook)** — falls Gemini relevant wird.

### Praxisprojekt: „Commit-Message-Kritiker"
Konsolen-Tool in C#: liest `git diff --staged`, ruft ein LLM mit JSON-Schema auf,
liefert `record Review(Severity Level, string Summary, string[] Concerns)`.
Erweiterung: ein Tool `read_file(path)`, das das Modell selbst aufrufen darf,
um Kontext nachzuladen → dein erster Mini-Agent.
Dritte Ausbaustufe: denselben Kritiker einmal roh über `IChatClient` und einmal über
`Microsoft.Agents.AI` bauen — der Vergleich zeigt dir genau, was ein Framework abnimmt
und was es dir verbirgt.

**Fertig, wenn:** du eine typisierte C#-Struktur zurückbekommst, deren Felder das Modell
verlässlich füllt, und du einen Tool-Call-Loop selbst geschrieben hast (kein Framework).

---

## Stufe 4 — RAG: Das Modell an deine Daten anschließen (4 Wochen)

**Ziel:** Du kannst eine Frage-Antwort-Funktion über Dokumente bauen, *und* du weißt,
warum die meisten RAG-Prototypen in Produktion enttäuschen.

### Inhalte
- Die Pipeline: **Ingest → Chunking → Embedding → Vektorspeicher → Retrieval → Rerank → Generierung**.
- **Chunking ist der Hebel Nr. 1** (Größe, Overlap, Metadaten mitführen). 2026 ist
  fixes Chunking nach Zeichenzahl der Anfängerfehler; Standard ist **semantisches
  Chunking** (Grenzen dort, wo die Embeddings aufeinanderfolgender Sätze auseinanderlaufen)
  bzw. strukturbewusstes Chunking entlang von Überschriften und Code-Blöcken.
- **Hybrid Search**: BM25/Volltext + Vektor, zusammengeführt per **Reciprocal Rank Fusion**.
  Reine Vektorsuche ist fast immer schlechter.
- **Reranking** (Cross-Encoder) — weiterhin der größte Qualitätssprung pro Aufwand.
  Faustregel 2026: 50 Kandidaten mit hohem Recall holen, per Cross-Encoder auf die
  besten 5 eindampfen. *Nicht* alle 50 ins Kontextfenster kippen, nur weil 1 M Token
  hineinpassen.
- Vektorspeicher: pgvector (Postgres, meist die richtige Antwort), Qdrant, Azure AI Search,
  SQLite-vec für lokal. Fürs Verständnis: einmal ohne Datenbank, nur mit Cosine-Similarity im Speicher.
- **Agentische Suche als ernsthafte Alternative zum Index.** Der Ansatz, den Coding-Agents
  fahren: kein Embedding-Index, sondern Werkzeuge (`grep`, `glob`, `read_file`) und ein
  Modell, das iterativ selbst sucht, liest und nachfasst. Vorteile: kein Index-Staleness,
  keine Chunking-Frage, natürliche Quellenangabe. Nachteile: mehr Token, höhere Latenz,
  schlechter bei „unscharfen" semantischen Fragen. Für Code und strukturierte Repos oft
  überlegen, für 50 000 PDFs nicht. **Diese Entscheidung ist die eigentliche
  Architekturfrage in Stufe 4** — nicht die Wahl der Vektordatenbank.
- **Agentic RAG**: die Zwischenform — das Modell entscheidet *während* des Antwortens,
  ob und wonach es retrievt, formuliert Suchanfragen um, prüft die Treffer und sucht bei
  Bedarf erneut. Deutlich besser bei mehrstufigen Fragen, deutlich teurer.
- **Memory als eigene Disziplin.** 2026 ein eigenständiger Architekturbaustein, nicht
  „RAG über den Chatverlauf": Extraktion von Fakten aus Konversationen, Aktualisieren und
  Löschen (nicht nur Anhängen), Trennung von Kurzzeit-/Langzeitgedächtnis. Relevant,
  sobald dein Feature über eine Session hinaus etwas wissen soll. Anlaufstellen:
  [Mem0](https://github.com/mem0ai/mem0), [LangMem](https://github.com/langchain-ai/langmem),
  der Memory-Tool-Ansatz der Anbieter-APIs. Warnung: hier ist viel Marketing im Markt —
  miss es wie jedes andere Retrieval (unten).
- Zitate/Quellenangaben, „ich weiß es nicht"-Verhalten, Umgang mit widersprüchlichen Quellen.
- **Wann RAG nicht die Antwort ist:** großer Kontext + Prompt Caching, oder klassische
  Suche + Volltext ans Modell, oder schlicht eine SQL-Abfrage.

### Kostenlose Quellen
- **[microsoft/generative-ai-for-beginners](https://github.com/microsoft/generative-ai-for-beginners)**
  — Lektionen zu Embeddings, Suche und RAG.
- **[Building and Evaluating Advanced RAG](https://www.deeplearning.ai/courses/)**
  (DeepLearning.AI, ~90 min) — betont Evaluation, was fast alle Tutorials auslassen.
- **[Hugging Face LLM Course](https://huggingface.co/learn/llm-course)**, Kapitel zu
  Embeddings & semantischer Suche.
- **[Anthropic: Effective context engineering for AI agents](https://www.anthropic.com/engineering/effective-context-engineering-for-ai-agents)**
  — der Text, der die Verschiebung von RAG-Index zu agentischer Suche am klarsten begründet.
- **[pgvector](https://github.com/pgvector/pgvector)** + **[Pgvector.EntityFrameworkCore](https://github.com/pgvector/pgvector-dotnet)**
  — für dich vermutlich der pragmatischste Weg.
- **[Ragas](https://github.com/explodinggradients/ragas)** — RAG-Metriken (Faithfulness,
  Context Precision/Recall). Bereitet Stufe 6 vor. In .NET: die `Retrieval`-,
  `Groundedness`- und `Completeness`-Evaluatoren aus `Microsoft.Extensions.AI.Evaluation`.

### Praxisprojekt: „Frag dein Repo"
Indexiere die Markdown-Dateien deines FeWoLearning-Monorepos (`CLAUDE.md`, alle
`catalog.md`, `docs/`) und beantworte Fragen wie „Welche Rust-Übungen fehlen noch und
warum?" — **mit Dateiangabe als Beleg**. Danach: baue absichtlich eine Frage ein, die
die Daten nicht hergeben, und bring das System dazu, das zuzugeben.
**Und dann die 2026-Zugabe:** dieselben 20 Fragen gegen einen Agenten mit reinen
`grep`/`read_file`-Tools, ganz ohne Index. Vergleiche Recall, Kosten und Latenz. Das
Ergebnis wird dich überraschen — und es ist genau die Zahl, die du in einem
Architektur-Review brauchst.

**Fertig, wenn:** du eine Retrieval-Qualitätsmessung hast (z. B. 20 Frage/Quelle-Paare,
Recall@5) und **Zahlen** dafür, wie viel Hybrid Search, Reranking und der agentische
Ansatz jeweils bringen.

---

## Stufe 5 — Agenten, Tools und MCP (4 Wochen)

**Ziel:** Du verstehst Agenten als das, was sie sind — eine `while`-Schleife um ein Modell
mit Werkzeugzugriff — und kannst beurteilen, wann sich der Kontrollverlust lohnt.

### Inhalte
- **Der Agent-Loop:** Modell → Tool-Call → Ausführung → Ergebnis zurück → wiederholen,
  bis fertig. Mehr ist es im Kern nicht. Schreib ihn einmal selbst, bevor du ein
  Framework nimmst.
- Muster: Reflection, Planning, Tool Use, Multi-Agent, **Orchestrator-Worker**,
  Routing, Evaluator-Optimizer.
- **Workflow vs. Agent** — Anthropics „Building Effective Agents" ist hier weiterhin der
  beste Text der Branche: deterministische Workflows schlagen Agenten fast immer. Agenten
  nur, wenn die Schrittfolge *wirklich* unbekannt ist. Die Rechnung dazu, die man sich
  merken sollte: drei verkettete Agenten mit je 70 % Erfolgsquote ergeben 34 % — jede
  zusätzliche autonome Stufe multipliziert Fehler, statt sie zu addieren.
- **Vier Bauweisen, zwei Fragen.** Wer liefert das *Harness* (Loop + Kontextverwaltung),
  wer das *Deployment*? Selbstgeschriebener Loop: keins von beidem. SDK-Tool-Runner /
  Agent-SDKs: Harness ja, Deployment nein. Gehostete Agentendienste (Anthropic Managed
  Agents, Microsoft **Foundry Agent Service**): beides. Diese Matrix zu kennen erspart
  die meisten Framework-Grundsatzdiskussionen.
- **MCP (Model Context Protocol)** — der De-facto-Standard, um Tools/Daten an Modelle
  anzubinden (JSON-RPC, Tools/Resources/Prompts). Zwei Dinge sind seit der Vorversion
  dieses Dokuments neu und beide betreffen dich als Architekten:
  - **Governance:** MCP wurde im Dezember 2025 an die **Agentic AI Foundation** (Linux
    Foundation) übergeben, zusammen mit `AGENTS.md` (OpenAI) und goose (Block); Agent
    Skills und A2A gehören inzwischen ebenfalls dorthin. Platinum-Mitglieder sind u. a.
    AWS, Anthropic, Google, Microsoft, OpenAI, Bloomberg, Cloudflare. Das ist der
    Unterschied zwischen „Herstellerformat" und „Standard, auf den man eine
    Integrationsstrategie stellen kann".
  - **Spezifikation `2026-07-28`:** die größte Revision seit dem Start, **mit Breaking
    Changes**. Kernpunkt: das Protokoll ist jetzt **zustandslos** — die Sessions und der
    `Mcp-Session-Id`-Header sind aus dem Streamable-HTTP-Transport verschwunden. Ein
    Remote-MCP-Server läuft damit hinter einem gewöhnlichen Load Balancer, ohne Sticky
    Sessions. Dazu: ein Extensions-Framework (Tasks sind dorthin gewandert), MCP Apps,
    gehärtete Autorisierung; Dynamic Client Registration ist zugunsten von CIMD
    abgekündigt, Roots/Sampling/Logging sind deprecated (laufen noch ≥ 12 Monate).
    **Wenn du 2025er-MCP-Tutorials liest: sie beschreiben ein anderes Protokoll.**
- **Der Token-Kostenhebel bei vielen Tools.** Alle Tool-Definitionen vorab zu laden und
  jedes Zwischenergebnis durchs Kontextfenster zu schleifen, skaliert nicht. Die
  Gegenmittel, die 2026 Praxis sind: **Tool Search** (Tools erst bei Bedarf laden),
  **programmatic tool calling** (das Modell ruft dein Tool aus einer Code-Sandbox heraus
  auf und filtert die Daten *bevor* sie in den Kontext kommen), pro Subagent nur die
  Tools, die er wirklich braucht.
- **MCP vs. Agent Skills** — die Frage kommt garantiert im Review, also klär sie vorher:
  MCP ist **Konnektivität** (welche Systeme kann der Agent anfassen), `SKILL.md` ist
  **prozedurales Wissen** (wie geht man in diesem Haus vor). Sie konkurrieren nicht,
  sie stapeln sich.
- Kontrolle: Human-in-the-Loop, Approval Gates, Sandboxing, Kosten- und Schrittlimits,
  Abbruchbedingungen.

### Kostenlose Quellen
- **[Anthropic: Building Effective Agents](https://www.anthropic.com/engineering/building-effective-agents)**
  — 20 Minuten Lesezeit, Pflichtlektüre.
- **[Anthropic: Writing Effective Tools for Agents](https://www.anthropic.com/engineering/writing-tools-for-agents)**
  — Tool-Design ist API-Design; das trifft direkt deine Kernkompetenz.
- **[Anthropic: Code Execution with MCP](https://www.anthropic.com/engineering/code-execution-with-mcp)**
  — der dritte Teil der Trilogie: mehr Tools bei weniger Token. Lies alle drei in der
  Reihenfolge Context Engineering → Tools → Code Execution.
- **[modelcontextprotocol.io](https://modelcontextprotocol.io)** — Spezifikation; dazu der
  **[Changelog der Version 2026-07-28](https://modelcontextprotocol.io/specification/2026-07-28/changelog)**
  und der **[MCP-Blog](https://blog.modelcontextprotocol.io/)** für die Roadmap.
- **[MCP C# SDK](https://github.com/modelcontextprotocol/csharp-sdk)** (von Microsoft mitgepflegt) — dein SDK.
- **[Hugging Face MCP Course](https://huggingface.co/learn/mcp-course)** (mit Anthropic gebaut)
  und **[Hugging Face AI Agents Course](https://huggingface.co/learn/agents-course)**
  — kostenlos, zertifiziert; letzterer mit einer Bonus-Unit zu Observability und Evals.
- **[microsoft/ai-agents-for-beginners](https://github.com/microsoft/ai-agents-for-beginners)** — 18 Lektionen.
- **[Microsoft Agent Framework Docs](https://learn.microsoft.com/agent-framework/)** — für
  .NET-Produktion die Referenz; LangChain/LangGraph nur, wenn du im Python-Ökosystem
  mitreden musst.

### Praxisprojekt: eigener MCP-Server
Bau einen MCP-Server in C# für dein FeWoLearning-Repo mit den Tools
`list_exercises(track, tier)`, `run_tests(track, filter)`, `catalog_status()`.
Häng ihn an Claude Code und lass den Agenten den Katalogstand prüfen.
Achte darauf, gegen die Spec `2026-07-28` zu bauen — also zustandslos, ohne
Session-Annahmen im Server.
Danach: schreib den Agent-Loop einmal *ohne* Framework — 80 Zeilen, und du verstehst
jedes Framework danach in einer Stunde.
Zugabe: verpacke eine deiner wiederkehrenden Arbeitsweisen als `SKILL.md` und beobachte,
wie sich das vom MCP-Server unterscheidet.

**Fertig, wenn:** du in einem Architektur-Review begründen kannst, warum eine gegebene
Anforderung **kein** Agent sein sollte.

---

## Stufe 6 — Produktionsreife: Evals, Observability, Kosten, Sicherheit (4 Wochen)

**Ziel:** Das ist die Stufe, die aus einem beeindruckenden Demo ein System macht, das
du unterschreiben kannst. Als Architekt ist das **deine wichtigste Stufe** — und die,
die 90 % aller Tutorials überspringen.

Falls du eine Zahl brauchst, um diese Stufe im Unternehmen zu verteidigen: nach
Erhebungen aus 2026 schaffen es die wenigsten Agenten-Projekte in Produktion, und die
Rollback-Quote unterscheidet sich drastisch danach, ob automatisierte Evals existieren
(~9 % mit voller Eval-Abdeckung gegenüber ~47 % ohne). Gleichzeitig haben rund 89 % der
Teams Tracing, aber nur etwa die Hälfte Evals. Genau in dieser Lücke liegt der Hebel.
Die häufigsten Ursachen sind auch nicht Modellqualität, sondern unklare Erfolgskriterien,
fehlender Tool-/Datenzugriff und Drift in der Eval-Abdeckung.

### Inhalte

**Evaluation (Testing für Nichtdeterminismus)**
- Eval-Datensätze aufbauen: 30–50 reale Fälle schlagen jede synthetische Suite.
- Metriken: exakte Übereinstimmung, Schema-Validität, **LLM-as-Judge** (und dessen Bias),
  paarweise Vergleiche, Regressionstests bei Modell- oder Promptwechsel.
- **Agenten brauchen andere Metriken als Chatbots**: Hat der Agent die Absicht richtig
  erkannt? Ist er bei der Aufgabe geblieben? Waren die Tool-Aufrufe korrekt (richtiges
  Tool, richtige Argumente)? Das sind eigene Messgrößen, keine Textähnlichkeit.
- In CI verankern: Prompts sind Code. Prompt-Änderung ohne Eval-Lauf = ungetesteter Deploy.
- **In .NET ist das kein Fremdkörper mehr:**
  [`Microsoft.Extensions.AI.Evaluation`](https://learn.microsoft.com/dotnet/ai/evaluation/libraries)
  bringt Evaluatoren für Relevance, Truth, Completeness, Fluency, Coherence, Retrieval,
  Equivalence und Groundedness — plus explizite Agenten-Evaluatoren
  (`IntentResolutionEvaluator`, `TaskAdherenceEvaluator`, `ToolCallAccuracyEvaluator`,
  `ContentHarmEvaluator`). Läuft in xUnit/MSTest/NUnit, also in genau der Pipeline, die
  du schon hast. Für dich ist das der kürzeste Weg von „müsste man mal" zu „läuft in CI".

**Observability**
- Tracing pro Request: Prompt, Modell, Tokens, Latenz, Tool-Calls, Kosten.
- **[OpenTelemetry GenAI Semantic Conventions](https://opentelemetry.io/docs/specs/semconv/gen-ai/)**
  — vendorneutral, und `Microsoft.Extensions.AI` emittiert sie bereits
  (`ChatClientBuilder.UseOpenTelemetry(...)`). **Aber:** die Konventionen stehen weiterhin
  auf *Development*, nichts ist als *Stable* markiert, und die Attributnamen haben sich
  seit 2024 mehrfach geändert. Trotzdem der richtige Weg — nur nicht davon ausgehen, dass
  deine Dashboards ein Upgrade unbeschadet überstehen. Das praktische Auswahlkriterium für
  ein Tool ist heute: nimmt es `gen_ai.*`-Spans über deinen bestehenden OTel-Collector
  entgegen, oder verlangt es ein eigenes SDK?

**Kosten & Latenz**
- Token-Budgets pro Feature, Prompt Caching, Batch-APIs (~50 % günstiger),
  **Effort-Stufen** (der erste Qualitäts-Kosten-Regler, siehe Stufe 2), Modell-Routing
  (kleines Modell zuerst, Eskalation nur bei Bedarf), Streaming für gefühlte Latenz,
  Semantic Caching.
- Reihenfolge, in der man das angeht: erst die kostenlosen Gewinne (Caching, Input-Hygiene,
  weniger Loop-Runden), dann erst die Abwägungen (Effort senken, Modell wechseln). Und:
  rechne **Kosten pro erledigter Aufgabe**, nicht pro Request — ein billigerer Request,
  der drei Runden mehr braucht, ist teurer. Vorsicht bei Modell-Kaskaden: Caches sind
  modellgebunden, eine Kaskade verschenkt also Cache-Treffer.

**Sicherheit**
- **Prompt Injection** — die zentrale neue Bedrohungsklasse und im
  **OWASP Top 10 for LLM Applications 2026** weiterhin auf Platz 1, gefolgt von
  Sensitive Information Disclosure. Merksatz: *jeder Inhalt, der ins Kontextfenster kommt,
  ist untrusted input.* Ein Agent mit Repo-Zugriff, der eine fremde Issue-Beschreibung
  liest, führt fremden Text als Anweisung aus. Das Problem ist architektonisch:
  Anweisungen und Daten teilen sich einen Kanal, ein Äquivalent zur parametrisierten
  Query gibt es nicht.
- **Excessive Agency** ist in der 2026er Liste von Platz 6 auf **Platz 3** gesprungen —
  die deutlichste Bewegung der Ausgabe und die direkte Folge davon, dass Agenten heute
  echte Systeme anfassen. Ergänzend gibt es eine eigene **OWASP Top 10 for Agentic
  Applications**; lies beide, wenn du Agenten baust.
- **Lethal Trifecta** (Simon Willison): Zugriff auf private Daten + untrusted content +
  Exfiltrationsmöglichkeit = Datenabfluss. Brich immer eines der drei.
- Least Privilege für Tools, Output-Validierung, PII-Redaction, Audit-Logs,
  Approval Gates für alles Schreibende.

### Kostenlose Quellen
- **[anthropics/courses → Prompt Evaluations](https://github.com/anthropics/courses)** — praxisnah.
- **[Microsoft Learn: AI evaluation for .NET](https://learn.microsoft.com/dotnet/ai/evaluation/libraries)** — dein Weg.
- **[promptfoo](https://github.com/promptfoo/promptfoo)** — Eval + Red-Teaming, CLI, CI-tauglich. Sehr guter Startpunkt.
- **[Ragas](https://github.com/explodinggradients/ragas)** / **[DeepEval](https://github.com/confident-ai/deepeval)** — Eval-Frameworks.
- **[Langfuse](https://github.com/langfuse/langfuse)** — Open-Source-LLM-Observability, self-hostbar, MIT;
  **[Arize Phoenix](https://github.com/Arize-ai/phoenix)** als OTel-natives Gegenstück.
- **[OWASP GenAI Security Project](https://genai.owasp.org/)** — Top 10 für LLM Apps *und* für Agentic Apps.
- **[Simon Willison zu Prompt Injection](https://simonwillison.net/tags/prompt-injection/)** —
  laufend aktualisiert, kein Hype.
- **[NIST AI Risk Management Framework](https://www.nist.gov/itl/ai-risk-management-framework)** — für formale Kontexte.

### Praxis
Nimm dein RAG-Projekt aus Stufe 4 und rüste nach: 30-Fälle-Eval-Set als xUnit-Suite mit
`Microsoft.Extensions.AI.Evaluation`, OTel-Tracing über `UseOpenTelemetry`, Kosten pro
Anfrage im Log. Dann tausche das Modell gegen ein kleineres — und lass die Zahlen
entscheiden, ob das trägt. Danach ein Red-Team-Durchlauf mit promptfoo gegen dein eigenes
System: schmuggle eine Anweisung in ein indexiertes Dokument und sieh zu, was passiert.

**Fertig, wenn:** du ein Modell wechseln kannst und **innerhalb von 10 Minuten** eine
belegte Aussage über Qualitäts-, Kosten- und Latenzänderung hast.

---

## Stufe 7 — Fundament: Wie das Ding von innen aussieht (4 Wochen, optional-aber-empfohlen)

**Ziel:** Kein Aberglaube mehr. Du weißt, was Training, Feintuning und Inferenz sind,
und kannst „lokales Modell" seriös bewerten.

### Inhalte
- Neuronale Netze, Backpropagation, Gradientenabstieg — auf Code-Ebene, nicht auf Formelebene.
- Die **Transformer-Architektur**: Attention, Positional Encoding, warum Kontext quadratisch kostet.
- Pretraining → SFT → RLHF/DPO: wie aus einem Textvorhersager ein Assistent wird.
  Ergänzt um das, was die aktuellen Reasoning-Modelle ausmacht: RL auf verifizierbaren
  Aufgaben (Mathe, Code mit Tests) — der Grund, warum diese Modelle ausgerechnet bei
  deiner Sorte Arbeit so stark sind.
- **Feintuning vs. RAG vs. Prompting** — Faustregel: Feintuning ändert *Verhalten und Format*,
  RAG liefert *Wissen*. Wer Feintuning für Wissensinjektion nutzt, hat sich vertan.
  LoRA/QLoRA machen es bezahlbar. Ergänzung 2026: Destillation eines großen Modells auf
  ein kleines für **eine** eng umrissene Aufgabe ist der häufigste Fall, in dem sich
  Feintuning betriebswirtschaftlich wirklich rechnet.
- **Lokale Modelle**: Quantisierung (Q4/Q8), VRAM-Bedarf, wann lokal sinnvoll ist
  (Datenschutz, Offline, Kosten bei Massenlast) und wann nicht (Qualität, Betrieb).
  Stand 2026 ist das Feld erwachsen geworden: **Qwen3** (Apache 2.0) ist für die meisten
  der pragmatische Default, **Qwen3-Coder-30B** das beste Preis-Leistungs-Verhältnis pro
  GB VRAM für Code, **gpt-oss-20b / gpt-oss-120b** (Apache 2.0, 128 K Kontext) OpenAIs
  Open-Weights-Linie. Mit ~24–32 GB VRAM oder einem 32-GB-Mac bist du im Spiel.
- **Small Language Models als Architekturthema, nicht als Sparmaßnahme.** Ein 7-B-Modell
  ist gegenüber einem 70–175-B-Modell etwa 10–30× günstiger in Latenz, Energie und FLOPs.
  Für die repetitiven Teilschritte eines Agenten (Routing, Extraktion, Formatierung,
  Klassifikation) reicht das meist — die teure Eskalation braucht nur der harte Rest.
  Lesetipp: das NVIDIA-Papier *„Small Language Models are the Future of Agentic AI"*
  ([arXiv:2506.02153](https://arxiv.org/abs/2506.02153)). Das ist die technische
  Begründung hinter dem Modell-Routing aus Stufe 6.

### Kostenlose Quellen
- **[Andrej Karpathy: Neural Networks — Zero to Hero](https://karpathy.ai/zero-to-hero.html)**
  ([GitHub](https://github.com/karpathy/nn-zero-to-hero),
  [YouTube-Playlist](https://www.youtube.com/playlist?list=PLAqhIrjkxbuWI23v9cThsA9GvCAUhRvKZ))
  — die mit Abstand beste kostenlose Ressource: Autograd-Engine, dann Sprachmodell,
  dann Tokenizer, dann ein GPT — alles from scratch, jede Zeile erklärt.
  **Mach mindestens `micrograd` (Video 1) und `Let's build GPT` mit.**
- **[karpathy/nanochat](https://github.com/karpathy/nanochat)** — inzwischen der
  interessantere Endpunkt als nanoGPT: die *komplette* Pipeline (Tokenizer → Pretraining
  → SFT → Inferenz → Web-UI) in einer lesbaren, abhängigkeitsarmen Codebasis. Ein
  eigenes ChatGPT-in-klein für ~15–50 $ Cloud-GPU-Zeit. Dient zugleich als Capstone für
  Karpathys angekündigten Kurs **LLM101n** (Eureka Labs).
  **[karpathy/nanoGPT](https://github.com/karpathy/nanoGPT)** bleibt der minimalere Einstieg.
- **[microsoft/AI-For-Beginners](https://github.com/microsoft/ai-for-beginners)** — 24 Lektionen ML/DL-Basis.
- **[fast.ai — Practical Deep Learning for Coders](https://course.fast.ai/)** — top-down,
  für Entwickler gemacht, komplett kostenlos.
- **[Hugging Face LLM Course](https://huggingface.co/learn/llm-course)**, Kapitel zu
  Finetuning, PEFT/LoRA.
- **[Ollama](https://github.com/ollama/ollama)** / **[llama.cpp](https://github.com/ggml-org/llama.cpp)**
  / **[LM Studio](https://lmstudio.ai/)** — lokale Modelle in 15 Minuten. Praktisch, weil
  deine Übungs-Repos damit offline laufen.

### Praxis
Karpathys `micrograd` nachbauen (nicht abtippen — mit Videos pausieren und selbst schreiben).
Ein Qwen3-Modell per Ollama lokal laufen lassen und mit derselben Eval-Suite aus Stufe 6
gegen dein Cloud-Modell antreten lassen. Das Ergebnis ist lehrreicher als jeder Blogpost —
und weil Ollama eine OpenAI-kompatible API ausliefert, kannst du es über einen
`IChatClient` einhängen, ohne deinen Code anzufassen. Genau dafür war die Abstraktion da.

**Fertig, wenn:** du in einem Meeting die Frage „Können wir das nicht einfach feintunen?"
fundiert beantworten kannst — in beide Richtungen.

---

## Stufe 8 — Die Architektenebene (2 Wochen, dann dauerhaft)

**Ziel:** Du entscheidest über KI-Einsatz im Unternehmen, nicht nur im Editor.

### Inhalte
- **Build vs. Buy vs. Compose**: Foundation-Model-API, Managed Service (**Microsoft
  Foundry** — so heißt Azure AI Foundry seit dem 1. Januar 2026 —, Amazon Bedrock,
  Google Vertex AI), Open-Weights self-hosted. Kriterien: Datenresidenz, Kostenmodell,
  Lock-in, Modellwechselfähigkeit. Achte auf Feature-Parität: nicht jede Fähigkeit der
  First-Party-API ist auf jeder Plattform verfügbar, und das ist selten dokumentiert,
  wo man es sucht.
- **Abstraktionsschichten als Versicherung**: `IChatClient` statt SDK-Direktaufrufe.
  Modelle veralten in Monaten — die Architektur muss das aushalten. Die Probe aufs
  Exempel ist Stufe 6: Wenn du ein Modell nicht in 10 Minuten mit belegter Aussage
  tauschen kannst, ist die Abstraktion Dekoration.
- **Offene Standards als zweite Versicherung.** MCP, `AGENTS.md`, `SKILL.md`, A2A und
  goose liegen seit Dezember 2025 bei der **Agentic AI Foundation** (Linux Foundation,
  getragen u. a. von AWS, Anthropic, Google, Microsoft, OpenAI, Bloomberg, Cloudflare).
  Für Beschaffung und Architektur heißt das: „unterstützt MCP" ist inzwischen ein
  legitimes Ausschreibungskriterium, kein Wunschdenken. Umgekehrt ist ein Werkzeug ohne
  MCP-/`AGENTS.md`-Unterstützung 2026 ein bewusster Lock-in.
- **Regulatorik — hier hat sich der Zeitplan verschoben, und zwar erheblich:**

  | Was | Wann | Status |
  |-----|------|--------|
  | Verbotene Praktiken, KI-Kompetenz | seit 2025-02-02 | in Kraft |
  | GPAI-Pflichten (General Purpose AI) | seit 2025-08-02 | in Kraft |
  | **Transparenzpflichten (Art. 50)** — Kennzeichnung von KI-Interaktion und -Inhalten | **seit 2026-08-02** | **in Kraft** (Wasserzeichenpflicht für bereits laufende Systeme mit Schonfrist bis 2026-12-02) |
  | Hochrisiko nach **Anhang III** (eigenständige Systeme) | ursprünglich 2026-08-02 → **2027-12-02** | verschoben |
  | Hochrisiko nach **Anhang I** (in Produkte eingebettet) | → **2028-08-02** | verschoben |

  Grundlage ist der **Digital Omnibus on AI**, veröffentlicht im Amtsblatt am 2026-07-24,
  in Kraft seit 2026-07-27 — sechs Tage vor der ursprünglichen Frist. Der häufigste
  Fehler in Diskussionen gerade: „Der AI Act ist verschoben." Nein — **verschoben sind
  die Hochrisiko-Pflichten, nicht die Transparenzpflichten.** Wenn dein Produkt Nutzer
  mit einem Chatbot sprechen lässt oder KI-Inhalte ausgibt, gilt das seit August 2026.
- **DSGVO bei Prompts und Logs** („Kein Kundendatum in Prompts" ist keine Policy, sondern
  eine Architekturentscheidung) — und beachte, dass Tracing (Stufe 6) genau die Daten
  persistiert, die du eben nicht persistieren wolltest. Redaction gehört in die
  Middleware, nicht in die Review-Checkliste.
- **Team-Enablement**: Coding-Agent-Richtlinien, Review-Pflicht für KI-Code,
  Umgang mit Lizenzfragen bei generiertem Code, geteilte `AGENTS.md`/`SKILL.md`-Bibliothek
  als Träger von Team-Konventionen.
- **Nicht-Ziele definieren**: Wo im Produkt KI *nicht* hingehört. Das ist oft die
  wertvollste Architekturaussage.

### Kostenlose Quellen
- **[EU AI Act Explorer](https://artificialintelligenceact.eu/)** — durchsuchbarer Volltext, Zeitplan.
- **[Microsoft Foundry Architecture Center](https://learn.microsoft.com/azure/architecture/ai-ml/)**
  — Referenzarchitekturen, Baseline-Designs.
- **[AWS Well-Architected — Generative AI Lens](https://docs.aws.amazon.com/wellarchitected/latest/generative-ai-lens/generative-ai-lens.html)**
  — herstellerlastig, aber die Checklisten sind übertragbar.
- **Agentic AI Foundation** — [aaif.io](https://aaif.io/) und die
  [Gründungsmitteilung der Linux Foundation](https://www.linuxfoundation.org/press/linux-foundation-announces-the-formation-of-the-agentic-ai-foundation):
  wer trägt die Standards, auf die du dich stützt.
- **[Google: Agents Companion Whitepaper](https://www.kaggle.com/whitepapers)** — Agent-Ops, Evaluation im Betrieb.

**Fertig, wenn:** du ein einseitiges ADR geschrieben hast, das für ein konkretes
Produkt festlegt: Modellstrategie, Datenfluss, Eval-Gate, Kostenlimit, Nicht-Ziele.

---

## Laufend: Auf dem Stand bleiben (dauerhaft, ~1 h/Woche)

Das Feld ändert sich schneller als Frameworks. Wenig, aber regelmäßig:

| Quelle | Warum |
|--------|-------|
| **[simonwillison.net](https://simonwillison.net/)** | Beste Signal-zu-Rausch-Ratio, entwicklerzentriert, täglich |
| **[Anthropic Engineering Blog](https://www.anthropic.com/engineering)** | Praxis-Patterns für Agenten und Tools |
| **[MCP Blog](https://blog.modelcontextprotocol.io/)** | Spec-Änderungen und Roadmap — nach der 2026-07-28-Revision Pflicht |
| **[.NET Blog / Agent Framework DevBlogs](https://devblogs.microsoft.com/dotnet/)** | dein Stack |
| **[Hugging Face Blog](https://huggingface.co/blog)** | Open-Weights-Ökosystem |
| **[Latent Space](https://www.latent.space/)** | Podcast + Newsletter, Engineering-Perspektive |
| **[OWASP GenAI Security Project](https://genai.owasp.org/)** | jährlich aktualisierte Bedrohungslage |
| **[arXiv cs.CL / Papers with Code](https://arxiv.org/list/cs.CL/recent)** | nur gezielt bei Bedarf |

**Regel:** Nicht jedem neuen Framework hinterherlaufen. Die Konzepte aus Stufe 3–6
(Tool Use, Retrieval, Evals, Tracing) sind seit Jahren stabil — nur die Verpackung wechselt.
Die Ausnahme, bei der Nachlesen wirklich nötig ist: wenn sich ein **Protokoll oder eine
API-Semantik** ändert (MCP 2026-07-28, das Verschwinden von `temperature`/`budget_tokens`).
Solche Änderungen brechen Code still, nicht laut.

---

## Vorschlag: Ein `ai/`-Track in diesem Repo

Dein Monorepo hat bereits ein bewährtes Muster: `exercises/<tier>/` mit rotem Test,
`solutions/<tier>/` mit Referenz, `catalog.md` als Ledger. Das passt perfekt auf diese
Roadmap — mit einer Anpassung: LLM-Ausgaben sind nichtdeterministisch, also sind die
Tests **Eval-Assertions** (Schema gültig, Tool wurde aufgerufen, Antwort enthält die
Quelle) statt exakter Vergleiche. Genau das ist die Lektion aus Stufe 6 — und mit
`Microsoft.Extensions.AI.Evaluation` bleibt es trotzdem eine gewöhnliche xUnit-Suite,
also kompatibel zum Rot/Grün-Muster aller anderen Tracks.

Zuordnung der Tiers:

| Tier | Nr. | Inhalt | Stufe |
|------|-----|--------|-------|
| `01-beginner` | 001–035 | Prompting, Tokens, erste API-Calls, Structured Output, Effort/Thinking | 1–3 |
| `02-intermediate` | 036–070 | Tool Use, Embeddings, RAG-Pipeline, Chunking, Hybrid Search, Reranking, Memory | 3–4 |
| `03-advanced` | 071–090 | Agent-Loop, MCP-Server (Spec 2026-07-28), `SKILL.md`, Multi-Agent, Evals, Tracing | 5–6 |
| `04-expert` | 091–100 | Cost/Latency-Tuning, Modell-Routing, Prompt-Injection-Härtung, lokale Modelle/SLMs, Feintuning | 6–7 |

Sprache: **C# als Leitsprache** (`Microsoft.Extensions.AI`, `Microsoft.Agents.AI`,
`Microsoft.Extensions.AI.Evaluation`, xUnit), **Python als Zweitsprache** für alles, wo
das Ökosystem eindeutig dort liegt (Embeddings, Eval-Tools, Karpathy-Material).
Offline-fähig über Ollama + Qwen3, damit die Tests ohne API-Key und ohne Kosten laufen —
was für einen Übungs-Track die härtere Anforderung ist als die Modellqualität.

Sag Bescheid, wenn ich den Track anlegen soll — Struktur, `catalog.md` und die ersten
fünf Übungen nach dem Rot/Grün-Muster der anderen Tracks.

---

## Die vier häufigsten Fehler auf diesem Weg

1. **Bei Stufe 1 stehenbleiben.** Prompting ist die Eintrittskarte, nicht das Ziel.
   Der Wert für dich als Architekt liegt in Stufe 4–6.
2. **Stufe 6 überspringen.** Ein RAG-Demo baut man an einem Wochenende. Ein RAG-System,
   dessen Qualität man *messen* kann, unterscheidet Profis von Bastlern. Die Zahlen aus
   Stufe 6 sagen dasselbe: Tracing haben fast alle, Evals die Hälfte — und genau daran
   entscheidet sich, ob etwas in Produktion bleibt.
3. **Frameworks vor Primitiven lernen.** Wer LangChain vor dem rohen Tool-Call-Loop lernt,
   kann nicht debuggen, wenn es klemmt — und es klemmt.
4. **Auf Modellnamen optimieren statt auf das Harness.** 2026 ist der Modellwechsel eine
   Konfigurationszeile; was den Unterschied macht, sind Kontext, Tools, Evals und Grenzen.
   Wer Halbtagesdiskussionen über die Modellwahl führt und keine Eval-Suite hat, optimiert
   die falsche Variable.

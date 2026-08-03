# KI-Roadmap für Softwareentwickler

Ein mehrstufiger Einarbeitungsplan — zugeschnitten auf einen **Senior .NET-Architekten**,
der KI (a) im eigenen Arbeitsalltag nutzen und (b) in eigene Software einbauen und
verantworten will.

> Stand: 2026-08-03. Kurse und Repos ändern sich schnell; die verlinkten Anlaufstellen
> (`anthropic.com/learn`, `huggingface.co/learn`, `deeplearning.ai/courses`,
> `learn.microsoft.com/dotnet/ai`) sind stabil, einzelne Kurstitel darunter nicht.
> Alles hier Verlinkte ist **kostenlos** nutzbar (Zertifikate teils kostenpflichtig);
> API-Nutzung kostet Geld — dazu Stufe 6.

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

---

## Stufe 1 — KI als Werkzeug im eigenen Alltag (3 Wochen)

**Ziel:** LLMs sind ab jetzt Teil deines Werkzeugkastens, nicht Spielerei.
Du weißt, wann sie helfen und wann sie dich verlangsamen.

### Inhalte
- Chat-Interfaces vs. **agentische Coding-Tools** (Claude Code, GitHub Copilot,
  Cursor, JetBrains AI) — der Unterschied ist gewaltig, siehe unten.
- Prompting als Handwerk: Kontext liefern, Rolle/Format vorgeben, Beispiele geben
  (Few-Shot), Aufgaben zerlegen, das Modell erst planen lassen.
- **Context Engineering**: Was das Modell sieht, entscheidet über die Qualität —
  mehr als jede Prompt-Formulierung. Bei Coding-Agents: `CLAUDE.md`/`AGENTS.md`,
  gezieltes Öffnen von Dateien, kurze Sessions statt endloser Threads.
- Wo LLMs zuverlässig sind (Boilerplate, Tests, Migrationen, Reviews, Doku,
  fremde Sprachen lernen — genau dein FeWoLearning-Fall) und wo nicht
  (exakte Zahlen, aktuelle API-Signaturen, alles ohne Verifikation).

### Kostenlose Quellen
- **[Anthropic Academy](https://www.anthropic.com/learn)** (Kurse auf
  [anthropic.skilljar.com](https://anthropic.skilljar.com)) — seit März 2026,
  ~18 kostenlose Kurse mit Zertifikat. Für dich: *Claude Code 101*,
  *Claude Code in Action*, *Claude with the API*.
- **[anthropics/courses](https://github.com/anthropics/courses)** — Jupyter-Notebooks:
  API Fundamentals, **Prompt Engineering Tutorial**, Real World Prompting,
  Prompt Evaluations, Tool Use. Das ist der beste kostenlose Prompting-Kurs, den es gibt.
- **[Google „Prompt Engineering" Whitepaper](https://www.kaggle.com/whitepaper-prompt-engineering)**
  (Lee Boonstra) — 60 Seiten, dicht, kein Marketing.
- **[ChatGPT Prompt Engineering for Developers](https://www.deeplearning.ai/courses/)**
  (DeepLearning.AI, ~1,5 h) — schneller Einstieg, Andrew Ng + OpenAI.
- **[Claude Code Docs](https://docs.claude.com/en/docs/claude-code/overview)** /
  **[GitHub Copilot Docs](https://docs.github.com/en/copilot)**.

### Praxis
1. Führe **eine ganze Arbeitswoche** ausschließlich mit einem Coding-Agent im
   Beifahrersitz. Nicht Autocomplete — echte Aufgaben delegieren.
2. Schreib dir eine persönliche `prompts.md` mit 10 wiederkehrenden Mustern
   („Review dieses Diffs auf Nebenwirkungen", „Portiere diese Klasse nach Go", …).
3. Lege in einem echten Repo eine `CLAUDE.md`/`AGENTS.md` an und miss, ob die
   Ergebnisse besser werden. (Dein FeWoLearning-Repo hat schon eine — gutes Beispiel.)

**Fertig, wenn:** du bei einer neuen Aufgabe *automatisch* abwägst „selbst tippen
oder delegieren?" und in mindestens zwei Fällen begründet **selbst** tippst.

---

## Stufe 2 — Das mentale Modell: Was ein LLM wirklich tut (2 Wochen)

**Ziel:** Du kannst erklären, warum ein Modell halluziniert, warum es rechnen nicht kann,
warum derselbe Prompt zweimal Verschiedenes liefert — ohne Mystik, ohne Mathe-Tiefe.

### Inhalte
- **Tokens** (und warum „Zähle die r in strawberry" scheitert), Tokenizer, Kosten pro Token.
- **Context Window**, „Lost in the middle", Context Rot bei langen Sessions.
- **Sampling**: temperature, top-p — warum LLMs nichtdeterministisch sind und was das
  für Tests bedeutet.
- **Embeddings & Vektorähnlichkeit** — die Grundlage von Stufe 4.
- Modellklassen: groß vs. klein, **Reasoning-Modelle** (denken vor dem Antworten) vs.
  schnelle Modelle, multimodal, lokal vs. Cloud.
- Grenzen: Wissensstichtag, Konfidenz ≠ Korrektheit, Prompt Injection (Vorschau auf Stufe 6).

### Kostenlose Quellen
- **[microsoft/generative-ai-for-beginners](https://github.com/microsoft/generative-ai-for-beginners)**
  — 21 Lektionen, Lektion 1–6 reichen hier. Beste strukturierte Gratis-Basis.
- **[Hugging Face LLM Course](https://huggingface.co/learn/llm-course)** — Kapitel 1–2
  (Transformers, Tokenizer) für das konzeptionelle Fundament.
- **[Andrej Karpathy: „Deep Dive into LLMs like ChatGPT"](https://www.youtube.com/watch?v=7xTGNNLPyMI)**
  (~3,5 h) und **[„Intro to LLMs"](https://www.youtube.com/watch?v=zjkBMFhNj_g)** (1 h) —
  wenn du nur *eine* Sache aus dieser Stufe machst, dann diese.
- **[Tiktokenizer](https://tiktokenizer.vercel.app/)** — Tokens live sehen. 10 Minuten, großer Aha-Effekt.
- **[The Illustrated Transformer](https://jalammar.github.io/illustrated-transformer/)** (Jay Alammar)
  — der Klassiker, falls du visuell lernst.

### Praxis
- Nimm einen 10-Seiten-Text, tokenisiere ihn, rechne die Kosten für drei Modelle aus.
- Stelle dieselbe Frage 10× bei `temperature=0` und `temperature=1`. Dokumentiere die Streuung.
- Erkläre einem Kollegen in 5 Minuten, was ein Embedding ist. Wenn du ins Stocken kommst → nochmal.

**Fertig, wenn:** du bei einem Fehlverhalten des Modells eine *Hypothese* hast
(Tokenisierung? Kontext zu lang? Sampling? Wissensstichtag?) statt „KI halt".

---

## Stufe 3 — Erste eigene Integration: API, Structured Output, Tool Use (3 Wochen)

**Ziel:** Du hast LLM-Funktionalität aus **eigenem C#-Code** aufgerufen, mit typisierten
Ergebnissen und Werkzeugaufrufen. Ab hier ist es normale Softwareentwicklung.

### Inhalte
- Messages-API: system/user/assistant, Multi-Turn, Streaming, Stop-Reasons, Token-Limits.
- **Structured Output / JSON-Schema** — der wichtigste Hebel für produktiven Einsatz:
  LLM-Output wird zu einem `record`, nicht zu einem String, den du parsen musst.
- **Tool Use / Function Calling** — das Modell fordert Funktionsaufrufe an, *dein* Code
  führt sie aus. Kern jeder späteren Agentenarchitektur.
- **Prompt Caching** — 5–10× Kostenhebel bei wiederholtem System-Kontext.
- Fehlerbehandlung: Rate Limits, Retries mit Backoff, Timeouts, Idempotenz.

### Der .NET-Stack (dein Heimvorteil)
| Baustein | Was es ist |
|----------|-----------|
| **[`Microsoft.Extensions.AI`](https://learn.microsoft.com/dotnet/ai/)** | die Abstraktionsschicht (`IChatClient`, `IEmbeddingGenerator`) — DI-, Logging-, OTel-fähig. **Hier anfangen.** |
| **[Microsoft Agent Framework](https://github.com/microsoft/agent-framework)** | Konvergenz aus Semantic Kernel + AutoGen; Orchestrierung, Workflows, Multi-Agent |
| **[Semantic Kernel](https://github.com/microsoft/semantic-kernel)** | der etablierte Vorgänger, weiterhin gepflegt und auf `Microsoft.Extensions.AI` migriert |
| **[dotnet/ai-samples](https://github.com/dotnet/ai-samples)** | offizielle, lauffähige Beispiele |

Denk in bekannten Mustern: `IChatClient` ist ein Interface wie `HttpClient` —
registriere es in DI, dekoriere es (Logging, Caching, Telemetrie), mocke es im Test.
Das ist bewusst so gebaut, damit du kein neues Paradigma lernen musst.

### Kostenlose Quellen
- **[microsoft/Generative-AI-for-beginners-dotnet](https://github.com/microsoft/Generative-AI-for-beginners-dotnet)**
  — 5 Lektionen, kurze Videos, lauffähiger .NET-Code. Exakt dein Einstiegspunkt.
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

**Fertig, wenn:** du eine typisierte C#-Struktur zurückbekommst, deren Felder das Modell
verlässlich füllt, und du einen Tool-Call-Loop selbst geschrieben hast (kein Framework).

---

## Stufe 4 — RAG: Das Modell an deine Daten anschließen (4 Wochen)

**Ziel:** Du kannst eine Frage-Antwort-Funktion über Dokumente bauen, *und* du weißt,
warum die meisten RAG-Prototypen in Produktion enttäuschen.

### Inhalte
- Die Pipeline: **Ingest → Chunking → Embedding → Vektorspeicher → Retrieval → Rerank → Generierung**.
- **Chunking ist der Hebel Nr. 1** (Größe, Overlap, semantisch vs. fix, Metadaten mitführen).
- **Hybrid Search**: BM25/Volltext + Vektor. Reine Vektorsuche ist fast immer schlechter.
- **Reranking** (Cross-Encoder) — größter Qualitätssprung pro Aufwand.
- Vektorspeicher: pgvector (Postgres, meist die richtige Antwort), Qdrant, Azure AI Search,
  SQLite-vec für lokal. Fürs Verständnis: einmal ohne Datenbank, nur mit Cosine-Similarity im Speicher.
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
- **[pgvector](https://github.com/pgvector/pgvector)** + **[Pgvector.EntityFrameworkCore](https://github.com/pgvector/pgvector-dotnet)**
  — für dich vermutlich der pragmatischste Weg.
- **[Ragas](https://github.com/explodinggradients/ragas)** — RAG-Metriken (Faithfulness,
  Context Precision/Recall). Bereitet Stufe 6 vor.

### Praxisprojekt: „Frag dein Repo"
Indexiere die Markdown-Dateien deines FeWoLearning-Monorepos (`CLAUDE.md`, alle
`catalog.md`, `docs/`) und beantworte Fragen wie „Welche Rust-Übungen fehlen noch und
warum?" — **mit Dateiangabe als Beleg**. Danach: baue absichtlich eine Frage ein, die
die Daten nicht hergeben, und bring das System dazu, das zuzugeben.

**Fertig, wenn:** du eine Retrieval-Qualitätsmessung hast (z. B. 20 Frage/Quelle-Paare,
Recall@5) und **Zahlen** dafür, wie viel Hybrid Search und Reranking jeweils bringen.

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
- **Workflow vs. Agent** — Anthropics „Building Effective Agents" ist hier der beste Text
  der Branche: deterministische Workflows schlagen Agenten fast immer. Agenten nur,
  wenn die Schrittfolge *wirklich* unbekannt ist.
- **MCP (Model Context Protocol)** — der De-facto-Standard, um Tools/Daten an Modelle
  anzubinden (JSON-RPC, Tools/Resources/Prompts). Für dich als Architekt der relevanteste
  Integrationsstandard der letzten Jahre: einmal ein Tool als MCP-Server bauen, jeder
  Client kann es nutzen.
- Kontrolle: Human-in-the-Loop, Approval Gates, Sandboxing, Kosten- und Schrittlimits,
  Abbruchbedingungen.

### Kostenlose Quellen
- **[Anthropic: Building Effective Agents](https://www.anthropic.com/engineering/building-effective-agents)**
  — 20 Minuten Lesezeit, Pflichtlektüre.
- **[Anthropic: Writing Effective Tools for Agents](https://www.anthropic.com/engineering/writing-tools-for-agents)**
  — Tool-Design ist API-Design; das trifft direkt deine Kernkompetenz.
- **[Hugging Face AI Agents Course](https://huggingface.co/learn/agents-course)**
  — kostenlos, zertifiziert, mit smolagents / LlamaIndex / LangGraph.
- **[Hugging Face MCP Course](https://huggingface.co/learn/mcp-course)** — MCP von Grund auf.
- **[modelcontextprotocol.io](https://modelcontextprotocol.io)** + **[MCP C# SDK](https://github.com/modelcontextprotocol/csharp-sdk)**
  (von Microsoft mitgepflegt) — Spezifikation und dein SDK.
- **[microsoft/ai-agents-for-beginners](https://github.com/microsoft/ai-agents-for-beginners)** — 18 Lektionen.
- **[LangGraph](https://github.com/langchain-ai/langgraph)** + **[LangChain Academy](https://academy.langchain.com/)**
  (kostenlos) — falls du im Python-Ökosystem mitreden musst. Für .NET-Produktion:
  Microsoft Agent Framework statt LangChain.

### Praxisprojekt: eigener MCP-Server
Bau einen MCP-Server in C# für dein FeWoLearning-Repo mit den Tools
`list_exercises(track, tier)`, `run_tests(track, filter)`, `catalog_status()`.
Häng ihn an Claude Code und lass den Agenten den Katalogstand prüfen.
Danach: schreib den Agent-Loop einmal *ohne* Framework — 80 Zeilen, und du verstehst
jedes Framework danach in einer Stunde.

**Fertig, wenn:** du in einem Architektur-Review begründen kannst, warum eine gegebene
Anforderung **kein** Agent sein sollte.

---

## Stufe 6 — Produktionsreife: Evals, Observability, Kosten, Sicherheit (4 Wochen)

**Ziel:** Das ist die Stufe, die aus einem beeindruckenden Demo ein System macht, das
du unterschreiben kannst. Als Architekt ist das **deine wichtigste Stufe** — und die,
die 90 % aller Tutorials überspringen.

### Inhalte

**Evaluation (Testing für Nichtdeterminismus)**
- Eval-Datensätze aufbauen: 30–50 reale Fälle schlagen jede synthetische Suite.
- Metriken: exakte Übereinstimmung, Schema-Validität, **LLM-as-Judge** (und dessen Bias),
  paarweise Vergleiche, Regressionstests bei Modell- oder Promptwechsel.
- In CI verankern: Prompts sind Code. Prompt-Änderung ohne Eval-Lauf = ungetesteter Deploy.

**Observability**
- Tracing pro Request: Prompt, Modell, Tokens, Latenz, Tool-Calls, Kosten.
- **[OpenTelemetry GenAI Semantic Conventions](https://opentelemetry.io/docs/specs/semconv/gen-ai/)**
  — vendorneutral, und `Microsoft.Extensions.AI` emittiert sie bereits.

**Kosten & Latenz**
- Token-Budgets pro Feature, Prompt Caching, Batch-APIs (~50 % günstiger),
  Modell-Routing (kleines Modell zuerst, Eskalation nur bei Bedarf), Streaming
  für gefühlte Latenz, Semantic Caching.

**Sicherheit**
- **Prompt Injection** — die zentrale neue Bedrohungsklasse. Merksatz: *jeder Inhalt,
  der ins Kontextfenster kommt, ist untrusted input.* Ein Agent mit Repo-Zugriff, der
  eine fremde Issue-Beschreibung liest, führt fremden Text als Anweisung aus.
- **Lethal Trifecta** (Simon Willison): Zugriff auf private Daten + untrusted content +
  Exfiltrationsmöglichkeit = Datenabfluss. Brich immer eines der drei.
- Least Privilege für Tools, Output-Validierung, PII-Redaction, Audit-Logs.

### Kostenlose Quellen
- **[anthropics/courses → Prompt Evaluations](https://github.com/anthropics/courses)** — praxisnah.
- **[promptfoo](https://github.com/promptfoo/promptfoo)** — Eval + Red-Teaming, CLI, CI-tauglich. Sehr guter Startpunkt.
- **[Ragas](https://github.com/explodinggradients/ragas)** / **[DeepEval](https://github.com/confident-ai/deepeval)** — Eval-Frameworks.
- **[Langfuse](https://github.com/langfuse/langfuse)** — Open-Source-LLM-Observability, self-hostbar.
- **[OWASP Top 10 for LLM Applications](https://genai.owasp.org/)** — die Referenz für Bedrohungsmodellierung.
- **[Simon Willison zu Prompt Injection](https://simonwillison.net/tags/prompt-injection/)** —
  laufend aktualisiert, kein Hype.
- **[NIST AI Risk Management Framework](https://www.nist.gov/itl/ai-risk-management-framework)** — für formale Kontexte.

### Praxis
Nimm dein RAG-Projekt aus Stufe 4 und rüste nach: 30-Fälle-Eval-Set, promptfoo in der
Pipeline, OTel-Tracing, Kosten pro Anfrage im Log. Dann tausche das Modell gegen ein
kleineres — und lass die Zahlen entscheiden, ob das trägt.

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
- **Feintuning vs. RAG vs. Prompting** — Faustregel: Feintuning ändert *Verhalten und Format*,
  RAG liefert *Wissen*. Wer Feintuning für Wissensinjektion nutzt, hat sich vertan.
  LoRA/QLoRA machen es bezahlbar.
- **Lokale Modelle**: Quantisierung (Q4/Q8), VRAM-Bedarf, wann lokal sinnvoll ist
  (Datenschutz, Offline, Kosten bei Massenlast) und wann nicht (Qualität, Betrieb).

### Kostenlose Quellen
- **[Andrej Karpathy: Neural Networks — Zero to Hero](https://karpathy.ai/zero-to-hero.html)**
  ([GitHub](https://github.com/karpathy/nn-zero-to-hero),
  [YouTube-Playlist](https://www.youtube.com/playlist?list=PLAqhIrjkxbuWI23v9cThsA9GvCAUhRvKZ))
  — die mit Abstand beste kostenlose Ressource: Autograd-Engine, dann Sprachmodell,
  dann Tokenizer, dann ein GPT — alles from scratch, jede Zeile erklärt.
  **Mach mindestens `micrograd` (Video 1) und `Let's build GPT` mit.**
- **[karpathy/nanoGPT](https://github.com/karpathy/nanoGPT)** und
  **[karpathy/nanochat](https://github.com/karpathy/nanochat)** — die komplette Pipeline
  in lesbarem Code.
- **[microsoft/AI-For-Beginners](https://github.com/microsoft/ai-for-beginners)** — 24 Lektionen ML/DL-Basis.
- **[fast.ai — Practical Deep Learning for Coders](https://course.fast.ai/)** — top-down,
  für Entwickler gemacht, komplett kostenlos.
- **[Hugging Face LLM Course](https://huggingface.co/learn/llm-course)**, Kapitel zu
  Finetuning, PEFT/LoRA.
- **[Ollama](https://github.com/ollama/ollama)** / **[llama.cpp](https://github.com/ggml-org/llama.cpp)**
  — lokale Modelle in 15 Minuten. Praktisch, weil deine Übungs-Repos damit offline laufen.

### Praxis
Karpathys `micrograd` nachbauen (nicht abtippen — mit Videos pausieren und selbst schreiben).
Ein 7B-Modell per Ollama lokal laufen lassen und mit derselben Eval-Suite aus Stufe 6
gegen dein Cloud-Modell antreten lassen. Das Ergebnis ist lehrreicher als jeder Blogpost.

**Fertig, wenn:** du in einem Meeting die Frage „Können wir das nicht einfach feintunen?"
fundiert beantworten kannst — in beide Richtungen.

---

## Stufe 8 — Die Architektenebene (2 Wochen, dann dauerhaft)

**Ziel:** Du entscheidest über KI-Einsatz im Unternehmen, nicht nur im Editor.

### Inhalte
- **Build vs. Buy vs. Compose**: Foundation-Model-API, Managed Service (Azure AI Foundry,
  Bedrock), Open-Weights self-hosted. Kriterien: Datenresidenz, Kostenmodell, Lock-in,
  Modellwechselfähigkeit.
- **Abstraktionsschichten als Versicherung**: `IChatClient` statt SDK-Direktaufrufe.
  Modelle veralten in Monaten — die Architektur muss das aushalten.
- **Regulatorik**: EU AI Act (Risikoklassen, Transparenzpflichten, GPAI-Regeln;
  Anwendung gestaffelt seit 2025/2026), DSGVO bei Prompts und Logs
  („Kein Kundendatum in Prompts" ist keine Policy, sondern eine Architekturentscheidung).
- **Team-Enablement**: Coding-Agent-Richtlinien, Review-Pflicht für KI-Code,
  Umgang mit Lizenzfragen bei generiertem Code.
- **Nicht-Ziele definieren**: Wo im Produkt KI *nicht* hingehört. Das ist oft die
  wertvollste Architekturaussage.

### Kostenlose Quellen
- **[EU AI Act Explorer](https://artificialintelligenceact.eu/)** — durchsuchbarer Volltext, Zeitplan.
- **[Azure AI Foundry Architecture Center](https://learn.microsoft.com/azure/architecture/ai-ml/)**
  — Referenzarchitekturen, Baseline-Designs.
- **[AWS Well-Architected — Generative AI Lens](https://docs.aws.amazon.com/wellarchitected/latest/generative-ai-lens/generative-ai-lens.html)**
  — herstellerlastig, aber die Checklisten sind übertragbar.
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
| **[.NET Blog / Semantic Kernel & Agent Framework DevBlogs](https://devblogs.microsoft.com/dotnet/)** | dein Stack |
| **[Hugging Face Blog](https://huggingface.co/blog)** | Open-Weights-Ökosystem |
| **[Latent Space](https://www.latent.space/)** | Podcast + Newsletter, Engineering-Perspektive |
| **[arXiv cs.CL / Papers with Code](https://arxiv.org/list/cs.CL/recent)** | nur gezielt bei Bedarf |

**Regel:** Nicht jedem neuen Framework hinterherlaufen. Die Konzepte aus Stufe 3–6
(Tool Use, Retrieval, Evals, Tracing) sind seit Jahren stabil — nur die Verpackung wechselt.

---

## Vorschlag: Ein `ai/`-Track in diesem Repo

Dein Monorepo hat bereits ein bewährtes Muster: `exercises/<tier>/` mit rotem Test,
`solutions/<tier>/` mit Referenz, `catalog.md` als Ledger. Das passt perfekt auf diese
Roadmap — mit einer Anpassung: LLM-Ausgaben sind nichtdeterministisch, also sind die
Tests **Eval-Assertions** (Schema gültig, Tool wurde aufgerufen, Antwort enthält die
Quelle) statt exakter Vergleiche. Genau das ist die Lektion aus Stufe 6.

Zuordnung der Tiers:

| Tier | Nr. | Inhalt | Stufe |
|------|-----|--------|-------|
| `01-beginner` | 001–035 | Prompting, Tokens, erste API-Calls, Structured Output | 1–3 |
| `02-intermediate` | 036–070 | Tool Use, Embeddings, RAG-Pipeline, Chunking, Hybrid Search | 3–4 |
| `03-advanced` | 071–090 | Agent-Loop, MCP-Server, Multi-Agent, Evals, Tracing | 5–6 |
| `04-expert` | 091–100 | Cost/Latency-Tuning, Prompt-Injection-Härtung, lokale Modelle, Feintuning | 6–7 |

Sprache: **C# als Leitsprache** (`Microsoft.Extensions.AI`, xUnit), **Python als
Zweitsprache** für alles, wo das Ökosystem eindeutig dort liegt (Embeddings, Eval-Tools,
Karpathy-Material). Offline-fähig über Ollama, damit Tests ohne API-Key laufen.

Sag Bescheid, wenn ich den Track anlegen soll — Struktur, `catalog.md` und die ersten
fünf Übungen nach dem Rot/Grün-Muster der anderen Tracks.

---

## Die drei häufigsten Fehler auf diesem Weg

1. **Bei Stufe 1 stehenbleiben.** Prompting ist die Eintrittskarte, nicht das Ziel.
   Der Wert für dich als Architekt liegt in Stufe 4–6.
2. **Stufe 6 überspringen.** Ein RAG-Demo baut man an einem Wochenende. Ein RAG-System,
   dessen Qualität man *messen* kann, unterscheidet Profis von Bastlern.
3. **Frameworks vor Primitiven lernen.** Wer LangChain vor dem rohen Tool-Call-Loop lernt,
   kann nicht debuggen, wenn es klemmt — und es klemmt.

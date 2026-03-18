Var embeddings är extremt användbara

I din typ av SaaS:

schema mapping
attribute mapping
category matching
product classification

Det är ofta mycket billigare och snabbare än LLM.



Kort sammanfattning
Teknik	Vad den gör
Embeddings	omvandlar text till matematiska vektorer
Vector search	hittar liknande betydelser
LLM	resonemang och generering
RAG	embeddings + LLM + dokument



Hur du preppar data för embeddings (best practice)
1. Skicka inte bara fältnamn

❌ Dåligt:

color_name

✔ Bra:

field: color_name, description: product color

👉 Embeddings funkar mycket bättre med lite kontext

2. Lägg till beskrivningar (om du har)

✔ Ännu bättre:

field: material_info, description: primary fabric of the product

👉 Detta är ofta den största kvalitetsökningen

3. Normalisera format

Gör detta innan embedding:

snake_case → ord

camelCase → ord

ta bort prefix

Exempel:

product_color_name → product color
4. Samma språk (viktigt)

✔ Bäst:

allt på engelska

Om du har:

färg → color

👉 översätt först (enkelt steg, stor effekt)

5. Lägg till kontext per system

Exempel:

"PIM field: color_name, description: product color"
"Zalando attribute: color, description: main product color"

👉 hjälper modellen förstå roll + meaning

6. (Bonus) Lägg till exempel

Om du har historik:

color_name → color
colour → color

👉 kan användas senare för ännu bättre matchning

🔥 Minimal “gold standard format”

För bästa resultat i din use case:

"{system}: {field}, description: {meaning}"

Exempel:

"PIM: color_name, description: product color"
"Zalando: color, description: primary product color"



Varför embeddings > ren LLM (i detta steg)
1. Mycket snabbare & billigare

Embeddings:

✔ millisekunder
✔ billigt

LLM:

❌ långsammare
❌ dyrare
2. Stabilare resultat

Embeddings:

✔ samma input → samma output

LLM:

❌ kan variera
❌ kan hallucinera
3. Perfekt för “matching problem”

Din use case:

fält A ≈ fält B

👉 detta är exakt vad embeddings är byggt för

4. Skalar mycket bättre

1000 fält:

embeddings → snabbt

LLM → dyrt och segt

5. Mindre risk för fel

Embeddings:

→ rankar likhet

LLM:

→ kan hitta på mapping
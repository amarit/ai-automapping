Hur man vanligtvis gör

En typisk struktur i en integrationsplattform ser ut så här:

RAG system
   │
   ├── Zalando knowledge base
   │     ├ attributes
   │     ├ category tree
   │     ├ validation rules
   │     └ examples
   │
   ├── Amazon knowledge base
   │     ├ attributes
   │     ├ category tree
   │     ├ variation rules
   │     └ examples
   │
   └── Shopify knowledge base
         ├ attributes
         └ product model


Typisk AI-pipeline i din typ av SaaS
Customer connects PIM
        │
        ▼
Extract schema
        │
        ▼
Detect product category
        │
        ▼
RAG search
(Zalando + category)
        │
        ▼
LLM mapping
        │
        ▼
Validation rules
        │
        ▼
Transformation engine
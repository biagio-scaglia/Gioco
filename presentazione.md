---
marp: true
theme: default
class: lead
paginate: true
backgroundColor: #111111
color: #eeeeee
---

# 🔴 Il Mio Primo Gioco in .NET 🔴
**Sviluppo di un Platformer con Raylib-cs**
*Presentato da Biagio Scaglia*

---

## 🎮 Che Cos'è?
Abbiamo creato un vero e proprio gioco platform 2D partendo da una console application vuota in **C# / .NET 10**.

- Grafica fluida a **60 FPS**.
- Fisica completa (Gravità, collisioni solide, salto).
- **Gestione a Stati**: Menu Principale, Opzioni e Gameplay.
- Niente framework giganti come Unity: solo puro codice e matematica!

---

![bg right:60% fit](assets/sprites/run3.png)

## 🦔 Il Protagonista: Knuckles

Abbiamo programmato una **Macchina a Stati dell'Animazione** (Animation State Machine) personalizzata!

A seconda dell'input del giocatore, le texture cambiano in tempo reale:
- **Idle** quando è fermo.
- Array di sprite sequenziali (`run1` -> `run6`) per la corsa.
- Sprite dipendenti dalla velocità verticale per il salto (`jump1` per la salita, `jump2` per la caduta).

---

## 💍 Raccogliere Anelli (E suoni!)

Un vero platformer ha bisogno di oggetti da raccogliere:
- Ogni anello è un oggetto OOP (`Class Ring`).
- Usa la collisione AABB (Rettangolo contro Rettangolo).
- Implementa eventi Audio: `.mp3` sparato in tempo reale al tocco!
- Aggiorna a schermo un sistema di Scoring dinamico.

**Menzione d'onore:** Abbiamo inserito uno sfondo *Parallax* per dare un senso di vastità al livello!

---

## 🤝 Open Source su GitHub

Il codice è interamente caricato, versionato e disponibile su GitHub!

🔗 **[https://github.com/biagio-scaglia/Gioco](https://github.com/biagio-scaglia/Gioco)**

Sentiti libero di clonare la repository, cambiare le texture in `assets/` e creare il tuo gioco in 5 minuti.

---

# 🚀 Grazie per l'Attenzione!
*Pronti per il prossimo livello!*

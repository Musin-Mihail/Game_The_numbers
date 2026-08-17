# TypeScript-типы SDK

Официальная страница: https://yandex.ru/dev/games/doc/ru/sdk/typescript.md

Для игр на TypeScript Яндекс рекомендует пакет [@types/ysdk](https://www.npmjs.com/package/@types/ysdk) (DefinitelyTyped). Эта игра на C#; пакет нужен как **контракт API**.

Локальная копия `1.2.0` (август 2025, на момент снимка 2026-08-18 — последний релиз): [`types/ysdk.d.ts`](../types/ysdk.d.ts).

```typescript
import type { SDK, Player } from 'ysdk';

const ysdk: SDK = await YaGames.init();
const player: Player = await ysdk.getPlayer();
```

Сигнатуры `adv`, `leaderboards`, `payments`, `Player.getData` / `setData` брать из `.d.ts`, пояснения и ограничения — из страниц `sdk/*.md`.

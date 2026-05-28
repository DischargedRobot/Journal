# LessonTable — журнал занятий

Таблица журнала: **студенты в строках**, **занятия в колонках**. Построена на [AG Grid](https://www.ag-grid.com/) (`ag-grid-react`).

Публичный API: `@/entities/lesson` → `LessonTable`.

## Возможности

- Фиксированные колонки слева (№, ФИО) и справа (%, Был/все, Аттестация, меню) — `pinned`.
- Динамические колонки по списку `lessons` — группа из 3 строк шапки (занятие → тема → Б/Н / Оценка).
- **Перестановка колонок** drag-and-drop (`marryChildren` — блок занятия двигается целиком).
- Множественный выбор строк (клик toggle, подсветка, контролируемый режим).
- Пустое состояние («Нет данных»).
- Горизонтальная прокрутка.

## Использование

```tsx
import { LessonTable } from "@/entities/lesson"
import type { TJournalRow, TLesson } from "@/shared/model/lesson"

<LessonTable
  lessons={lessons}
  rows={journalRows}
  selectedRowUuids={["uuid-1", "uuid-2"]}
  onRowSelect={(uuids) => {}}
  onHeaderMoreToolsClick={() => {}}
  onRowMoreToolsClick={(row) => {}}
  moreToolsButton={({ onClick, row }) => (
    <MoreToolsButton
      items={row ? rowMenuItems(row) : headerMenuItems}
      onMenuClick={onClick}
      sx={row ? undefined : { color: "common.white" }}
    />
  )}
/>
```

## Props (`LessonTableProps`)

| Prop                | Тип              | Обязательный | Описание                                                               |
| ------------------- | ---------------- | ------------ | ---------------------------------------------------------------------- |
| `lessons`           | `TLesson[]`      | да           | Занятия — источник колонок                                             |
| `rows`              | `TJournalRow[]`  | да           | Студенты и ячейки журнала                                              |
| `selectedRowUuids`  | `string[]`       | нет          | Контролируемый набор выбранных строк. Без prop — state внутри таблицы |
| `onRowSelect`       | `(uuids) => void`| нет          | Клик по строке: добавляет/убирает uuid из выбора (toggle)              |
| `onHeaderMoreToolsClick` | `() => void` | нет | Клик по ⋮ в шапке (пробрасывается в `moreToolsButton`) |
| `onRowMoreToolsClick` | `(row) => void` | нет | Клик по ⋮ в строке |
| `moreTools` | `boolean` | нет | Колонка ⋮; по умолчанию `true` |
| `moreToolsButton` | `ComponentType<{ onClick, row? }>` | нет | Кастомная кнопка; по умолчанию встроенная ⋮ |

## Структура файлов

```
lesson-table/
  LessonTable.tsx         # AgGridReact, тема, события
  buildColumnDefs.ts      # pinned + группы колонок занятий
  LessonGridMenuCells.tsx # ⋮ в шапке и строках
  useOrderedLessons.ts    # порядок колонок после drag
  lessonFormat.ts         # дата, тема
  lesson-grid.css         # стили шапки и выделения строки
  agGridSetup.ts          # ModuleRegistry (AllCommunityModule)
  types.ts
```

## Перетаскивание колонок

`onColumnMoved` → `syncLessonOrder` → `orderedLessons` → пересборка `columnDefs`.

Порядок хранится в локальном state (`lessonOrder`), при перезагрузке сбрасывается.

## Ограничения

- Редактирование ячеек (Б/Н, оценка) не реализовано — только отображение.
- Превью колонки при drag (как было с dnd-kit `DragOverlay`) в AG Grid не воспроизведено — стандартный UX перемещения колонок.

"use client"

import "./agGridSetup"
import "./lesson-grid.css"

import { useTheme } from "@mui/material/styles"
import type { CellClickedEvent, ColumnMovedEvent, GridApi, RowClassParams, RowClassRules } from "ag-grid-community"
import { themeQuartz } from "ag-grid-community"
import { AgGridReact } from "ag-grid-react"
import type { TJournalRow, TLesson } from "@/shared/model/lesson"
import { ComponentType, memo, useCallback, useEffect, useMemo, useRef, useState } from "react"
import type { AgGridReact as AgGridReactType } from "ag-grid-react"
import { buildLessonColumnDefs } from "./buildColumnDefs"
import DefaultLessonMoreToolsButton from "./DefaultLessonMoreToolsButton"
import type { LessonGridContext, LessonMoreToolsButtonProps } from "./types"
import { useOrderedLessons } from "./useOrderedLessons"

// настройки по умолчанию для колонок
const defaultColDef = {
    sortable: false,
    filter: false,
    resizable: true,
    suppressHeaderMenuButton: true,
} as const

const toggleRowInSelection = (selected: string[], uuid: string): string[] =>
    selected.includes(uuid) ? selected.filter((id) => id !== uuid) : [...selected, uuid]


const sumDisplayedColumnsWidth = (api: GridApi<TJournalRow>): number =>
    (api.getAllDisplayedColumns() ?? []).reduce(
        (sum, column) => sum + column.getActualWidth(),
        0,
    )

// расчёт минимальной ширины грида
const sumDisplayedColumnsMinWidth = (api: GridApi<TJournalRow>): number =>
    (api.getAllDisplayedColumns() ?? []).reduce((sum, column) => {
        const minWidth = column.getMinWidth()
        return sum + (minWidth > 0 ? minWidth : column.getActualWidth())
    }, 0)

// используется для закрепления порядка колонок при перетаскивании
// (В случае ререндера таблицы, порядок колонок может измениться, к примеру,
// если пользователь переетащил, потом переключил ввидимость одной из колонок,
// то порядок колонок будет изменен, тогда порядок будет браться не тот что 
// в гриде был после перестановки до переключения видимости, а то в каком передали массив)
const extractLessonOrder = (event: ColumnMovedEvent): string[] => {
    const order: string[] = []

    // получаем все отображаемые колонки (уже имеет новый порядок)
    const displayedColumns = event.api.getAllDisplayedColumns() ?? []
    for (const col of displayedColumns) {
        const colId = col.getColId()
        if (colId.endsWith("_presence")) {
            order.push(colId.replace(/_presence$/, "")) // добавляем в порядок колонку без "_presence"
        }
    }
    return order // возвращаем порядок колонок
}



export type Props = {
    lessons: TLesson[]
    rows: TJournalRow[]
    // Контролируемый набор выбранных строк (uuid студентов).
    selectedRowUuids?: string[]
    onRowSelect?: (uuids: string[]) => void
    // Клик по ⋮ в шапке (если не обрабатывается внутри `moreToolsButton`).
    onHeaderMoreToolsClick?: () => void
    // Клик по ⋮ в строке (если не обрабатывается внутри `moreToolsButton`).
    onRowMoreToolsClick?: (row: TJournalRow) => void
    // Показать колонку ⋮ (по умолчанию `true`).
    showMoreTools?: boolean
    // Кастомная кнопка ⋮; по умолчанию — встроенная иконка.
    moreToolsButton?: ComponentType<LessonMoreToolsButtonProps>

    onPresenceCellClick?: (
        params: CellClickedEvent<TJournalRow>,
        lesson: TLesson,
    ) => void
}

const LessonTable = ({
    lessons,
    rows,
    selectedRowUuids: selectedRowUuidsProp,
    onRowSelect,
    onHeaderMoreToolsClick,
    onRowMoreToolsClick,
    showMoreTools = true,
    moreToolsButton,
    onPresenceCellClick,
}: Props) => {
    const MoreToolsButtonComponent = moreToolsButton ?? DefaultLessonMoreToolsButton
    const theme = useTheme()
    const gridRef = useRef<AgGridReactType<TJournalRow>>(null)

    const [selectedRowUuidsInner, setSelectedRowUuidsInner] = useState<string[]>([])
    const { orderedLessons, syncLessonOrder } = useOrderedLessons(lessons)

    // используется для управления выбранными строками
    const selectedRowUuids = selectedRowUuidsProp ?? selectedRowUuidsInner
    const selectedSet = useMemo(() => new Set(selectedRowUuids), [selectedRowUuids])

    // стркои есть - автовысота, иначе фиксированная высота с "Нет данных"
    const hasRows = rows.length > 0
    const domLayout = hasRows ? "autoHeight" : "normal"

    // тема для компонента AgGridReact
    const gridTheme = useMemo(
        () =>
            themeQuartz.withParams({
                headerBackgroundColor: theme.palette.primary.main,
                headerTextColor: theme.palette.primary.contrastText,
                headerCellHoverBackgroundColor: theme.palette.primary.light,
                rowHoverColor: "none",
                selectedRowBackgroundColor: "none",
                borderColor: theme.palette.divider,
                wrapperBorder: false,
            }),
        [theme],
    )

    // колонки для компонента AgGridReact
    const columnDefs = useMemo(
        () => buildLessonColumnDefs(orderedLessons, onPresenceCellClick, { showMoreTools }),
        [onPresenceCellClick, orderedLessons, showMoreTools],
    )

    // контекст для компонента AgGridReact, передается в компонент AgGridReact
    const gridContext = useMemo<LessonGridContext>(
        () => ({
            onHeaderMoreToolsClick,
            onRowMoreToolsClick,
            // тут передается компонент кнопки ⋮, который будет использоваться в компоненте AgGridReact
            moreToolsButton: MoreToolsButtonComponent,
        }),
        [onHeaderMoreToolsClick, onRowMoreToolsClick, MoreToolsButtonComponent],
    )


    // используется для обработки клика по строке
    const handleRowClick = useCallback(
        (uuid: string) => {
            const nextUuids = toggleRowInSelection(selectedRowUuids, uuid)

            if (selectedRowUuidsProp === undefined) {
                setSelectedRowUuidsInner(nextUuids)
            }
            onRowSelect?.(nextUuids)
        },
        [onRowSelect, selectedRowUuids, selectedRowUuidsProp],
    )

    // используется для синхронизации порядка колонок при перетаскивании
    const handleColumnMoved = useCallback(
        (event: ColumnMovedEvent) => {
            if (!event.finished) {
                return
            }

            const lessonOrder = extractLessonOrder(event)
            if (lessonOrder.length > 0) {
                syncLessonOrder(lessonOrder)
            }
        },
        [syncLessonOrder],
    )

    const [gridWidth, setGridWidth] = useState<number>()
    const [gridMinWidth, setGridMinWidth] = useState<number>()
    // используется для синхронизации ширины грида
    const syncGridWidth = useCallback((api: GridApi<TJournalRow>) => {
        const width = sumDisplayedColumnsWidth(api)
        const minWidth = sumDisplayedColumnsMinWidth(api)

        if (width > 0) {
            setGridWidth(width)
        }
        if (minWidth > 0) {
            setGridMinWidth(minWidth)
        }
    }, [])

    // используется для установки domLayout в компоненте AgGridReact
    useEffect(() => {
        const api = gridRef.current?.api
        if (!api) {
            return
        }

        api.setGridOption("domLayout", domLayout)
    }, [domLayout])

    useEffect(() => {
        const api = gridRef.current?.api
        if (!api) {
            return
        }

        syncGridWidth(api)
    }, [columnDefs, syncGridWidth])

    // для настройки выделения строк
    const isPrevRowSelected = useCallback(
        (params: RowClassParams<TJournalRow>, selectedSet: Set<string>): boolean => {
            const rowIndex = params.node.rowIndex
            if (rowIndex == null || rowIndex <= 0) {
                return false
            }
            const prevRow = params.api.getDisplayedRowAtIndex(rowIndex - 1)
            return prevRow?.data?.student.uuid != null && selectedSet.has(prevRow.data.student.uuid)
        },
        [],
    )

    const isNextRowSelected = useCallback(
        (params: RowClassParams<TJournalRow>, selectedSet: Set<string>): boolean => {
            const rowIndex = params.node.rowIndex
            if (rowIndex == null) {
                return false
            }
            const nextRow = params.api.getDisplayedRowAtIndex(rowIndex + 1)
            return nextRow?.data?.student.uuid != null && selectedSet.has(nextRow.data.student.uuid)
        },
        [],
    )

    // выставляем классы для строк по правилам
    const rowClassRules = useMemo<RowClassRules<TJournalRow>>(
        () => ({
            "lesson-grid-row-selected": (params) =>
                Boolean(params.data?.student.uuid && selectedSet.has(params.data.student.uuid)),
            "lesson-grid-row-selected-first": (params) => {
                if (!params.data?.student.uuid || !selectedSet.has(params.data.student.uuid)) {
                    return false
                }
                return !isPrevRowSelected(params, selectedSet)
            },
            "lesson-grid-row-selected-last": (params) => {
                if (!params.data?.student.uuid || !selectedSet.has(params.data.student.uuid)) {
                    return false
                }
                return !isNextRowSelected(params, selectedSet)
            },
        }),
        [isNextRowSelected, isPrevRowSelected, selectedSet],
    )

    // TODO: доделать так чтобы ширина грида была 1000px но изменялись раземеры колонок если не хватает столбцов
    const gridStyle =
        gridWidth || gridMinWidth
            ? {
                width: gridWidth,
                minWidth: gridMinWidth ?? gridWidth,
            }
            : undefined


    return (
        <div
            className={`lesson-grid  max-w-full overflow-x-auto ${hasRows ? "lesson-grid--auto" : "lesson-grid--empty"}`}
            style={{ width: "1000px" }}
        >
            <AgGridReact<TJournalRow>
                ref={gridRef}
                theme={gridTheme}
                rowData={rows}
                columnDefs={columnDefs}
                defaultColDef={defaultColDef}
                context={gridContext}
                getRowId={({ data }) => data.student.uuid}
                // высота грида по контенту
                domLayout={domLayout}
                // синхронизация ширины грида при загрузке
                onGridReady={({ api }) => syncGridWidth(api)}
                // синхронизация ширины грида при изменении ширины колонок
                onColumnResized={(event) => {
                    if (event.finished) {
                        syncGridWidth(event.api)
                    }
                }}
                onRowClicked={({ data, event }) => {
                    const target = event?.target as HTMLElement | undefined
                    if (target?.closest(".lesson-grid-more-tools") || !data) {
                        return
                    }

                    const cellEl = target?.closest(".ag-cell")
                    const colId = cellEl?.getAttribute("col-id")
                    if (colId?.endsWith("_presence")) {
                        return;
                    }

                    handleRowClick(data.student.uuid)
                }}
                rowSelection={{
                    mode: "multiRow",
                    checkboxes: false,        // нет чекбокса в строках
                    headerCheckbox: false,    // нет чекбокса в шапке
                    enableClickSelection: false, // выбор кликом по строке
                    enableSelectionWithoutKeys: true, // выбор без клавиш
                }}
                suppressCellFocus
                onColumnMoved={(event) => {
                    handleColumnMoved(event)
                    if (event.finished) {
                        syncGridWidth(event.api)
                    }
                }}
                rowClassRules={rowClassRules}
                suppressColumnMoveAnimation={false}
                overlayNoRowsTemplate='<span class="ag-overlay-no-rows-center">Нет данных</span>'
            />
        </div>
    )
}

export default memo(LessonTable)

"use client"

import "./agGridSetup"
import "./lesson-grid.css"

import Paper from "@mui/material/Paper"
import { useTheme } from "@mui/material/styles"
import type { ColumnMovedEvent } from "ag-grid-community"
import { themeQuartz } from "ag-grid-community"
import { AgGridReact } from "ag-grid-react"
import type { TJournalRow } from "@/shared/model/lesson"
import { memo, useCallback, useEffect, useMemo, useRef, useState } from "react"
import type { AgGridReact as AgGridReactType } from "ag-grid-react"
import { buildLessonColumnDefs } from "./buildColumnDefs"
import DefaultLessonMoreToolsButton from "./DefaultLessonMoreToolsButton"
import type { LessonGridContext, LessonTableProps } from "./types"
import { useOrderedLessons } from "./useOrderedLessons"

const defaultColDef = {
    sortable: false,
    filter: false,
    resizable: true,
    suppressHeaderMenuButton: true,
} as const

const toggleRowInSelection = (selected: string[], uuid: string): string[] =>
    selected.includes(uuid) ? selected.filter((id) => id !== uuid) : [...selected, uuid]

const extractLessonOrder = (event: ColumnMovedEvent): string[] => {
    const order: string[] = []

    for (const col of event.api.getAllDisplayedColumns() ?? []) {
        const colId = col.getColId()
        if (colId.endsWith("_presence")) {
            order.push(colId.replace(/_presence$/, ""))
        }
    }

    return order
}

const LessonTable = ({
    lessons,
    rows,
    selectedRowUuids: selectedRowUuidsProp,
    onRowSelect,
    onHeaderMoreToolsClick,
    onRowMoreToolsClick,
    showMoreTools = true,
    moreToolsButton: moreToolsButtonProp,
}: LessonTableProps) => {
    const MoreToolsButtonComponent = moreToolsButtonProp ?? DefaultLessonMoreToolsButton
    const theme = useTheme()
    const gridRef = useRef<AgGridReactType<TJournalRow>>(null)
    const [selectedRowUuidsInner, setSelectedRowUuidsInner] = useState<string[]>([])
    const { orderedLessons, syncLessonOrder } = useOrderedLessons(lessons)

    const selectedRowUuids = selectedRowUuidsProp ?? selectedRowUuidsInner
    const selectedSet = useMemo(() => new Set(selectedRowUuids), [selectedRowUuids])
    const hasRows = rows.length > 0
    const domLayout = hasRows ? "autoHeight" : "normal"

    useEffect(() => {
        gridRef.current?.api?.redrawRows()
    }, [selectedRowUuids])

    const gridTheme = useMemo(
        () =>
            themeQuartz.withParams({
                headerBackgroundColor: theme.palette.primary.main,
                headerTextColor: theme.palette.primary.contrastText,
                headerCellHoverBackgroundColor: theme.palette.primary.dark,
                borderColor: theme.palette.divider,
                wrapperBorder: false,
            }),
        [theme],
    )

    const columnDefs = useMemo(
        () => buildLessonColumnDefs(orderedLessons, { showMoreTools }),
        [orderedLessons, showMoreTools],
    )

    const gridContext = useMemo<LessonGridContext>(
        () => ({
            onHeaderMoreToolsClick,
            onRowMoreToolsClick,
            moreToolsButton: MoreToolsButtonComponent,
        }),
        [onHeaderMoreToolsClick, onRowMoreToolsClick, MoreToolsButtonComponent],
    )

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

    useEffect(() => {
        const api = gridRef.current?.api
        if (!api) {
            return
        }

        api.setGridOption("domLayout", domLayout)
    }, [domLayout])

    const getRowClass = useCallback(
        (params: { data?: TJournalRow }) =>
            params.data && selectedSet.has(params.data.uuid)
                ? "lesson-grid-row-selected"
                : undefined,
        [selectedSet],
    )

    return (
        <Paper
            elevation={0}
            className={`w-full max-w-full overflow-hidden rounded-2xl border lesson-grid ${hasRows ? "lesson-grid--auto" : "lesson-grid--empty"}`}
            sx={{
                borderColor: "divider",
                "--lesson-grid-primary": theme.palette.primary.main,
            }}
        >
            <AgGridReact<TJournalRow>
                ref={gridRef}
                theme={gridTheme}
                rowData={rows}
                columnDefs={columnDefs}
                defaultColDef={defaultColDef}
                context={gridContext}
                getRowId={({ data }) => data.uuid}
                domLayout={domLayout}
                onRowClicked={({ data, event }) => {
                    const target = event?.target as HTMLElement | undefined
                    if (target?.closest(".lesson-grid-more-tools") || !data) {
                        return
                    }
                    handleRowClick(data.uuid)
                }}
                suppressCellFocus
                onColumnMoved={handleColumnMoved}
                getRowClass={getRowClass}
                suppressColumnMoveAnimation={false}
                overlayNoRowsTemplate='<span class="ag-overlay-no-rows-center">Нет данных</span>'
            />
        </Paper>
    )
}

export default memo(LessonTable)

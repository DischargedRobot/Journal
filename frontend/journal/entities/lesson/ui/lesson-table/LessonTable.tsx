"use client"

import {
    closestCenter,
    DndContext,
    DragOverlay,
    type DragEndEvent,
    type DragOverEvent,
    type DragStartEvent,
    KeyboardSensor,
    PointerSensor,
    useSensor,
    useSensors,
} from "@dnd-kit/core"
import {
    horizontalListSortingStrategy,
    SortableContext,
    sortableKeyboardCoordinates,
} from "@dnd-kit/sortable"
import MoreVertIcon from "@mui/icons-material/MoreVert"
import IconButton from "@mui/material/IconButton"
import Paper from "@mui/material/Paper"
import Table from "@mui/material/Table"
import TableBody from "@mui/material/TableBody"
import TableCell from "@mui/material/TableCell"
import TableContainer from "@mui/material/TableContainer"
import TableHead from "@mui/material/TableHead"
import TableRow from "@mui/material/TableRow"
import { memo, useMemo, useState } from "react"
import { LessonBodyCells } from "./LessonBodyCells"
import { LessonColumnOverlay } from "./LessonColumnOverlay"
import { LessonDragContext } from "./LessonDragContext"
import {
    LessonMetaHeaderCell,
    LessonSubHeaderCells,
    LessonTopicHeaderCell,
} from "./LessonHeaderCells"
import { LessonSortableHandlesProvider } from "./LessonSortableHandlesProvider"
import { bodyCellSx, headerCellSx } from "./styles"
import type { LessonTableProps } from "./types"
import { useOrderedLessons } from "./useOrderedLessons"

const HEADER_ROW_SPAN = 3
const LESSON_COL_WIDTH = 196

const LessonTable = ({
    lessons,
    rows,
    selectedRowUuid: selectedRowUuidProp,
    onRowSelect,
    onRowMenuClick,
    onHeaderMenuClick,
}: LessonTableProps) => {
    const [selectedRowUuidInner, setSelectedRowUuidInner] = useState<string | null>(null)
    /** Какая колонка сейчас перетаскивается (для overlay и затемнения) */
    const [activeLessonUuid, setActiveLessonUuid] = useState<string | null>(null)

    const selectedRowUuid = selectedRowUuidProp ?? selectedRowUuidInner
    const { orderedLessons, moveLesson } = useOrderedLessons(lessons)

    // distance: 6 — не путать клик по строке с началом drag
    const sensors = useSensors(
        useSensor(PointerSensor, { activationConstraint: { distance: 6 } }),
        useSensor(KeyboardSensor, {
            coordinateGetter: sortableKeyboardCoordinates,
        }),
    )

    const activeLesson = useMemo(
        () => orderedLessons.find((lesson) => lesson.uuid === activeLessonUuid) ?? null,
        [activeLessonUuid, orderedLessons],
    )

    // для сенсоров
    const handleRowClick = (uuid: string) => {
        if (selectedRowUuidProp === undefined) {
            setSelectedRowUuidInner(uuid)
        }
        onRowSelect?.(uuid)
    }

    const handleLessonDragStart = ({ active }: DragStartEvent) => {
        setActiveLessonUuid(String(active.id))
    }

    // Колонки «разъезжаются» уже при движении, не только после отпускания
    const handleLessonDragOver = ({ active, over }: DragOverEvent) => {
        moveLesson(String(active.id), over ? String(over.id) : undefined)
    }

    const handleLessonDragEnd = ({ active, over }: DragEndEvent) => {
        moveLesson(String(active.id), over ? String(over.id) : undefined)
        setActiveLessonUuid(null)
    }

    const handleLessonDragCancel = () => {
        setActiveLessonUuid(null)
    }

    return (
        <LessonDragContext.Provider value={activeLessonUuid}>
            {/* DndContext: сенсоры, коллизии, жизненный цикл drag */}
            <DndContext
                sensors={sensors}
                collisionDetection={closestCenter}
                onDragStart={handleLessonDragStart}
                onDragOver={handleLessonDragOver}
                onDragEnd={handleLessonDragEnd}
                onDragCancel={handleLessonDragCancel}
            >
                <TableContainer
                    component={Paper}
                    elevation={0}
                    className="w-full max-w-full overflow-auto rounded-2xl border"
                    sx={{ borderColor: "divider" }}
                >
                    <Table size="small" stickyHeader sx={{ minWidth: 720 }}>
                        {/* Sortable только для заголовков; тело следует за orderedLessons */}
                        <SortableContext
                            items={orderedLessons.map((lesson) => lesson.uuid)}
                            strategy={horizontalListSortingStrategy}
                        >
                            <LessonSortableHandlesProvider lessons={orderedLessons}>
                                <TableHead>
                                    <TableRow>
                                        <TableCell rowSpan={HEADER_ROW_SPAN} sx={headerCellSx}>
                                            №
                                        </TableCell>
                                        <TableCell
                                            rowSpan={HEADER_ROW_SPAN}
                                            sx={{
                                                ...headerCellSx,
                                                minWidth: 140,
                                                textAlign: "left",
                                            }}
                                        >
                                            Фамилия И.О.
                                        </TableCell>
                                        {orderedLessons.map((lesson) => (
                                            <LessonMetaHeaderCell
                                                key={`${lesson.uuid}-meta`}
                                                lesson={lesson}
                                            />
                                        ))}
                                        <TableCell rowSpan={HEADER_ROW_SPAN} sx={headerCellSx}>
                                            %
                                        </TableCell>
                                        <TableCell rowSpan={HEADER_ROW_SPAN} sx={headerCellSx}>
                                            Был/все
                                        </TableCell>
                                        <TableCell rowSpan={HEADER_ROW_SPAN} sx={headerCellSx}>
                                            Аттестация
                                        </TableCell>
                                        <TableCell
                                            rowSpan={HEADER_ROW_SPAN}
                                            sx={{ ...headerCellSx, px: 0.5 }}
                                        >
                                            <IconButton
                                                size="small"
                                                onClick={onHeaderMenuClick}
                                                sx={{ color: "common.white" }}
                                                aria-label="Меню таблицы"
                                            >
                                                <MoreVertIcon fontSize="small" />
                                            </IconButton>
                                        </TableCell>
                                    </TableRow>
                                    <TableRow>
                                        {orderedLessons.map((lesson) => (
                                            <LessonTopicHeaderCell
                                                key={`${lesson.uuid}-topic`}
                                                lesson={lesson}
                                            />
                                        ))}
                                    </TableRow>
                                    <TableRow>
                                        {orderedLessons.map((lesson) => (
                                            <LessonSubHeaderCells
                                                key={`${lesson.uuid}-sub`}
                                                lesson={lesson}
                                            />
                                        ))}
                                    </TableRow>
                                </TableHead>
                            </LessonSortableHandlesProvider>
                        </SortableContext>

                        {/* Порядок ячеек = orderedLessons; dnd-kit здесь не используется */}
                        <TableBody>
                            {rows.length === 0 ? (
                                <TableRow>
                                    <TableCell
                                        colSpan={2 + orderedLessons.length * 2 + 4}
                                        align="center"
                                        sx={bodyCellSx}
                                    >
                                        Нет данных
                                    </TableCell>
                                </TableRow>
                            ) : (
                                rows.map((row) => {
                                    const isSelected = selectedRowUuid === row.uuid

                                    return (
                                        <TableRow
                                            key={row.uuid}
                                            hover
                                            selected={isSelected}
                                            onClick={() => handleRowClick(row.uuid)}
                                            sx={{
                                                cursor: "pointer",
                                                bgcolor: "background.paper",
                                                ...(isSelected && {
                                                    boxShadow: 2,
                                                    "& .journal-row-index": {
                                                        borderLeft: 4,
                                                        borderLeftColor: "primary.main",
                                                        borderLeftStyle: "solid",
                                                    },
                                                }),
                                            }}
                                        >
                                            <TableCell
                                                className="journal-row-index"
                                                sx={{ ...bodyCellSx, textAlign: "left" }}
                                            >
                                                {row.order}
                                            </TableCell>
                                            <TableCell
                                                sx={{ ...bodyCellSx, textAlign: "left" }}
                                            >
                                                {row.fullName}
                                            </TableCell>
                                            {orderedLessons.map((lesson) => (
                                                <LessonBodyCells
                                                    key={`${row.uuid}-${lesson.uuid}`}
                                                    lesson={lesson}
                                                    row={row}
                                                />
                                            ))}
                                            <TableCell sx={bodyCellSx}>{row.percent}</TableCell>
                                            <TableCell sx={bodyCellSx}>
                                                {row.attendedTotal}
                                            </TableCell>
                                            <TableCell sx={bodyCellSx}>
                                                {row.attestation}
                                            </TableCell>
                                            <TableCell sx={{ ...bodyCellSx, px: 0.5 }}>
                                                <IconButton
                                                    size="small"
                                                    onClick={(event) => {
                                                        event.stopPropagation()
                                                        onRowMenuClick?.(row)
                                                    }}
                                                    aria-label="Меню строки"
                                                >
                                                    <MoreVertIcon fontSize="small" />
                                                </IconButton>
                                            </TableCell>
                                        </TableRow>
                                    )
                                })
                            )}
                        </TableBody>
                    </Table>
                </TableContainer>
                {/* dropAnimation={null} — без анимации «приземления» после отпускания */}
                <DragOverlay dropAnimation={null}>
                    {activeLesson ? (
                        <LessonColumnOverlay
                            lesson={activeLesson}
                            rows={rows}
                            colWidth={LESSON_COL_WIDTH}
                        />
                    ) : null}
                </DragOverlay>
            </DndContext>
        </LessonDragContext.Provider>
    )
}

export default memo(LessonTable)

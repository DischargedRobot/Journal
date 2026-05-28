"use client"

import { TJournalRow, TLesson } from "@/shared/model/lesson"
import MoreVertIcon from "@mui/icons-material/MoreVert"
import IconButton from "@mui/material/IconButton"
import Paper from "@mui/material/Paper"
import Table from "@mui/material/Table"
import TableBody from "@mui/material/TableBody"
import TableCell from "@mui/material/TableCell"
import TableContainer from "@mui/material/TableContainer"
import TableHead from "@mui/material/TableHead"
import TableRow from "@mui/material/TableRow"
import type { SxProps, Theme } from "@mui/material/styles"
import { memo, useState } from "react"

interface Props {
    lessons: TLesson[]
    rows: TJournalRow[]
    selectedRowUuid?: string
    onRowSelect?: (uuid: string) => void
    onRowMenuClick?: (row: TJournalRow) => void
    onHeaderMenuClick?: () => void
}

const HEADER_ROW_SPAN = 3

const headerCellSx: SxProps<Theme> = {
    bgcolor: "primary.main",
    color: "common.white",
    borderColor: "primary.dark",
    borderWidth: 1,
    borderStyle: "solid",
    fontWeight: 600,
    textAlign: "center",
    verticalAlign: "middle",
    px: 1,
    py: 0.75,
    lineHeight: 1.25,
    whiteSpace: "nowrap",
}

const bodyCellSx: SxProps<Theme> = {
    borderColor: "divider",
    borderWidth: 1,
    borderStyle: "solid",
    textAlign: "center",
    verticalAlign: "middle",
    px: 1,
    py: 1,
    fontSize: "0.875rem",
}

const formatLessonDate = (iso: string) => {
    const date = new Date(iso)
    const day = String(date.getDate()).padStart(2, "0")
    const month = String(date.getMonth() + 1).padStart(2, "0")
    return `${day}.${month}`
}

const getLessonTopic = (lesson: TLesson) =>
    lesson.name ?? lesson.shortName ?? `Занятие ${lesson.code}`

const LessonTable = ({
    lessons,
    rows,
    selectedRowUuid: selectedRowUuidProp,
    onRowSelect,
    onRowMenuClick,
    onHeaderMenuClick,
}: Props) => {
    const [selectedRowUuidInner, setSelectedRowUuidInner] = useState<string | null>(
        null,
    )
    const selectedRowUuid = selectedRowUuidProp ?? selectedRowUuidInner

    const handleRowClick = (uuid: string) => {
        if (selectedRowUuidProp === undefined) {
            setSelectedRowUuidInner(uuid)
        }
        onRowSelect?.(uuid)
    }

    return (
        <TableContainer
            component={Paper}
            elevation={0}
            sx={{
                width: "100%",
                maxWidth: "100%",
                overflow: "auto",
                borderRadius: 2,
                border: 1,
                borderColor: "divider",
            }}
        >
            <Table size="small" stickyHeader sx={{ minWidth: 720 }}>
                <TableHead>
                    <TableRow>
                        <TableCell rowSpan={HEADER_ROW_SPAN} sx={headerCellSx}>
                            №
                        </TableCell>
                        <TableCell
                            rowSpan={HEADER_ROW_SPAN}
                            sx={{ ...headerCellSx, minWidth: 140, textAlign: "left" }}
                        >
                            Фамилия И.О.
                        </TableCell>
                        {lessons.map((lesson) => (
                            <TableCell
                                key={`${lesson.uuid}-meta`}
                                colSpan={2}
                                sx={headerCellSx}
                            >
                                №{lesson.code} {formatLessonDate(lesson.startDate)}
                            </TableCell>
                        ))}
                        <TableCell rowSpan={HEADER_ROW_SPAN} sx={headerCellSx}>
                            %
                        </TableCell>
                        <TableCell rowSpan={HEADER_ROW_SPAN} sx={headerCellSx}>
                            Был/все
                        </TableCell>
                        <TableCell rowSpan={HEADER_ROW_SPAN} sx={headerCellSx}>
                            Атестация
                        </TableCell>
                        <TableCell rowSpan={HEADER_ROW_SPAN} sx={{ ...headerCellSx, px: 0.5 }}>
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
                        {lessons.map((lesson) => (
                            <TableCell
                                key={`${lesson.uuid}-topic`}
                                colSpan={2}
                                sx={{
                                    ...headerCellSx,
                                    whiteSpace: "normal",
                                    maxWidth: 160,
                                }}
                            >
                                {getLessonTopic(lesson)}
                            </TableCell>
                        ))}
                    </TableRow>
                    <TableRow>
                        {lessons.flatMap((lesson) => [
                            <TableCell key={`${lesson.uuid}-presence`} sx={headerCellSx}>
                                Б/Н
                            </TableCell>,
                            <TableCell key={`${lesson.uuid}-grade`} sx={headerCellSx}>
                                Оценка
                            </TableCell>,
                        ])}
                    </TableRow>
                </TableHead>
                <TableBody>
                    {rows.length === 0 ? (
                        <TableRow>
                            <TableCell
                                colSpan={2 + lessons.length * 2 + 4}
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
                                    <TableCell sx={{ ...bodyCellSx, textAlign: "left" }}>
                                        {row.fullName}
                                    </TableCell>
                                    {lessons.flatMap((lesson) => {
                                        const cell = row.cells[lesson.uuid] ?? {
                                            presence: "",
                                            grade: "",
                                        }

                                        return [
                                            <TableCell
                                                key={`${row.uuid}-${lesson.uuid}-presence`}
                                                sx={bodyCellSx}
                                            >
                                                {cell.presence}
                                            </TableCell>,
                                            <TableCell
                                                key={`${row.uuid}-${lesson.uuid}-grade`}
                                                sx={bodyCellSx}
                                            >
                                                {cell.grade}
                                            </TableCell>,
                                        ]
                                    })}
                                    <TableCell sx={bodyCellSx}>{row.percent}</TableCell>
                                    <TableCell sx={bodyCellSx}>{row.attendedTotal}</TableCell>
                                    <TableCell sx={bodyCellSx}>{row.attestation}</TableCell>
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
    )
}
export default memo(LessonTable)
"use client"

import Box from "@mui/material/Box"
import { formatLessonDate, getLessonTopic } from "./lessonFormat"
import { headerCellSx, overlayBodyCellSx } from "./styles"
import type { LessonColumnOverlayProps } from "./types"

/** «Призрак» колонки под курсором; не в потоке таблицы, pointerEvents отключены. */
export const LessonColumnOverlay = ({ lesson, rows, colWidth }: LessonColumnOverlayProps) => (
    <Box
        sx={{
            width: colWidth,
            bgcolor: "primary.main",
            color: "common.white",
            border: 1,
            borderColor: "primary.dark",
            borderRadius: 1,
            boxShadow: 6,
            overflow: "hidden",
            pointerEvents: "none",
        }}
    >
        <Box sx={{ ...headerCellSx, borderRadius: 0 }}>
            №{lesson.code} {formatLessonDate(lesson.startDate)}
        </Box>
        <Box
            sx={{
                ...headerCellSx,
                whiteSpace: "normal",
                minHeight: 48,
                display: "flex",
                alignItems: "center",
                justifyContent: "center",
            }}
        >
            {getLessonTopic(lesson)}
        </Box>
        <Box sx={{ display: "flex" }}>
            <Box sx={{ ...headerCellSx, flex: 1, borderRadius: 0 }}>Б/Н</Box>
            <Box sx={{ ...headerCellSx, flex: 1, borderRadius: 0 }}>Оценка</Box>
        </Box>
        {rows.map((row) => {
            const cell = row.cells[lesson.uuid] ?? { presence: "", grade: "" }

            return (
                <Box key={row.uuid} sx={{ display: "flex", bgcolor: "background.paper" }}>
                    <Box sx={overlayBodyCellSx}>{cell.presence}</Box>
                    <Box sx={overlayBodyCellSx}>{cell.grade}</Box>
                </Box>
            )
        })}
    </Box>
)

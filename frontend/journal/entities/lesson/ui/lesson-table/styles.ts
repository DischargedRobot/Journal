import type { SxProps, Theme } from "@mui/material/styles"

export const headerCellSx: SxProps<Theme> = {
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

export const bodyCellSx: SxProps<Theme> = {
    borderColor: "divider",
    borderWidth: 1,
    borderStyle: "solid",
    textAlign: "center",
    verticalAlign: "middle",
    px: 1,
    py: 1,
    fontSize: "0.875rem",
}

/** Колонка «на месте» при drag (превью рисуется отдельно в DragOverlay). */
export const draggingColumnSx = {
    opacity: 0.35,
} as const

export const overlayBodyCellSx: SxProps<Theme> = {
    ...bodyCellSx,
    flex: 1,
    color: "text.primary",
    minHeight: 41,
    display: "flex",
    alignItems: "center",
    justifyContent: "center",
}

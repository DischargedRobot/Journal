"use client"

import MoreVertIcon from "@mui/icons-material/MoreVert"
import IconButton from "@mui/material/IconButton"
import type { LessonMoreToolsButtonProps } from "./types"

const DefaultLessonMoreToolsButton = ({ onClick, row }: LessonMoreToolsButtonProps) => (
	<IconButton
		size="small"
		onClick={onClick}
		sx={row ? undefined : { color: "common.white" }}
		aria-label={row ? "Меню строки" : "Меню таблицы"}
	>
		<MoreVertIcon fontSize="small" />
	</IconButton>
)

export default DefaultLessonMoreToolsButton

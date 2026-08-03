"use client"

import MoreVertIcon from "@mui/icons-material/MoreVert"
import IconButton from "@mui/material/IconButton"
import type { LessonMoreToolsButtonProps } from "./types"

const DefaultLessonMoreToolsButton = ({
	onClick,
	row,
	className,
}: LessonMoreToolsButtonProps) => (
	<IconButton
		size="small"
		className={className}
		onClick={(event) => {
			event.stopPropagation()
			onClick()
		}}
		onMouseDown={(event) => event.stopPropagation()}
		onPointerDown={(event) => event.stopPropagation()}
		sx={row ? undefined : { color: "secondary.main" }}
		aria-label={row ? "Меню строки" : "Меню таблицы"}
	>
		<MoreVertIcon fontSize="small" />
	</IconButton>
)

export default DefaultLessonMoreToolsButton

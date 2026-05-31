import Box from "@mui/material/Box"
import Typography from "@mui/material/Typography"
import type { SxProps, Theme } from "@mui/material/styles"
import type { ReactNode } from "react"

export type StatCardProps = {
	icon: ReactNode
	value: ReactNode
	label: string
	className?: string
	sx?: SxProps<Theme>
	onClick?: () => void
}

const StatCard = ({
	icon,
	value,
	label,
	className = "",
	sx,
	onClick,
}: StatCardProps) => (
	<Box
		className={`flex items-center gap-3 rounded-[20px] px-4 py-3 ${className}`}
		onClick={onClick}
		sx={[
			{
				backgroundColor: "secondary.light",
				boxShadow: "0 0 25px 0 rgba(0, 0, 0, 0.08)",
				...(onClick ? { cursor: "pointer" } : {}),
			},
			...(Array.isArray(sx) ? sx : [sx]),
		]}
	>
		<Box className="flex shrink-0 items-center justify-center">{icon}</Box>
		<Box className="flex min-w-0 flex-col">
			<Typography
				className="title title_x-litle font-bold"
				sx={{ color: "contrastingSecondary.main" }}
			>
				{value}
			</Typography>
			<Typography
				className="title title_x-litle"
				sx={{ color: "default.main" }}
			>
				{label}
			</Typography>
		</Box>
	</Box>
)

export default StatCard
